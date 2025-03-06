import sys
import os
import logging
import pathlib
import re
import json
import yaml
from fontTools import ttLib

if __name__ == '__main__':
    font = ttLib.TTFont(sys.argv[1], recalcTimestamp=False)
    with open(sys.argv[2]) as f:
        symbols = json.load(f)
    with open(os.path.join(os.path.dirname(__file__), 'patch.yaml')) as f:
        segoe_map = yaml.load(f, Loader=yaml.Loader)['map']

    # reproducible build
    font['head'].modified = 2082844800

    # modify metrics
    u = font['head'].unitsPerEm
    font['hhea'].ascent = u
    font['hhea'].descent = 0
    font['hhea'].lineGap = 0
    font['OS/2'].sTypoAscender = u
    font['OS/2'].sTypoDescender = 0
    font['OS/2'].sTypoLineGap = 0
    font['OS/2'].usWinAscent = u
    font['OS/2'].usWinDescent = 0

    # modify names
    family_name = re.sub(r'([a-z])([A-Z])', r'\1 \2', pathlib.Path(sys.argv[1]).stem).replace('-', ' ')
    for name_entry in font['name'].names:
        if name_entry.nameID in [1, 3, 4]:
            name_entry.string = family_name

    for cmap_table in font['cmap'].tables:
        # remove temporary codepoints
        removing = [k for k in cmap_table.cmap.keys() if k >= 0xe0000 and k < 0xf0000]
        for k in removing:
            del cmap_table.cmap[k]

        def get_symbol(c : int):
            for symbol, s in symbols.items():
                if s == c:
                    return symbol
            return c

        # map fallback icons
        def fallback(start : int, end : int):
            for c in range(start, end, 4):
                regular_defined = c in cmap_table.cmap
                filled_defined = c + 1 in cmap_table.cmap
                color_defined = c + 2 in cmap_table.cmap
                light_defined = c + 3 in cmap_table.cmap
                rtl_regular_defined = c + 0x10000 in cmap_table.cmap
                rtl_filled_defined = c + 0x10001 in cmap_table.cmap
                rtl_color_defined = c + 0x10002 in cmap_table.cmap
                rtl_light_defined = c + 0x10003 in cmap_table.cmap

                if not (regular_defined or filled_defined or color_defined or light_defined):
                    continue

                if not regular_defined:
                    if (filled_defined):
                        cmap_table.cmap[c] = cmap_table.cmap[c + 1]
                if not filled_defined:
                    if (regular_defined):
                        cmap_table.cmap[c + 1] = cmap_table.cmap[c]

                if regular_defined or filled_defined:
                    if not rtl_regular_defined:
                        cmap_table.cmap[c + 0x10000] = cmap_table.cmap[c]
                        if regular_defined and (rtl_filled_defined or rtl_color_defined or rtl_light_defined):
                            logging.warning('Regular RTL variant for {} is unexpectedly missing'.format(get_symbol(c)))
                    if not rtl_filled_defined:
                        cmap_table.cmap[c + 0x10001] = cmap_table.cmap[c + 1]
                        if filled_defined and (rtl_regular_defined or rtl_color_defined or rtl_light_defined):
                            logging.warning('Filled RTL variant for {} is unexpectedly missing'.format(get_symbol(c)))

                if color_defined and not rtl_color_defined:
                    cmap_table.cmap[c + 0x10002] = cmap_table.cmap[c + 2]
                    if rtl_regular_defined or rtl_filled_defined or rtl_light_defined:
                        logging.warning('Color RTL variant for {} is unexpectedly missing'.format(get_symbol(c)))
                if light_defined and not rtl_light_defined:
                    cmap_table.cmap[c + 0x10003] = cmap_table.cmap[c + 3]
                    if rtl_regular_defined or rtl_filled_defined or rtl_color_defined:
                        logging.warning('Light RTL variant for {} is unexpectedly missing'.format(get_symbol(c)))

        fallback(0x0f0000, 0x0ffffd)

        # Segoe Fluent Icons codepoints
        for codepoint, [symbol, offset] in segoe_map.items():
            target = symbols[symbol] + offset
            if target in cmap_table.cmap:
                cmap_table.cmap[codepoint] = cmap_table.cmap[target]

    font.save(sys.argv[1])
