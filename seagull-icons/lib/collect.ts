import assert from 'assert';
import fs from 'fs';
import path from 'path';
import { Argv } from 'yargs';
import { ensure, resolveAsset } from './utils.js';

const names = new Set<string>();
const resizable_names = new Set<string>();
const glyph_names = new Map<string, string>();
const mirror_glyphs = new Set<string>();

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
  src_set: string,
  dest_subdir: string,
  locale: string,
  depth: number,
  mirror: boolean,
  get_dest: (name: string, subdir: string, fname: string, locale: string) => [string, string[]],
) {
  fs.readdirSync(src_root).forEach((fname) => {
    const src_item = path.join(src_root, fname);
    const src_stat = fs.statSync(src_item);
    let subdir = dest_subdir;
    let locl = locale;

    if (src_stat.isDirectory()) {
      const dname = path.basename(src_item);
      if (depth == 0) {
        src_set = dname;
        const meta_name = path.join(src_item, 'metadata.json');
        if (fs.existsSync(meta_name)) {
          mirror =
            (JSON.parse(fs.readFileSync(meta_name).toString()) as MetaData).directionType ===
            'mirror';
        }
      } else if (dname === 'PDF') {
        return;
      } else if (dname.match(/^[a-z]{2}(-[A-Za-z][a-z]{3})?(-[A-Za-z]{2})?$/)) {
        // language tags
        let tag = dname
          .replace(/^gr\b/, 'el') // incorrect tag
          .replace(/^kk$/, 'kk-cyrl'); // explicitly tag as Cyrillic to avoid over-used with Latin script
        if (src_set.includes('Text Sort') && tag === 'sr') tag = 'sr-cyrl';
        assert(locale === '', `Multiple locale in path: ${src_item}`);
        locl = `.${tag}`;
      } else if (dname === 'LTR') {
        subdir = path.join(subdir, dname);
      } else if (dname === 'RTL') {
        subdir = path.join(subdir, dname);
      } else if (depth === 1 && dname !== 'SVG') {
        console.warn(`[WARN] Unrecognized subfolder: ${subdir}`);
        // subdir = path.join(subdir, dname);
        return;
      }
      collect(src_item, src_set, subdir, locl, depth + 1, mirror, get_dest);
    } else if (fname.startsWith('.') || fname.startsWith('_')) {
      return;
    } else if (fname.endsWith('.svg')) {
      const [name, dest_files] = get_dest(src_set, subdir, fname, locl);
      for (const dest_file of dest_files) {
        ensure(path.dirname(dest_file));
        // prefer Bidi icons in explicit "LTR"/"RTL" subfolders
        if (fs.existsSync(dest_file) && !src_item.match(/[\/\\](?:LTR|RTL)[\/\\]/)) {
          continue;
        }

        if (mirror) {
          mirror_glyphs.add(name);
        }
        fs.copyFileSync(src_item, dest_file);
      }
    }
  });
}

