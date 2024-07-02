import * as fs from 'fs';
import * as path from 'path';
import * as yargs from 'yargs';
import { Parser } from 'xml2js';
import * as paper from 'paper';
import { Doc, Visible } from './types';
import { getPathData } from './utils';
import { Matrix } from 'paper/dist/paper-core';

const argv = yargs
  .string('json')
  .string('dir')
  .array('dir')
  .strict()
  .parseSync();

paper.setup([32, 32]);

const parser = new Parser({
  preserveChildrenOrder: true,
  explicitChildren: true,
  explicitArray: true,
});

const mirror_set = new Set<string>(
  JSON.parse(fs.readFileSync(argv.json).toString())
);

argv.dir.forEach((d) => {
  fs.readdirSync(d).forEach((f) => {
    const src_item = path.join(d, f);
    const dest_item = path.join(d, 'RTL', f);
    if (!mirror_set.has(f) || fs.existsSync(dest_item)) return;

    parser.parseString(fs.readFileSync(src_item), (err, doc: Doc) => {
      if (err) {
        throw src_item;
      }

      const item = new paper.CompoundPath(
        doc.svg.$$.map((e) => getPathData(e as Visible)).join()
      );
      item.transform(new Matrix(-1, 0, 0, 1, parseInt(doc.svg.$.width), 0));
      fs.writeFileSync(
        dest_item,
        `<svg width="${doc.svg.$.width}" height="${doc.svg.$.height}" viewBox="${doc.svg.$.viewBox}" xmlns="http://www.w3.org/2000/svg">\n  <path d="${item.pathData}" fill="#212121" />\n</svg>`
      );
    });
  });
});
