import { execSync } from 'child_process';
import fs from 'fs';
import { styleText } from 'util';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import collect, { parseCollect } from './lib/collect.js';
import transform, { parseTransform } from './lib/transform.js';
import layerize, { parseLayerize } from './lib/layerize.js';
import mirror, { parseMirror } from './lib/mirror.js';

process.chdir(import.meta.dirname);

const _ = yargs()
  .command(
    'collect',
    'Collect assets',
    (yargs) => parseCollect(yargs),
    (argv) => collect(argv),
  )
  .command(
    'transform',
    'Transform Seagull icon graphs',
    (yargs) => parseTransform(yargs),
    (argv) => transform(argv),
  )
  .command(
    'layerize',
    'Layerize color icons',
    (yargs) => parseLayerize(yargs),
    (argv) => layerize(argv),
  )
  .command(
    'mirror',
    'Mirror icons for RTL languages',
    (yargs) => parseMirror(yargs),
    (argv) => mirror(argv),
  )
  .command(
    '*',
    'Run all steps',
    (yargs) =>
      yargs
        .boolean('seagull')
        .default('format', ['otf'])
        .choices('format', ['otf', 'ttf'])
        .array('format'),
    (argv) => {
      function log(msg: string) {
        console.log(styleText('gray', msg));
      }
      log('Cleaning...');
      fs.rmSync('./obj', { recursive: true, force: true });
      log('Collecting...');
      collect();
      log('[Seagull] Transforming...');
      transform();
      log('[Seagull] Layerizing...');
      layerize({
        in: './obj/color/20',
        override: './obj/color/override',
        mono: './obj/composed/seagull',
        extra: './obj/mono/resizable',
        'extra-filter': './override/mono/20',
        size: 20,
        shrink: 2,
        upm: 1792,
        config: './layerize.toml',
        mirror: './obj/mirror.json',
        out: './obj/colr/seagull',
      });
      log('[Seagull] Mirroring...');
      mirror({
        dir: ['./obj/composed/seagull'],
        config: './obj/mirror.json',
      });
      log('[Seagull] Generating...');
      argv.format.forEach((format) => {
        execSync(
          `python ./generate.py --in=./obj/composed/seagull --colr=./obj/colr/seagull --icons=./obj/icons.json --out=./obj --name=SeagullFluentIcons --upm=1792 --format=${format}`,
          { stdio: 'inherit' },
        );
        fs.copyFileSync(
          `./obj/SeagullFluentIcons.${format}`,
          `./assets/SeagullFluentIcons.${format}`,
        );
      });

      if (argv.seagull) {
        return;
      }

      function getUnitsPerEm(size: number) {
        switch (size) {
          case 10:
            return 1000;
          case 12:
            return 1008;
          case 16:
            return 1024;
          case 20:
          case 24:
          case 28:
          case 32:
            return size * 64;
          case 48:
            return 2304;
          default:
            throw new Error(`Unsupported size: ${size}`);
        }
      }

      fs.readdirSync('./obj/mono', { withFileTypes: true })
        .filter((d) => d.isDirectory() && Number(d.name))
        .forEach((dir) => {
          const colr = fs.existsSync(`./obj/color/${dir.name}`);
          const upm = getUnitsPerEm(Number(dir.name));
          const wd = `./obj/composed/${dir.name}`;
          fs.cpSync(`./obj/mono/${dir.name}`, wd, { recursive: true });
          if (colr) {
            log(`[Size${dir.name}] Layerizing...`);
            layerize({
              in: `./obj/color/${dir.name}`,
              mono: wd,
              extra: wd,
              'extra-filter': `./override/mono/${dir.name}`,
              size: Number(dir.name),
              upm: upm,
              mirror: './obj/mirror.json',
              out: `./obj/colr/${dir.name}`,
            });
          } else {
            fs.mkdirSync(`./obj/colr/${dir.name}`, { recursive: true });
            fs.mkdirSync(`./override/mono/${dir.name}`, { recursive: true });
          }
          log(`[Size${dir.name}] Mirroring...`);
          mirror({
            dir: [wd],
            config: './obj/mirror.json',
          });
          log(`[Size${dir.name}] Generating...`);
          argv.format.forEach((format) => {
            execSync(
              `python ./generate.py --in=./obj/composed/${dir.name} --override=./override/mono/${dir.name} --colr=./obj/colr/${dir.name} --icons=./obj/icons.json --out=./obj --name=FluentSystemIcons-Size${dir.name} --upm=${upm} --format=${format}`,
              { stdio: 'inherit' },
            );
            fs.copyFileSync(
              `./obj/FluentSystemIcons-Size${dir.name}.${format}`,
              `./assets/FluentSystemIcons-Size${dir.name}.${format}`,
            );
          });
        });
    },
  )
  .strict()
  .parseSync(hideBin(process.argv));
