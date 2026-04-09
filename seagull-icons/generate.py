import argparse
import hashlib
import _hashlib
import json
import logging
import os
import re
import shutil
import tomllib
from typing import Any, Optional, cast
from xml.etree import ElementTree

from fontTools.fontBuilder import FontBuilder
from fontTools.otlLib.builder import SingleSubstBuilder
from fontTools.pens.cu2quPen import Cu2QuPen
from fontTools.pens.t2CharStringPen import T2CharString, T2CharStringPen
from fontTools.pens.ttGlyphPen import Glyph, TTGlyphPen
from fontTools.svgLib import SVGPath
from fontTools.ttLib import TTFont, newTable
from fontTools.ttLib.tables import otTables

from cffsubr import subroutinize

logging.basicConfig(format='[%(levelname)s] %(message)s')

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

class HashWriter:
    def __init__(self, hasher: _hashlib.HASH):
        self.hasher = hasher

    def simpletag(self, tag: str, attrs: list[tuple[str, Any]] | None = None):
        self.hasher.update(f'<{tag}'.encode('utf-8'))
        for k, v in attrs or []:
            self.hasher.update(k.encode('utf-8'))
            self.hasher.update(str(v).encode('utf-8'))
        self.hasher.update(b'/>')

    def begintag(self, tag: str, attrs: list[tuple[str, Any]] | None = None):
        self.hasher.update(f'<{tag}'.encode('utf-8'))
        for k, v in attrs or []:
            self.hasher.update(k.encode('utf-8'))
            self.hasher.update(str(v).encode('utf-8'))
        self.hasher.update(b'>')

    def endtag(self, tag: str):
        self.hasher.update(f'</{tag}>'.encode('utf-8'))

    def newline(self):
        self.hasher.update(b'\n')

re_lang_tag = re.compile(r'\.[A-Za-z]{2}(?:-[A-Za-z]{4})?(?:-[A-Za-z]{2})?\.')
re_name = re.compile(r'^(((?P<icon>[^.]+?)\.(?P<variant>regular|filled|color|light))(?:\.(?P<rtl>rtl))?)(?:\.(?P<lang>\w{2}(?:-\w{4})?(?:-\w{2})?))?(?P<ext>\.svg)?$')

def build_font(input: str, output: str, ps_name: str, icons: dict[str, int], remap: dict[int, int], unify: dict[str, list[str]], upm: int, is_ttf: bool) -> dict[str, str]:
    fb = FontBuilder(unitsPerEm=upm, isTTF=is_ttf)

    family_name = re.sub(r'([a-z])([A-Z])', r'\1 \2', ps_name).replace('-', ' ')

    glyphs : dict[str, Glyph | T2CharString] = {}
    glyphs_hash : dict[str, bytes] = {}
    glyphs_map : dict[bytes, str] = {}
    subst : dict[str, str] = {}

    cmap : dict[int, str] = {}
    hmtx : dict[str, tuple[int, int]] = {}

    files = sorted(os.listdir(input), key=lambda f: int(f.startswith('_')) * 4
                                                + int(re_lang_tag.search(f) is not None) * 2
                                                + int(f.find('.rtl.') >= 0))

    for fname in files:
        name, ext = os.path.splitext(fname)
        if ext.lower() != '.svg':
            continue
        path = os.path.join(input, fname)

        xml = ElementTree.parse(path)
        scale = upm / float(xml.getroot().get('height', 20))
        svg = SVGPath(path, transform=(scale, 0, 0, -scale, 0, upm))
        hasher = hashlib.shake_256()
        if is_ttf:
            pen = TTGlyphPen(glyphSet=None)
            svg.draw(Cu2QuPen(pen, max_err=1.0, reverse_direction=True))
            glyph = pen.glyph()
            glyph.recalcBounds(None)
            lsb = glyph.xMin

            writer = HashWriter(hasher)
            glyph.toXML(writer, ttFont=None)
            hash = hasher.digest(32)
        else:
            pen = T2CharStringPen(width=upm, glyphSet=None)
            svg.draw(pen)
            glyph = pen.getCharString()
            lsb = 0

            glyph.decompile()
            for program in cast(list, glyph.program):
                hasher.update(str(program).encode('utf-8'))
            hash = hasher.digest(32)

        # dedup (COLR layers and localized variants)
        if hash in glyphs_map:
            if name.startswith('_'):
                subst[name] = glyphs_map[hash]
                logging.debug(f'Linking {name} to {glyphs_map[hash]}')
                # if name in icons:
                #     cmap[icons[name]] = glyphs_map[hash]
                continue
            else:
                m = re_name.match(name)
                if m and m.group('lang'):
                    base = m.group(1)
                    if hash == glyphs_hash.get(base):
                        subst[name] = base
                        logging.debug(f'Removing {name} on {base}')
                        continue

        elif not name.endswith('.color'):
            glyphs_map[hash] = name

        glyphs[name] = glyph
        glyphs_hash[name] = hash
        hmtx[name] = (upm, lsb)
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
    for codepoint, target in remap.items():
        if target in cmap:
            cmap[codepoint] = cmap[target]

    for name, aliases in unify.items():
        codepoints = [icons[f'{name}.regular']] + [icons[f'{alias}.regular'] for alias in aliases]
        targets : dict[int, str] = {}
        can_unify = True
        dedup : set[tuple[str, ...]] = set()
        for offset in [0, 1, 2, 3, 0x10000, 0x10001, 0x10002, 0x10003]:
            values = [ cmap[cp + offset] for cp in codepoints if (cp + offset) in cmap ]
            distinct = len(set(values))
            if distinct == 0:
                continue
            elif distinct == 1:
                targets[offset] = values[0]
                continue
            hashes = { glyphs_hash[v] for v in values }
            if len(hashes) == 1:
                dedup.add(tuple(values))
                targets[offset] = values[0]
            elif len(values) > 1:
                logging.warning(f'Cannot unify {name} with {", ".join(aliases)}')
                can_unify = False
                break
        if can_unify:
            for offset, target in targets.items():
                for cp in codepoints:
                    cmap[cp + offset] = target
            # dedup (unification candidates)
            for group in dedup:
                for name in group[1:]:
                    logging.debug(f'Linking {name} to {group[0]}')
                    subst[name] = group[0]
                    glyph_order.remove(name)
                    glyphs.pop(name)
                    hmtx.pop(name)
                    # TODO: rename potentially orphaned localized variants

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

