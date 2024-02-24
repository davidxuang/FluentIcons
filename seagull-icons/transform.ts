import * as fs from 'fs';
import * as path from 'path';
import * as yargs from 'yargs';
import { parse } from 'yaml';
import { Parser } from 'xml2js';
import * as paper from 'paper';
import { Doc, Visible } from './types';
import { ensure, getPathData } from './utils';

const argv = yargs
  .string('yaml')
  .string('source')
  .string('override-source')
  .array('override-source')
  .string('dest')
  .string('override-dest')
  .array('override-dest')
  .strict()
  .parseSync();

const YAML = argv.yaml;
const SRC_DIR = argv.source;
const SRC_OVERRIDES = argv['override-source'];
const DEST_DIR = argv.dest;
const DEST_OVERRIDES = argv['override-dest'];

const styles = ['regular', 'filled'];

paper.setup([20, 20]);
const align = new paper.Point(-2, -2);

const parser = new Parser({
  preserveChildrenOrder: true,
  explicitChildren: true,
  explicitArray: true,
});

type Translate = [number, number];
type BaseMeta = [
  string, // cat
  [number, number], // translate
  [number, number] // badge_translate
];
type BadgeAbstract = {
  window?: Translate;
  mask: string | { [cat: string]: string };
  badge: string | { [cat: string]: string };
};
type BadgeMeta = BadgeAbstract & {
  inherits?: string;
  // icon:root@variant#/override
  icons: string[];
};

// read configurations
const data = parse(fs.readFileSync(YAML).toString()) as {
  translate: {
    [icon: string]: Translate;
  };
  redirect: { [icon: string]: string };
  base: {
    [icon: string]: string | BaseMeta | { [badge: string]: string | BaseMeta };
  };
  abstract: { [badge: string]: BadgeAbstract };
  // name#variant
  compose: { [badge: string]: BadgeMeta };
  rtl: {
    base: {
      [icon: string]: {
        mirror_badge?: boolean;
        mirror_category?: boolean;
      };
    };
    compose: {
      [badge: string]: {
        mirror?: string;
        icons: string[];
      };
    };
  };
};

// reset artifacts
if (fs.existsSync(DEST_DIR)) {
  fs.rmSync(DEST_DIR, { recursive: true });
}

function copy(src: string, dest: string) {
  fs.readdirSync(src).forEach((f) => {
    if (path.extname(f) != '.svg' || fs.existsSync(path.join(dest, f))) {
      return;
    }

    parser.parseString(fs.readFileSync(path.join(src, f)), (err, doc: Doc) => {
      if (err) {
        throw path.join(src, f);
      }

      const item = new paper.CompoundPath(
        doc.svg.$$.map((e) => getPathData(e as Visible)).join()
      );
      item.translate(align);
      fs.writeFileSync(
        path.join(dest, f),
        `<svg width="16" height="16" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg">\n  <path d="${item.pathData}" fill="#212121" />\n</svg>`
      );
      item.remove();
    });
  });
}

// copy overrides
ensure(DEST_DIR);
ensure(path.join(DEST_DIR, 'RTL'));
DEST_OVERRIDES.forEach((dir) => {
  copy(dir, DEST_DIR);
  if (fs.existsSync(path.join(dir, 'RTL'))) {
    copy(path.join(dir, 'RTL'), path.join(DEST_DIR, 'RTL'));
  }
});

// compose icons
type ComposeMeta = {
  base: string;
  base_alias: string;
  badge_name: string;
  badge_style: string;
  variant?: string;
  style: string;
  src_name: string;
  dest_name: string;
};

