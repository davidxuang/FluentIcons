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
  if (path.extname(f) !== '.svg') {
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
    if (
      bounds.left < -tolerance ||
      bounds.top < -tolerance ||
      bounds.right > 16 + tolerance ||
      bounds.bottom > 16 + tolerance
    ) {
      console.warn(`${file}: ${bounds}`);
    }

    item.remove();
  });
});
