import fs from 'fs';
import path from 'path';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import * as fantasticon from 'fantasticon';
import { resolveName } from './utils.js';

const argv = yargs()
  .string('source')
  .string('override')
  .array('override')
  .string('codepoints')
  .string('colr')
  .string('name')
  .number('units-em')
  .string('dest')
  .strict()
  .parseSync(hideBin(process.argv));

const symbols: { [symbol: string]: number } = JSON.parse(
  fs.readFileSync(argv.codepoints).toString()
);
const codepoints = Object.fromEntries([
  ...Object.entries(symbols)
    .map(([symbol, codepoint]): [string, number][] => {
      return [
        [`${symbol}-regular`, codepoint],
        [`${symbol}-filled`, codepoint + 1],
        [`${symbol}-color`, codepoint + 2],
        [`${symbol}-light`, codepoint + 3],
        [`${symbol}_rtl-regular`, codepoint + 0x10000],
        [`${symbol}_rtl-filled`, codepoint + 0x10001],
        [`${symbol}_rtl-color`, codepoint + 0x10002],
        [`${symbol}_rtl-light`, codepoint + 0x10003],
      ];
    })
    .flat(),
  // use ASCII codepoints for now
  ...fs
    .readdirSync(argv.colr)
    .filter((f) => f.endsWith('.svg'))
    .map((f, i): [string, number] => [path.parse(f).name, 0xe0000 + i]),
]);

argv.override?.forEach((override) => {
  fs.cpSync(override, argv.source, { recursive: true, force: true });
});
fs.cpSync(argv.colr, argv.source, { recursive: true, force: true });

const rtl_dir = path.join(argv.source, 'RTL');
if (fs.existsSync(rtl_dir)) {
  fs.readdirSync(rtl_dir).forEach((name) => {
    const spec = resolveName(name);
    fs.copyFileSync(
      path.join(rtl_dir, name),
      path.join(
        argv.source,
        `${spec.name}_rtl-${spec.variant}.svg`
      )
    );
  });
  fs.rmSync(rtl_dir, { recursive: true });
}

async function main() {
  await fantasticon.generateFonts({
    inputDir: path.resolve(argv.source),
    outputDir: path.resolve(argv.dest),
    name: argv.name,
    fontTypes: [fantasticon.ASSET_TYPES.TTF],
    assetTypes: [fantasticon.ASSET_TYPES.HTML, fantasticon.ASSET_TYPES.CSS],
    formatOptions: { json: { indent: 2 } },
    codepoints: codepoints,
    fontHeight: argv.unitsEm,
    normalize: true,
  });
}

main();
