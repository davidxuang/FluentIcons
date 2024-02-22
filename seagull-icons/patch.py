import sys
import os
import pathlib
import re
import json
import yaml
from fontTools import ttLib

if __name__ == '__main__':
    font = ttLib.TTFont(sys.argv[1])
    with open(sys.argv[2]) as f:
        symbols = json.load(f)
    with open(os.path.join(os.path.dirname(__file__), 'patch.yaml')) as f:
        segoe_map = yaml.load(f, Loader=yaml.Loader)['map']

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
    family_name = re.sub(r'([a-z])([A-Z])', r'\1 \2', pathlib.Path(sys.argv[1]).stem)
    for name_entry in font['name'].names:
        if name_entry.nameID in [1, 3, 4]:
            name_entry.string = family_name

    for cmap_table in font['cmap'].tables:
        # remove temporary codepoints
        removing = [k for k in cmap_table.cmap.keys() if k > 0 and k < 0xe000]
        for k in removing:
            del cmap_table.cmap[k]

        if 0xf0000 not in cmap_table.cmap:
            continue

        # map fallback icons
        def fallback(start : int, end : int):
            for c in range(start, end, 4):
                regular_defined = c in cmap_table.cmap
                filled_defined = c + 1 in cmap_table.cmap
                rtl_regular_defined = c + 2 in cmap_table.cmap
                rtl_filled_defined = c + 3 in cmap_table.cmap

                if (not(regular_defined or filled_defined)):
                    break

                if (not regular_defined):
                    cmap_table.cmap[c] = cmap_table.cmap[c + 1]
                elif (not filled_defined):
                    cmap_table.cmap[c + 1] = cmap_table.cmap[c]

                if (not rtl_regular_defined):
                    cmap_table.cmap[c + 2] = cmap_table.cmap[c]
                if (not rtl_filled_defined):
                    cmap_table.cmap[c + 3] = cmap_table.cmap[c + 1]

        # fallback(0xe000, 0xf900)
        fallback(0x0f0000, 0x0ffffd)
        fallback(0x100000, 0x10fffd)

        # Segoe Fluent Icons codepoints
        for codepoint, target in segoe_map.items():
            cmap_table.cmap[codepoint] = cmap_table.cmap[symbols[target[0]] + target[1]]

    font.save(sys.argv[1])