function merge(dir: string) {
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

export function parseCollect(yargs: Argv) {
  return yargs.string('in').string('root');
}

export default function fun(
  argv: Partial<ReturnType<typeof parseCollect> extends Argv<infer P> ? P : never> = {},
) {
  argv.in ??= './upstream/assets';
  argv.root ??= './obj';
  names.clear();
  glyph_names.clear();
  resizable_names.clear();
  mirror_glyphs.clear();

  const MONO_DIR = path.join(argv.root, 'mono');
  const COLOR_DIR = path.join(argv.root, 'color');
  const CSV = path.join(argv.root, '../collect.csv');
  const ICONS_JSON = path.join(argv.root, 'icons.json');
  const MIRROR_JSON = path.join(argv.root, 'mirror.json');
  const ICON_CS = path.join(argv.root, 'Icon.cs');

  if (fs.existsSync(MONO_DIR)) {
    fs.rmSync(MONO_DIR, { recursive: true });
  }
  if (fs.existsSync(COLOR_DIR)) {
    fs.rmSync(COLOR_DIR, { recursive: true });
  }

  collect(argv.in, null!, '', '', 0, false, (src_set, subdir, name, locale) => {
    const spec = resolveAsset(src_set, name);
    if (spec === null) {
      return ['', []];
    } else if (spec.direction) {
      assert(subdir === '' || subdir === spec.direction, `Conflict subdir in ${name}`);
      subdir = path.join(spec.direction, subdir);
    }
    names.add(spec.name_enum);
    glyph_names.set(spec.name_enum, spec.name_glyph);

    const dest_files = [
      path.join(
        spec.variant === 'color' ? COLOR_DIR : MONO_DIR,
        spec.size.toString(),
        subdir,
        `${spec.name_glyph}.${spec.variant}${locale}.svg`,
      ),
    ];

    // resizable for Seagull
    if (
      (spec.size === 20 && ['regular', 'filled'].includes(spec.variant)) ||
      (spec.size === 32 && spec.variant == 'light')
    ) {
      resizable_names.add(spec.name_enum);
      dest_files.push(
        path.join(
          MONO_DIR,
          'resizable',
          subdir,
          `${spec.name_glyph}.${spec.variant}${locale}.svg`,
        ),
      );
    } else if (spec.size === 20 && spec.variant === 'color') {
      resizable_names.add(spec.name_enum);
    }

    // overriding size16 icons
    if (
      spec.name_glyph.match(/^(?:planet$|presence_|spatula_spoon$|text_whole_word$)/) &&
      spec.size === 16
    ) {
      dest_files.push(
        path.join(
          spec.variant === 'color' ? COLOR_DIR : MONO_DIR,
          'override',
          subdir,
          `${spec.name_glyph}.${spec.variant}${locale}.svg`,
        ),
      );
    }

    return [spec.name_glyph, dest_files];
  });

  fs.readdirSync(MONO_DIR).forEach((size) => {
    merge(path.join(MONO_DIR, size));
  });
  fs.readdirSync(COLOR_DIR).forEach((size) => {
    merge(path.join(COLOR_DIR, size));
  });

  // patch for `text_align_right`
  if (
    [...fs.readdirSync(MONO_DIR)].every((size) => {
      const d = path.join(MONO_DIR, size, 'RTL');
      return !fs.existsSync(path.join(d, 'text_align_right.regular.svg'));
    }) // guard
  ) {
    mirror_glyphs.add('text_align_right');
  }

  // patch for `text_edit_style`
  [...fs.readdirSync(MONO_DIR)].forEach((size) => {
    const d = path.join(MONO_DIR, size);
    if (
      fs.existsSync(path.join(d, 'text_edit_style.regular.en.svg')) &&
      !fs.existsSync(path.join(d, 'text_edit_style.regular.svg'))
    )
      fs.cpSync(
        path.join(d, 'text_edit_style.regular.en.svg'),
        path.join(d, 'text_edit_style.regular.svg'),
      );
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

  const enum_list = fs
    .readFileSync(CSV, 'utf-8')
    .split(/\r?\n/)
    .filter((l) => l.length > 0);

  [...names].sort().forEach((e) => {
    if (!enum_list.includes(e)) {
      enum_list.push(e);
      console.log(`[NEW] ${e}`);
    }
  });

  const icons = Object.fromEntries(enum_list.map((e) => [glyph_names.get(e), nextCodepoint()]));
  let icon_cs_lines = [
    `using FluentIcons.Common.Internals;

namespace FluentIcons.Common;

public enum Icon : int
{`,
  ];
  enum_list.forEach((e, i) => {
    if (glyph_names.has(e)) {
      if (!resizable_names.has(e)) {
        icon_cs_lines.push(`    [NonResizable]`);
      }
      icon_cs_lines.push(`    ${e} = ${i},`);
    } else {
      console.warn(`[NOT_FOUND] ${e}`);
    }
  });
  icon_cs_lines.push(`}`);

  fs.writeFileSync(CSV, enum_list.join('\n'));
  fs.writeFileSync(ICONS_JSON, JSON.stringify(icons));
  fs.writeFileSync(MIRROR_JSON, JSON.stringify([...mirror_glyphs].sort()));
  fs.writeFileSync(ICON_CS, icon_cs_lines.join('\n'));
}
