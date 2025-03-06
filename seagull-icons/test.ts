import fs from 'fs';
import path from 'path';
import yargs from 'yargs';
import { Parser } from 'xml2js';
import paper from 'paper';
import { Doc, Renderable } from './types';
import { getPathData } from './utils';

const argv = yargs.string('source').strict().parseSync();

paper.setup([16, 16]);
const tolerance = 1 / 12;

const parser = new Parser({
  preserveChildrenOrder: true,
  explicitChildren: true,
  explicitArray: true,
});

fs.readdirSync(argv.source, { recursive: true }).forEach((f) => {
  if (
    typeof f !== 'string' ||
    path.extname(f) !== '.svg' ||
    f.endsWith('-light.svg')
  ) {
    return;
  }

  const file = path.join(argv.source, f);
  parser.parseString(fs.readFileSync(file), (err, doc: Doc) => {
    if (err) {
      throw file;
    }

    const item = new paper.CompoundPath(
      doc.svg.$$.map((e) => getPathData(e as Renderable)).join()
    );
    const bounds = item.bounds;
    const margin = [-bounds.left, -bounds.top, bounds.right - 16, bounds.bottom - 16];
    if (Math.max(...margin) >= tolerance) {
      console.warn(`[${Math.max(...margin).toFixed(4)}] ${file}: [${margin.map((m) => m.toFixed(4)).join(', ')}]`);
    }

    item.remove();
  });
});
