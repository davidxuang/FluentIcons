import sys
from os import path
from fontTools import ttLib

if __name__ == '__main__':
    font = ttLib.TTFont(sys.argv[1])
    u = font['head'].unitsPerEm
    font['hhea'].ascent = u
    font['hhea'].descent = 0
    font['hhea'].lineGap = 0
    font['OS/2'].sTypoAscender = u
    font['OS/2'].sTypoDescender = 0
    font['OS/2'].sTypoLineGap = 0
    font['OS/2'].usWinAscent = u
    font['OS/2'].usWinDescent = 0
    font.save(sys.argv[2])
