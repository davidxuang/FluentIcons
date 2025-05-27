import fs from 'fs';
import path from 'path';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import { ensure, resolveName } from './utils.js';

const argv = yargs()
  .string('source')
  .string('root')
  .strict()
  .parseSync(hideBin(process.argv));
const MONO_DIR = path.join(argv.root, 'mono');
const COLOR_DIR = path.join(argv.root, 'color');
const ICONS = path.join(argv.root, 'icons.json');
const ICONS_RESIZABLE = path.join(argv.root, 'icons-resizable.json');
const MIRROR = path.join(argv.root, 'mirror.json');

const names = new Set<string>();
const resizable_names = new Set<string>();
const mirror_names = new Set<string>();

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
  get_dest: (subdir: string, name: string) => [string, string[]]
) {
  fs.readdirSync(src_root).forEach((fname) => {
    const src_item = path.join(src_root, fname);
    const src_stat = fs.statSync(src_item);
    let subdir = dest_subdir;

    if (src_stat.isDirectory()) {
      const src_name = path.basename(src_item);
      if (depth == 0) {
        const meta_name = path.join(src_item, 'metadata.json');
        if (fs.existsSync(meta_name)) {
          mirror =
            (JSON.parse(fs.readFileSync(meta_name).toString()) as MetaData)
              .directionType === 'mirror';
        }
        const words = src_name.split(' ');
        if (words.indexOf('LTR') >= 0) {
          subdir = path.join(subdir, 'LTR');
        } else if (words.indexOf('RTL') >= 0) {
          subdir = path.join(subdir, 'RTL');
        }
      } else if (
        src_name.match(/^[a-z]{2}(-[A-Za-z][a-z]{3})?(-[A-Za-z]{2})?$/) ||
        src_name === 'PDF'
      ) {
        return; // skip language-specific
      } else if (depth === 1 && src_name !== 'SVG') {
        subdir = path.join(subdir, src_name);
      }
      collect(src_item, subdir, depth + 1, mirror, get_dest);
    } else if (fname.startsWith('.') || fname.startsWith('_')) {
      return;
    } else if (fname.endsWith('.svg')) {
      // unify 'ic_fluent_' prefix
      if (fname.startsWith('ic_') && !fname.startsWith('ic_fluent_')) {
        fname = fname.replace('ic_', 'ic_fluent_');
      }

      // remove Bidi suffixes
      if (fname.indexOf('_ltr_') >= 0) {
        fname = fname.replace('_ltr_', '_');
      } else if (fname.indexOf('_rtl_') >= 0) {
        fname = fname.replace('_rtl_', '_');
      }

      const [name, dest_files] = get_dest(subdir, fname);
      for (const dest_file of dest_files) {
        ensure(path.dirname(dest_file));
        // prefer Bidi icons in explicit "LTR"/"RTL" subfolders
        if (
          fs.existsSync(dest_file) &&
          src_item.match(/\s(?:LTR|RTL)|(?:LTR|RTL)\s/)
        ) {
          continue;
        }

        names.add(name);
        if (mirror) {
          mirror_names.add(name);
        }
        fs.copyFileSync(src_item, dest_file);
      }
    }
  });
}

function merge(dir: string) {
  if (fs.existsSync(path.join(dir, 'override'))) {
    fs.readdirSync(path.join(dir, 'override')).forEach((f) => {
      fs.cpSync(path.join(dir, 'override', f), path.join(dir, f), {
        force: true,
      });
    });
    fs.rmSync(path.join(dir, 'override'), { recursive: true });
  }

  if (fs.existsSync(path.join(dir, 'LTR'))) {
    fs.readdirSync(path.join(dir, 'LTR')).forEach((f) => {
      const dest = path.join(dir, f);
      if (!fs.existsSync(dest)) {
        fs.copyFileSync(path.join(dir, 'LTR', f), dest);
      }
    });
    fs.rmSync(path.join(dir, 'LTR'), { recursive: true });
  }
}

if (fs.existsSync(MONO_DIR)) {
  fs.rmSync(MONO_DIR, { recursive: true });
}
if (fs.existsSync(COLOR_DIR)) {
  fs.rmSync(COLOR_DIR, { recursive: true });
}

collect(argv.source, '', 0, false, (subdir, name) => {
  const spec = resolveName(name, true);
  if (spec === null) {
    return [null, []];
  }

  // unify rotate names
  spec.name = spec.name.replace(/(?<!rotate)_(90|270)$/, '_rotate_$1');

  const dest_files = [
    path.join(
      spec.variant === 'color' ? COLOR_DIR : MONO_DIR,
      spec.size.toString(),
      subdir,
      `${spec.name}-${spec.variant}.svg`
    ),
  ];

  // resizable for Seagull
  if (
    (spec.size === 20 && ['regular', 'filled'].includes(spec.variant)) ||
    (spec.size === 32 && spec.variant == 'light')
  ) {
    resizable_names.add(spec.name);
    dest_files.push(
      path.join(
        MONO_DIR,
        'resizable',
        subdir,
        `${spec.name}-${spec.variant}.svg`
      )
    );
  }

  // overriding size16 icons
  if (
    spec.name.match(/^(?:planet$|presence_|spatula_spoon$|text_whole_word$)/) &&
    spec.size === 16
  ) {
    dest_files.push(
      path.join(
        spec.variant === 'color' ? COLOR_DIR : MONO_DIR,
        'override',
        subdir,
        `${spec.name}-${spec.variant}.svg`
      )
    );
  }

  return [spec.name, dest_files];
});

fs.readdirSync(MONO_DIR).forEach((size) => {
  merge(path.join(MONO_DIR, size));
});
fs.readdirSync(COLOR_DIR).forEach((size) => {
  merge(path.join(COLOR_DIR, size));
});

// [regular, filled, light, color], RTL in PUA-B
const nextCodepoint = (() => {
  const advance = 4;
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

const icons = Object.fromEntries(
  [...names].sort().map((name) => [name, nextCodepoint()])
);
const icons_resizable = Object.fromEntries(
  [...resizable_names].sort().map((name) => [name, icons[name]])
);

fs.writeFileSync(ICONS, JSON.stringify(icons));
fs.writeFileSync(ICONS_RESIZABLE, JSON.stringify(icons_resizable));
fs.writeFileSync(MIRROR, JSON.stringify([...mirror_names].sort()));
