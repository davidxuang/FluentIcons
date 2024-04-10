import * as fs from 'fs';
import * as path from 'path';
import * as yargs from 'yargs';
import { Parser } from 'xml2js';
import * as xmlbuilder from 'xmlbuilder';
import * as color from 'color-string';
import * as paper from 'paper';
import { Doc, Visible } from './types';
import { ensure, forEachDrawable, getPathData } from './utils';

const argv = yargs
  .string('mono')
  .string('source')
  .string('colr20')
  .string('colr16')
  .string('ttx')
  .strict()
  .parseSync();
const MONO_DIR = argv.mono;
const SRC_DIR = argv.source;
const COLR_20_DIR = argv.colr20;
const COLR_16_DIR = argv.colr16;
const COLR_TTX = argv.ttx;

paper.setup([20, 20]);

const parser = new Parser({
  preserveChildrenOrder: true,
  explicitChildren: true,
  explicitArray: true,
});

const glyphs = new Map<string, [string, number][]>();
const cmpts = new Map<string, string>();
const cpals = new Array<string>();

fs.readdirSync(MONO_DIR).forEach((f) => {
  const file = path.join(SRC_DIR, f);
  parser.parseString(fs.readFileSync(file), (err, doc: Doc) => {
    if (err) {
      throw file;
    }

    const name = path.parse(file).name;
    const layers: [string, number][] = [];
    forEachDrawable(doc.svg.$$, (elem, e) => {
      const path_data = getPathData(elem);
      let fill = color.to.hex(
        color.get((elem as Visible).$.fill ?? doc.svg.$['fill'] ?? 'black').value
      );
      if (fill.length === 7) {
        fill = fill + 'FF';
      }
      let c = cpals.indexOf(fill);
      if (c < 0) {
        c = cpals.push(fill) - 1;
      }
      const pair = Array.from(cmpts.entries()).find(
        ([_, v]) => path_data === v
      );
      if (pair) {
        layers.push([pair[0], c]);
      } else {
        cmpts.set(`${name}-x${e}`, path_data);
        layers.push([`${name}-x${e}`, c]);
      }
    });

    glyphs.set(name, layers);
  });
});

ensure(COLR_16_DIR);
ensure(COLR_20_DIR);
cmpts.forEach((pathData, name) => {
  fs.writeFileSync(
    path.join(COLR_20_DIR, `${name}.svg`),
    `<svg width="20" height="20" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">\n  <path d="${pathData}" fill="#212121" />\n</svg>`
  );
  const item = new paper.CompoundPath(pathData);
  item.translate([-2, -2]);
  fs.writeFileSync(
    path.join(COLR_16_DIR, `${name}.svg`),
    `<svg width="16" height="16" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg">\n  <path d="${item.pathData}" fill="#212121" />\n</svg>`
  );
});

const ttFont = xmlbuilder.create('ttFont');
ttFont.att('sfntVersion', '\\x00\\x01\\x00\\x00');
ttFont.att('ttLibVersion', '3.0');

// COLR
const COLR = ttFont.ele('COLR');
COLR.ele('version', { value: 0 });
glyphs.forEach((layers, name) => {
  const glyph = COLR.ele('ColorGlyph', { name: name });
  layers.forEach(([layer, color]) => {
    glyph.ele('layer', { colorID: color, name: layer });
  });
});
const CPAL = ttFont.ele('CPAL');
CPAL.ele('version', { value: 0 });
CPAL.ele('numPaletteEntries', { value: cpals.length });
const palette = CPAL.ele('palette', { index: 0 });
cpals.forEach(function (color, c) {
  if (color.startsWith('url')) {
    console.warn('unexpected color: ' + color);
    color = '#000000ff';
  }
  palette.ele('color', { index: c, value: color });
});

let ttx = fs.createWriteStream(COLR_TTX);
ttx.write(`<?xml version="1.0" encoding="UTF-8"?>\n`);
ttx.write(ttFont.toString());
ttx.end();
