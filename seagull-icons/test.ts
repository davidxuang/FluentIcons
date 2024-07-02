import * as fs from 'fs';
import * as path from 'path';
import * as yargs from 'yargs';
import { Parser } from 'xml2js';
import * as paper from 'paper';
import { Doc, Visible } from './types';
import { getPathData } from './utils';

const argv = yargs.string('source').strict().parseSync();

paper.setup([16, 16]);
const tolerance = 1 / 16;

const parser = new Parser({
  preserveChildrenOrder: true,
  explicitChildren: true,
  explicitArray: true,
});

fs.readdirSync(argv.source, { recursive: true }).forEach((f) => {
  if (
    typeof f !== 'string' ||
    path.extname(f) !== '.svg' ||
    f.endsWith('_light.svg')
  ) {
    return;
  }

  const file = path.join(argv.source, f);
  parser.parseString(fs.readFileSync(file), (err, doc: Doc) => {
    if (err) {
      throw file;
    }

    const item = new paper.CompoundPath(
      doc.svg.$$.map((e) => getPathData(e as Visible)).join()
    );
    const bounds = item.bounds;
    const out_bound = Math.max(
      -bounds.left,
      -bounds.top,
      bounds.right - 16,
      bounds.bottom - 16
    );
    if (out_bound > tolerance) {
      console.warn(`[${out_bound.toFixed(4)}] ${file}: ${bounds}`);
    }

    item.remove();
  });
});
