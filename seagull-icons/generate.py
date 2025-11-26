import argparse
import io
import json
import logging
import os
import re
import shutil
import tomllib
from typing import Optional
from xml.etree import ElementTree

from fontTools.fontBuilder import FontBuilder
from fontTools.pens.cu2quPen import Cu2QuPen
from fontTools.pens.t2CharStringPen import T2CharStringPen
from fontTools.pens.ttGlyphPen import TTGlyphPen
from fontTools.svgLib import SVGPath
from fontTools.ttLib import TTFont
from fontTools.misc.xmlWriter import XMLWriter

def fallback(cmap: dict[int, str], start: int, end: int):
    for c in range(start, end, 4):
        regular_defined = c in cmap
        filled_defined = c + 1 in cmap
        color_defined = c + 2 in cmap
        light_defined = c + 3 in cmap
        rtl_regular_defined = c + 0x10000 in cmap
        rtl_filled_defined = c + 0x10001 in cmap
        rtl_color_defined = c + 0x10002 in cmap
        rtl_light_defined = c + 0x10003 in cmap

        if not (regular_defined or filled_defined or color_defined or light_defined):
            continue

        if not regular_defined:
            if filled_defined:
                cmap[c] = cmap[c + 1]
        if not filled_defined:
            if regular_defined:
                cmap[c + 1] = cmap[c]

        if regular_defined or filled_defined:
            if not rtl_regular_defined:
                cmap[c + 0x10000] = cmap[c]
                if regular_defined and (rtl_filled_defined or rtl_color_defined or rtl_light_defined):
                    logging.warning('Regular RTL variant for %s is unexpectedly missing', hex(c))
            if not rtl_filled_defined:
                cmap[c + 0x10001] = cmap[c + 1]
                if filled_defined and (rtl_regular_defined or rtl_color_defined or rtl_light_defined):
                    logging.warning('Filled RTL variant for %s is unexpectedly missing', hex(c))

        if color_defined and not rtl_color_defined:
            cmap[c + 0x10002] = cmap[c + 2]
            if rtl_regular_defined or rtl_filled_defined or rtl_light_defined:
                logging.warning('Color RTL variant for %s is unexpectedly missing', hex(c))
        if light_defined and not rtl_light_defined:
            cmap[c + 0x10003] = cmap[c + 3]
            if rtl_regular_defined or rtl_filled_defined or rtl_color_defined:
                logging.warning('Light RTL variant for %s is unexpectedly missing', hex(c))

def build_font(input: str, output: str, ps_name: str, icons: dict[str, int], segoe: dict[int, tuple[str, int]], upm: int, is_ttf: bool) -> dict[str, str]:
    fb = FontBuilder(unitsPerEm=upm, isTTF=is_ttf)

    family_name = re.sub(r'([a-z])([A-Z])', r'\1 \2', ps_name).replace('-', ' ')

    glyphs = {}
    r_glyphs = {}
    subst = {}

    cmap = {}
    hmtx = {}

    for fname in os.listdir(input):
        name, ext = os.path.splitext(fname)
        if ext.lower() != '.svg':
            continue
        path = os.path.join(input, fname)

        xml = ElementTree.parse(path)
        scale = upm / float(xml.getroot().get('height', 20))
        svg = SVGPath(path, transform=(scale, 0, 0, -scale, 0, upm))
        if is_ttf:
            pen = TTGlyphPen(glyphSet=None)
            svg.draw(Cu2QuPen(pen, max_err=1.0, reverse_direction=True))
            glyph = pen.glyph()
            buffer = io.StringIO()
            writer = XMLWriter(buffer)
            glyph.toXML(writer, ttFont=None)
            gkey = buffer.getvalue()
        else:
            pen = T2CharStringPen(width=upm, glyphSet=None)
            svg.draw(pen)
            glyph = pen.getCharString()
            glyph.decompile()
            gkey = " ".join(str(item) for item in glyph.program)

        # dedup
        if gkey in r_glyphs and not name.endswith('-color'): # no dedup for color glyphs
            subst[name] = r_glyphs[gkey]
            logging.debug(f'Linking {name} to {r_glyphs[gkey]}')
            if name in icons:
                cmap[icons[name]] = r_glyphs[gkey]
        else:
            glyphs[name] = glyph
            if is_ttf:
                glyph.recalcBounds(None)
                hmtx[name] = (upm, glyph.xMin)
            else:
                hmtx[name] = (upm, 0)

            if not name.endswith('-color'):
                r_glyphs[gkey] = name
            if name in icons:
                cmap[icons[name]] = name

    glyph_order = ['.notdef'] + sorted(glyphs.keys())

    if is_ttf:
        pen = TTGlyphPen(glyphSet=None)
        glyphs['.notdef'] = pen.glyph()
        hmtx['.notdef'] = (upm, 0)
    else:
        pen = T2CharStringPen(width=upm, glyphSet=None)
        glyphs['.notdef'] = pen.getCharString()
        hmtx['.notdef'] = (upm, 0)
    cmap[0x00] = '.notdef'

    fallback(cmap, 0x0f0000, 0x0ffffd)

    # Segoe Fluent Icons mapping
    for codepoint, [symbol, offset] in segoe.items():
        target = icons[f'{symbol}-regular'] + offset
        if target in cmap:
            cmap[codepoint] = cmap[target]

    fb.setupGlyphOrder(glyph_order)
    fb.setupCharacterMap(cmap)
    if is_ttf:
        fb.setupGlyf(glyphs)
    else:
        fb.setupCFF(
            psName=ps_name,
            charStringsDict=glyphs,
            fontInfo={
                'FullName': family_name,
                'FamilyName': family_name,
                'Weight': 'Regular',
            },
            privateDict={}
        )
    fb.setupHorizontalMetrics(hmtx)
    fb.setupHorizontalHeader(ascent=upm, descent=0)
    fb.setupNameTable({
        'familyName': family_name,
        'styleName': 'Regular',
        'uniqueFontIdentifier': family_name,
        'fullName': family_name,
        'version': 'Version 1.0',
        'psName': ps_name,
    })
    fb.setupOS2(
        version=4,
        fsType=0,
        ySubscriptXSize=upm//2,
        ySubscriptYSize=upm//2,
        ySubscriptXOffset=0,
        ySubscriptYOffset=0,
        ySuperscriptXSize=upm//2,
        ySuperscriptYSize=upm//2,
        ySuperscriptXOffset=0,
        ySuperscriptYOffset=upm//2,
        yStrikeoutSize=upm//16,
        yStrikeoutPosition=upm//2,
        achVendID='dvxg',
        fsSelection=0x40,
        sTypoAscender=upm,
        sTypoDescender=0,
        usWinAscent=upm,
        usWinDescent=0,
        ulCodePageRange1=2**31,
        sxHeight=upm//2,
        sCapHeight=upm,
        usBreakChar=0x00,
    )
    fb.setupPost()

    fb.save(os.path.join(output, f'{ps_name}.{"ttf" if is_ttf else "otf"}'))
    return subst

