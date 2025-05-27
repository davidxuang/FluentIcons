import fs from 'fs';
import path, { resolve } from 'path';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import { Parser } from 'xml2js';
import paper from 'paper';
import { Doc, Renderable } from './types.js';
import { ensure, getPathData, resolveName } from './utils.js';

const argv = yargs()
  .string('config')
  .string('dir')
  .array('dir')
  .strict()
  .parseSync(hideBin(process.argv));

paper.setup([32, 32]);

const parser = new Parser({
  preserveChildrenOrder: true,
  explicitChildren: true,
  explicitArray: true,
});

const mirror_set = new Set<string>(
  JSON.parse(fs.readFileSync(argv.config).toString())
);

argv.dir.forEach((d) => {
  ensure(path.join(d, 'RTL'));
  fs.readdirSync(d).forEach((f) => {
    if (!f.endsWith('.svg')) return;

    const src_item = path.join(d, f);
    const dest_item = path.join(d, 'RTL', f);

    const spec = resolveName(f);
    if (spec === null || !mirror_set.has(spec.name) || fs.existsSync(dest_item))
      return;

    parser.parseString(fs.readFileSync(src_item), (err, doc: Doc) => {
      if (err) {
        throw src_item;
      }

      const item = new paper.CompoundPath(
        doc.svg.$$.map((e) => getPathData(e as Renderable)).join()
      );
      item.transform(
        new paper.Matrix(-1, 0, 0, 1, parseInt(doc.svg.$.width), 0)
      );
      fs.writeFileSync(
        dest_item,
        `<svg width="${doc.svg.$.width}" height="${doc.svg.$.height}" viewBox="${doc.svg.$.viewBox}" xmlns="http://www.w3.org/2000/svg">\n  <path d="${item.pathData}" fill="#212121" />\n</svg>`
      );
    });
  });
});
