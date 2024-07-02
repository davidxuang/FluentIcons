import * as fs from 'fs';
import * as path from 'path';
import * as yargs from 'yargs';
import { ensure } from './utils';

const argv = yargs
  .string('source')
  .string('system')
  .string('seagull')
  .string('codepoints')
  .string('mirror')
  .strict()
  .parseSync();
const SRC_DIR = argv.source;
const SYSTEM_DIR = argv.system;
const SEAGULL_DIR = argv.seagull;
const CODEPOINTS = argv.codepoints;
const MIRROR = argv.mirror;

const mirror_set = new Set<string>();

type MetaData = {
  name: string;
  size: number[];
  style: string[];
  keyword: string;
  description: string;
  metaphor: string[];
  directionType?: 'mirror' | 'unique';
};

function collect(
  src_root: string,
  dest_subdir: string,
  depth: number,
  mirror: boolean,
  get_dest: (subdir: string, name: string) => string
) {
  fs.readdirSync(src_root).forEach((name) => {
    const src_item = path.join(src_root, name);
    const src_stat = fs.statSync(src_item);
    let dest_dir = dest_subdir;

    if (src_stat.isDirectory()) {
      const src_name = path.basename(src_item);
      if (depth === 0) {
        const meta_name = path.join(src_item, 'metadata.json');
        if (fs.existsSync(meta_name)) {
          mirror =
            (JSON.parse(fs.readFileSync(meta_name).toString()) as MetaData)
              .directionType === 'mirror';
        }
        const words = src_name.split(' ');
        if (words.indexOf('LTR') >= 0) {
          dest_dir = path.join(dest_dir, 'LTR');
        } else if (words.indexOf('RTL') >= 0) {
          dest_dir = path.join(dest_dir, 'RTL');
        }
      } else if (
        src_name.match(/^[a-z]{2}(-[A-Za-z][a-z]{3})?(-[A-Za-z]{2})?$/) ||
        src_name === 'PDF'
      ) {
        return; // skip language-specific
      } else if (depth === 1 && src_name !== 'SVG') {
        dest_dir = path.join(dest_dir, src_name);
      }
      collect(src_item, dest_dir, depth + 1, mirror, get_dest);
    } else if (name.startsWith('.') || name.startsWith('_')) {
    } else if (name.endsWith('svg')) {
      // apply 'ic_fluent_' prefix
      if (name.startsWith('ic_') && !name.startsWith('ic_fluent_')) {
        name = name.replace('ic_', 'ic_fluent_');
      }

      // remove Bidi suffixes
      if (name.indexOf('_ltr_') >= 0) {
        name = name.replace('_ltr_', '_');
      } else if (name.indexOf('_rtl_') >= 0) {
        name = name.replace('_rtl_', '_');
      }

      const dest_file = get_dest(dest_subdir, name);
      if (dest_file === undefined) {
        return;
      }
      const dest_dir = path.dirname(dest_file);
      ensure(dest_dir);
      // prefer Bidi icons in explicit "LTR"/"RTL" subfolders
      if (
        fs.existsSync(dest_file) &&
        src_item.match(/\s(?:LTR|RTL)|(?:LTR|RTL)\s/)
      ) {
        return;
      }

      if (mirror) {
        mirror_set.add(path.basename(dest_file));
      }
      fs.copyFileSync(src_item, dest_file);
    }
  });
}

function merge(dir: string) {
  if (!fs.existsSync(path.join(dir, 'LTR'))) {
    return;
  }
  fs.readdirSync(path.join(dir, 'LTR')).forEach((f) => {
    const dest = path.join(dir, f);
    if (!fs.existsSync(dest)) {
      fs.copyFileSync(path.join(dir, 'LTR', f), dest);
    }
  });
  fs.rmSync(path.join(dir, 'LTR'), { recursive: true });
}

if (fs.existsSync(SYSTEM_DIR)) {
  fs.rmSync(SYSTEM_DIR, { recursive: true });
}
if (fs.existsSync(SEAGULL_DIR)) {
  fs.rmSync(SEAGULL_DIR, { recursive: true });
}
if (fs.existsSync(CODEPOINTS)) {
  fs.rmSync(CODEPOINTS);
}
collect(SRC_DIR, '', 0, false, (subdir, name) => {
  let matches =
    name.match(/^ic_fluent_(.+)_20_(regular|filled)\.svg$/) ||
    name.match(/^ic_fluent_(.+)_32_(light)\.svg$/);
  if (matches) {
    // unify rotate names
    matches[1] = matches[1].replace(/(?<!rotate)_(90|270)$/, '_rotate_$1');

    return path.join(
      SYSTEM_DIR,
      subdir,
      `ic_fluent_${matches[1]}_${matches[2]}.svg`
    );
  }
  // size16 icons
  matches = name.match(
    /^ic_fluent_(?=presence_|spatula_spoon_|text_whole_word_)(.+)_16_(regular|filled)\.svg$/
  );
  if (matches) {
    return path.join(
      SEAGULL_DIR,
      subdir,
      `ic_fluent_${matches[1]}_${matches[2]}.svg`
    );
  }
  return undefined;
});
merge(SYSTEM_DIR);
merge(SEAGULL_DIR);

// [regular, filled, light, regular RTL, filled RTL, light RTL]
const nextCodepoint = (() => {
  const advance = 6;
  let pos = 0xf0000 - advance;
  return () => {
    pos += advance;
    // reserve BMP PUA for Segoe compatibility
    if (pos < 0x100000) {
      if (pos + advance - 1 > 0xffffd) {
        throw pos;
      }
    } else if (pos + advance - 1 > 0x10fffd) {
      throw pos;
    }
    return pos;
  };
})();

const codepoints = Object.fromEntries(
  fs
    .readdirSync(SYSTEM_DIR)
    .filter((fname) => fname.endsWith('.svg'))
    .map((fname) =>
      fname.replace(/ic_fluent_(.+)_(regular|filled|light).svg/, '$1')
    )
    .filter((name, index, array) => {
      if (name.match(/^\w+$/)) {
        return array.indexOf(name) === index; // unique
      } else {
        throw name;
      }
    })
    .sort()
    .map((name) => [name, nextCodepoint()])
);
fs.writeFileSync(CODEPOINTS, JSON.stringify(codepoints));
fs.writeFileSync(MIRROR, JSON.stringify([...mirror_set]));
