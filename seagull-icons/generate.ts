import * as fs from 'fs';
import * as path from 'path';
import * as yargs from 'yargs';
import * as fantasticon from 'fantasticon';

const argv = yargs
  .string('source')
  .string('override')
  .string('codepoints')
  .string('colr')
  .string('name')
  .number('font-height')
  .string('dest')
  .strict()
  .parseSync();

const symbols: { [symbol: string]: number } = JSON.parse(
  fs.readFileSync(argv.codepoints).toString()
);
const codepoints = Object.fromEntries([
  ...Object.entries(symbols)
    .map(([symbol, codepoint]): [string, number][] => {
      return [
        [`ic_fluent_${symbol}_regular`, codepoint],
        [`ic_fluent_${symbol}_filled`, codepoint + 1],
        [`ic_fluent_${symbol}_light`, codepoint + 2],
        [`ic_fluent_${symbol}_rtl_regular`, codepoint + 3],
        [`ic_fluent_${symbol}_rtl_filled`, codepoint + 4],
        [`ic_fluent_${symbol}_rtl_light`, codepoint + 5],
      ];
    })
    .flat(),
  // use ASCII codepoints for now
  ...fs
    .readdirSync(argv.colr)
    .filter((f) => f.endsWith('.svg'))
    .map((f, i): [string, number] => [path.parse(f).name, i + 1]),
]);

fs.cpSync(argv.override, argv.source, { recursive: true, force: true });
fs.cpSync(argv.colr, argv.source, { recursive: true, force: true });

const rtl_dir = path.join(argv.source, 'RTL');
if (fs.existsSync(rtl_dir)) {
  fs.readdirSync(rtl_dir).forEach((name) => {
    fs.copyFileSync(
      path.join(rtl_dir, name),
      path.join(
        argv.source,
        name.replace(/_(regular|filled|light).svg$/, '_rtl_$1.svg')
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
    fontHeight: argv.fontHeight,
    normalize: true,
  });
}

main();