meta_locl = {
  'ar': { 'arab': ['dflt', 'ARA '] },
  'bg': { 'cyrl': 'BGR ' },
  'ca': { 'latn': 'CAT ' },
  'da': { 'latn': 'DAN ' },
  'de': { 'latn': 'DEU ' },
  'el': { 'grek': ['dflt', 'ELL ', 'PGR '] }, # `gr`
  'en': { 'latn': 'ENG ' },
  'es': { 'latn': 'ESP ' },
  'et': { 'latn': 'ETI ' },
  'eu': { 'latn': 'EUQ ' },
  'fi': { 'latn': 'FIN ' },
  'fr': { 'latn': 'FRA ' },
  'gl': { 'latn': 'GAL ' },
  'he': { 'hebr': ['dflt', 'IWR '] },
  'hu': { 'latn': 'HUN ' },
  'it': { 'latn': 'ITA ' },
  'ja': { 'hani': 'JAN ', 'kana': ['dflt', 'JAN '] },
  'kk-cyrl': { 'cyrl': 'KAZ ' },
  'ko': { 'hang': ['dflt', 'KOR '] },
  'lt': { 'latn': 'LTH ' },
  'lv': { 'latn': 'LVI ' },
  'ms': { 'latn': 'MLY ' },
  'no': { 'latn': ['NOR ', 'NYN '] },
  'pt': { 'latn': 'PTG ' },
  'ru': { 'cyrl': 'RUS ' },
  'se': { 'latn': 'NSM ' },
  'sl': { 'latn': 'SLV ' },
  'sr': { 'cyrl': ['SRB ', 'BOS '], 'latn': ['SRB ', 'BOS ', 'HRV '] },
  'sr-cyrl': { 'cyrl': ['SRB ', 'BOS '] },
  'sr-latn': { 'latn': ['SRB ', 'BOS ', 'HRV '] },
  'sv': { 'latn': 'SVE ' },
  'tr': { 'latn': 'TRK ' },
  'uk': { 'cyrl': 'UKR ' },
  'zh': { 'hani': ['dflt', 'ZHH ', 'ZHS ', 'ZHT ', 'ZHTM'], 'bopo': ['dflt', 'ZHP '] },
}