const resolve = (() => {
  const re = /^(\w+)(?:\:(\w+))?(?:@(\w+))?(?:#\/(\w+))?$/;
  return (
    descriptor: string,
    style: string,
    badge_name: string,
    badge_style: string
  ): ComposeMeta => {
    const matches = descriptor.match(re);
    if (!matches) {
      throw descriptor;
    }

    const meta: ComposeMeta = {
      base: matches[1] ?? matches[4],
      base_alias: matches[2] ?? matches[1] ?? matches[4],
      badge_name: badge_name,
      badge_style: badge_style,
      variant: matches[3],
      style: style,
      src_name: `ic_fluent_${
        data.redirect[matches[1]] ?? matches[1]
      }_${style}.svg`,
      dest_name: undefined,
    };

    if (matches[4]) {
      meta.dest_name = `ic_fluent_${matches[4]}_${style}.svg`;
    } else {
      meta.dest_name = `ic_fluent_${[meta.base_alias, badge_name, meta.variant]
        .filter((x) => x)
        .join('_')}_${style}.svg`;
    }

    return meta;
  };
})();

const compose = (() => {
  function getMask(badge_meta: BadgeMeta, cat: string) {
    if (cat === undefined || cat === null) {
      return new paper.Path();
    }

    const m = badge_meta.mask ?? data.abstract[badge_meta.inherits].mask;
    const window =
      badge_meta.window ?? data.abstract[badge_meta.inherits]?.window;

    if (m === undefined) {
      throw [badge_meta, cat];
    }

    if (typeof m === 'string' || m[cat] === undefined) {
      const item = new paper.CompoundPath(m['#'] ?? m);
      if (window) {
        if (cat.startsWith('1') || cat.startsWith('2')) {
          item.translate([window[0], 0]);
        }
        if (cat.startsWith('2') || cat.startsWith('3')) {
          item.translate([0, window[1]]);
        }
      }
      return item;
    } else {
      return new paper.CompoundPath(m[cat]);
    }
  }

  function getBadge(badge_meta: BadgeMeta, cat: string) {
    if (cat === undefined || cat === null) {
      return new paper.Path();
    }

    const b = badge_meta.badge ?? data.abstract[badge_meta.inherits].badge;
    const window =
      badge_meta.window ?? data.abstract[badge_meta.inherits]?.window;

    if (typeof b === 'string' || b[cat] === undefined) {
      const item = new paper.CompoundPath(b['#'] ?? b);
      if (window) {
        if (cat.startsWith('1') || cat.startsWith('2')) {
          item.translate([window[0], 0]);
        }
        if (cat.startsWith('2') || cat.startsWith('3')) {
          item.translate([0, window[1]]);
        }
      }
      return item;
    } else {
      return new paper.CompoundPath(b[cat]);
    }
  }

  return function (
    meta: ComposeMeta,
    base_meta: BaseMeta,
    badge_meta: BadgeMeta,
    src_dir: string[],
    dest_dir: string
  ) {
    const off_subtract = new paper.Path('M0,0h2.121320343559643l16,16H16L0,0Z');
    const off_unite = new paper.Path(
      'M0.8536,0.1465c-0.1953,-0.1953 -0.5119,-0.1953 -0.7072,0c-0.1952,0.1952 -0.1952,0.5118 0,0.707l15,15.0001c0.1953,0.1952 0.5119,0.1952 0.7072,0c0.1952,-0.1953 0.1952,-0.5119 0,-0.7072l-15,-15z'
    );

    const src_file = src_dir
      .map((d) => path.join(d, meta.src_name))
      .find((f) => fs.existsSync(f));

    if (src_file === undefined) {
      console.warn(`[NOT_FOUND] ${meta.src_name} ==> ${meta.dest_name}`);
      return;
    }

    parser.parseString(fs.readFileSync(src_file), (err, doc: Doc) => {
      if (err) {
        throw src_file;
      }

      let item: paper.PathItem = new paper.CompoundPath(
        doc.svg.$$.map((elem) => getPathData(elem as Visible)).join()
      );
      item.translate(align);

      if (base_meta[1]) {
        item.translate(base_meta[1]);
      }

      const mask = getMask(badge_meta, base_meta[0]);
      const badge = getBadge(badge_meta, base_meta[0]);

      if (base_meta[2]) {
        mask.translate(base_meta[2]);
        badge.translate(base_meta[2]);
      }

      let path_data: string = undefined;

      if (!mask.pathData && !badge.pathData && meta.badge_name) {
        console.warn(`[NOP] ${meta.src_name} ==> ${meta.dest_name}`);
      }

      if (meta.variant === 'off') {
        item = item
          .subtract(mask)
          .exclude(badge)
          .subtract(off_subtract)
          .unite(off_unite);
        path_data = item.pathData;
      } else if (mask.pathData) {
        item = item.subtract(mask);
        path_data = item.pathData + badge.pathData;
      } else {
        // use XOR if mask is empty
        item = item.exclude(badge);
        path_data = item.pathData;
      }

      fs.writeFileSync(
        path.join(dest_dir, meta.dest_name),
        `<svg width="16" height="16" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg">\n  <path d="${path_data}" fill="#212121" />\n</svg>`
      );

      item.remove();
      mask.remove();
      badge.remove();
    });
  };
})();

function getBaseMeta(base: string[], variant: string, badge: string): BaseMeta {
  let entry = base.map((b) => data.base[b]).find((e) => e);
  if (entry === undefined) {
    return ['#', undefined, undefined];
  }

  if (typeof entry === 'object' && !(entry instanceof Array)) {
    entry = entry[`@${variant}`] ?? entry[badge] ?? entry['#'];
  }

  if (typeof entry === 'string') {
    return [entry, undefined, undefined];
  } else {
    return entry as BaseMeta;
  }
}

const sources = [...SRC_OVERRIDES, SRC_DIR];
Object.entries(data.compose).forEach(([badge, badge_meta]) => {
  const [badge_name, badge_style] = badge.split('#');

  badge_meta.icons?.forEach((descriptor) => {
    function f(style: string) {
      const meta = resolve(descriptor, style, badge_name, badge_style);
      const base_meta = getBaseMeta(
        [meta.base, meta.base_alias],
        meta.variant,
        meta.badge_name
      );
      compose(meta, base_meta, badge_meta, sources, DEST_DIR);
    }

    if (styles.includes(badge_style)) {
      f(badge_style);
    } else {
      styles.forEach(f);
    }
  });
});

const rtl_sources = [...sources.map((d) => path.join(d, 'RTL')), ...sources];
fs.mkdirSync(path.join(DEST_DIR, 'RTL'), { recursive: true });
function mirrorCategoty(cat: string) {
  if (!cat) {
    return cat;
  }
  const c = parseInt(cat.slice(0, 1));
  if (c < 1 || c > 4) {
    return cat;
  }
  return `${5 - c}${cat.slice(1)}`;
}

Object.entries(data.rtl.compose).forEach(([badge_rtl, badge_rtl_meta]) => {
  const [badge_name, badge_style] = badge_rtl.split('#');

  badge_rtl_meta.icons?.forEach((descriptor) => {
    function f(style: string) {
      const meta = resolve(descriptor, style, badge_name, badge_style);
      const base_meta = getBaseMeta(
        [meta.base, meta.base_alias],
        meta.variant,
        meta.badge_name
      );
      if (data.rtl.base[meta.base]?.mirror_category) {
        base_meta[0] = mirrorCategoty(base_meta[0]);
      }
      const badge_meta = data.rtl.base[meta.base]?.mirror_badge
        ? data.compose[badge_rtl_meta.mirror ?? badge_rtl]
        : data.compose[badge_rtl];
      compose(
        meta,
        base_meta,
        badge_meta,
        rtl_sources,
        path.join(DEST_DIR, 'RTL')
      );
    }

    if (styles.includes(badge_style)) {
      f(badge_style);
    } else {
      styles.forEach(f);
    }
  });
});

// copy not modified
copy(SRC_DIR, DEST_DIR);
copy(path.join(SRC_DIR, 'RTL'), path.join(DEST_DIR, 'RTL'));