def patch_ttx(path: str, subst: dict[str, str]) -> None:
    ttx = ElementTree.parse(path)

    for glyph in ttx.iter('Glyph'):
        value = glyph.attrib.get('value')
        if value and value in subst:
            glyph.attrib['value'] = subst[value]
    for layer_glyph in ttx.iter('LayerGlyph'):
        value = layer_glyph.attrib.get('value')
        if value and value in subst:
            layer_glyph.attrib['value'] = subst[value]
    for base_glyph in ttx.iter('BaseGlyph'):
        value = base_glyph.attrib.get('value')
        if value and value in subst:
            base_glyph.attrib['value'] = subst[value]

    ttx.write(path)

def patch_font(path: str, ttx: Optional[str]) -> None:
    font = TTFont(path, recalcTimestamp=False)

    # reproducible build
    font['head'].created = 2082844800
    font['head'].modified = 2082844800

    if ttx is not None:
        font.importXML(ttx)
    font.save(path)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--in', type=str, nargs='?')
    parser.add_argument('--override', type=str, nargs='*')
    parser.add_argument('--colr', type=str, nargs='?')
    parser.add_argument('--icons', type=str, nargs='?')
    parser.add_argument('--out', type=str, nargs='?')
    parser.add_argument('--name', type=str, nargs='?')
    parser.add_argument('--upm', type=int, nargs='?', default=1024)
    parser.add_argument('--format', choices=['ttf', 'otf'], default='otf')

    args = parser.parse_args()

    icons = {}
    with open(args.icons, 'r', encoding='utf-8') as f:
        for icon, codepoint in json.load(f).items():
            icons.update({
                f'{icon}-regular': codepoint,
                f'{icon}-filled': codepoint + 1,
                f'{icon}-color': codepoint + 2,
                f'{icon}-light': codepoint + 3,
                f'{icon}_rtl-regular': codepoint + 0x10000,
                f'{icon}_rtl-filled': codepoint + 0x10001,
                f'{icon}_rtl-color': codepoint + 0x10002,
                f'{icon}_rtl-light': codepoint + 0x10003,
            })

    segoe = {}
    with open(os.path.join(os.path.dirname(__file__), 'generate.toml'), 'rb') as f:
        for k, v in tomllib.load(f)['segoe'].items():
            segoe[int(k, 0)] = v

    for override in args.override or []:
        shutil.copytree(override, getattr(args, 'in'), dirs_exist_ok=True)
    if args.colr:
        shutil.copytree(args.colr, getattr(args, 'in'), dirs_exist_ok=True)

    rtl_dir = os.path.join(getattr(args, 'in'), 'RTL')
    if os.path.exists(rtl_dir):
        for fname in os.listdir(rtl_dir):
            shutil.copy(os.path.join(rtl_dir, fname), os.path.join(getattr(args, 'in'), fname.replace('-', '_rtl-')))
        shutil.rmtree(rtl_dir)

    subst = build_font(getattr(args, 'in'), args.out, args.name, icons, segoe, args.upm, args.format == 'ttf')
    colr = os.path.join(args.colr, 'colr.ttx')
    if os.path.exists(colr):
        patch_ttx(colr, subst)
    else:
        colr = None
    patch_font(os.path.join(args.out, f'{args.name}.{args.format}'), colr)