def patch_font(path: str, ttx: Optional[str]) -> None:
    font = TTFont(path, recalcTimestamp=False)

    # reproducible build
    font['head'].created = 2082844800 # pyright: ignore[reportAttributeAccessIssue]
    font['head'].modified = 2082844800

    if ttx is not None:
        font.importXML(ttx)

    if path.endswith('.ttf'):
        names = font['glyf'].glyphs.keys()
    else:
        names = font['CFF '].cff.topDictIndex[0].CharStrings.charStrings.keys()

    _lookups : dict[str, dict[str, str]] = { k: {} for k in meta_locl.keys() }
    for name in names:
        if name.startswith('_'):
            continue
        m = re_name.match(name)
        if m and m.group('lang'):
            base = m.group(1)
            lang = m.group('lang')
            if lang not in _lookups:
                raise ValueError(f'Unknown language tag: {lang} in {name}')
            _lookups[lang][base] = name
    _lookups = { f: _lookups[f] for f in meta_locl.keys() if len(_lookups[f]) > 0 }

    lookups = otTables.LookupList()
    lookups.Lookup = [] # pyright: ignore
    features = otTables.FeatureList() # pyright: ignore
    features.FeatureRecord = []
    feature_index : dict[str, int] = {}
    for lang, mapping in _lookups.items():
        builder = SingleSubstBuilder(font, location=None)
        for base, alt in mapping.items():
            builder.mapping[base] = alt
        lookups.Lookup.append(builder.build())
        feat = otTables.FeatureRecord() # pyright: ignore
        feat.FeatureTag = 'locl'
        feat.Feature = otTables.Feature() # pyright: ignore
        feat.Feature.LookupListIndex = [len(lookups.Lookup) - 1]
        features.FeatureRecord.append(feat)
        feature_index[lang] = len(features.FeatureRecord) - 1

    _scripts : dict[str, dict[str, list[str]]] = { 'DFLT': {} }
    for lang, ot_system in meta_locl.items():
        if lang not in _lookups:
            continue
        for script, ot_langs in ot_system.items():
            if isinstance(ot_langs, str):
                ot_langs = [ot_langs]
            if script not in _scripts:
                _scripts[script] = {}
            if lang.find('-') == -1:
                for ot_lang in ot_langs:
                    if ot_lang != 'dflt':
                        _scripts['DFLT'][ot_lang] = [lang]
                        _scripts[script][ot_lang] = [lang]
                    else:
                        _scripts[script]['dflt'] = [lang]
            else:
                for ot_lang in ot_langs:
                    if not ot_lang in _scripts[script]:
                        _scripts[script][ot_lang] = []
                    _scripts[script][ot_lang].append(lang)
    _scripts = { s: langs for s, langs in _scripts.items() if len(langs) > 0 }

    scripts = otTables.ScriptList() # pyright: ignore
    scripts.ScriptRecord = []
    for s, langs in _scripts.items():
        script_rec = otTables.ScriptRecord() # pyright: ignore
        script_rec.ScriptTag = s

        script = otTables.Script() # pyright: ignore
        script.LangSysRecord = []
        script_rec.Script = script
        for ot_lang, langs in langs.items():
            sys = otTables.LangSys() # pyright: ignore
            sys.ReqFeatureIndex = 0xffff
            sys.FeatureIndex = [feature_index[lang] for lang in langs if lang in feature_index]
            if ot_lang == 'dflt':
                script.DefaultLangSys = sys
            else:
                sys_rec = otTables.LangSysRecord() # pyright: ignore
                sys_rec.LangSysTag = ot_lang
                sys_rec.LangSys = sys
                script.LangSysRecord.append(sys_rec)
        scripts.ScriptRecord.append(script_rec)

    if len(scripts.ScriptRecord) > 0:
        gsub = otTables.GSUB() # pyright: ignore
        gsub = otTables.GSUB() # pyright: ignore
        gsub.Version = 0x00010000
        gsub.ScriptList = scripts
        gsub.FeatureList = features
        gsub.LookupList = lookups
        GSUB = newTable('GSUB')
        GSUB.table = gsub # pyright: ignore
        font['GSUB'] = GSUB

    # optimization
    if 'CFF ' in font or 'CFF2' in font:
        subroutinize(font)

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
                f'{icon}.regular': codepoint,
                f'{icon}.filled': codepoint + 1,
                f'{icon}.color': codepoint + 2,
                f'{icon}.light': codepoint + 3,
                f'{icon}.regular.rtl': codepoint + 0x10000,
                f'{icon}.filled.rtl': codepoint + 0x10001,
                f'{icon}.color.rtl': codepoint + 0x10002,
                f'{icon}.light.rtl': codepoint + 0x10003,
            })

    remap : dict[int, int] = {}
    unify : dict[str, list[str]]
    with open(os.path.join(os.path.dirname(__file__), 'generate.toml'), 'rb') as f:
        meta = tomllib.load(f)
        unify = meta.get('unification', {})
        for k, [g, h] in meta['segoe'].items():
            remap[int(k, 0)] = icons[f'{g}.regular'] + h

    for override in args.override or []:
        shutil.copytree(override, getattr(args, 'in'), dirs_exist_ok=True)
    if args.colr:
        shutil.copytree(args.colr, getattr(args, 'in'), dirs_exist_ok=True)
        if os.path.exists(os.path.join(getattr(args, 'colr'), 'redirects.json')):
            with open(os.path.join(getattr(args, 'colr'), 'redirects.json'), 'r', encoding='utf-8') as f:
                redirects = json.load(f)
                for k, v in redirects.items():
                    remap[icons[k]] = icons[v]
                    remap[icons[k] + 0x10000] = icons[v] + 0x10000

    rtl_dir = os.path.join(getattr(args, 'in'), 'RTL')
    if os.path.exists(rtl_dir):
        for fname in os.listdir(rtl_dir):
            m = re_name.match(fname)
            assert m is not None
            lang = f'.{m.group("lang")}' if m.group('lang') else ''
            rname = f'{m.group(2)}.rtl{lang}{m.group("ext")}'
            shutil.copy(os.path.join(rtl_dir, fname), os.path.join(getattr(args, 'in'), rname))
        shutil.rmtree(rtl_dir)

    subst = build_font(getattr(args, 'in'), args.out, args.name, icons, remap, unify, args.upm, args.format == 'ttf')
    colr = os.path.join(args.colr, 'colr.ttx')
    if os.path.exists(colr):
        patch_ttx(colr, subst)
    else:
        colr = None
    patch_font(os.path.join(args.out, f'{args.name}.{args.format}'), colr)
