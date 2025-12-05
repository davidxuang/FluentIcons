import fs from 'fs';
import path from 'path';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import { parse } from '@std/toml';
import { Parser } from 'xml2js';
import paper from 'paper';
import { Doc, Renderable } from './types.js';
import { ensure, getPathData } from './utils.js';

const argv = yargs()
  .string('config')
  .string('in')
  .string('in-override')
  .array('in-override')
  .string('out')
  .string('out-override')
  .array('out-override')
  .strict()
  .parseSync(hideBin(process.argv));

const CONFIG = argv.config;
const SRC_DIR = argv.in;
const SRC_OVERRIDES = argv['in-override'];
const DEST_DIR = argv.out;
const DEST_OVERRIDES = argv['out-override'];

paper.setup([32, 32]);
const align = new paper.Point(-2, -2);

const parser = new Parser({
  preserveChildrenOrder: true,
  explicitChildren: true,
  explicitArray: true,
});

type Translate = [number, number];
type Style = 'regular' | 'filled' | 'light';
type BaseMeta = {
  '0': string; // category
  u?: Translate; // base translate for 16px
  v?: Translate; // badge translate for 16px
  ul?: Translate; // base translate for 28px
  vl?: Translate; // badge translate for 28px
};
type BadgeMeta = {
  window?: Translate;
  mask: string | { [cat: string]: string };
  badge: string | { [cat: string]: string };
};
type BadgeTemplate = BadgeMeta & {
  [style in Style]?: BadgeMeta;
};
type BadgeComposeV = BadgeMeta & {
  // icon:root@modifier#/override
  icons: string[];
  icons_rtl?: string[];
};
type BadgeCompose = BadgeComposeV & {
  inherits?: string;
  mirrors?: string;
} & {
  [style in Style]?: BadgeComposeV;
};

// read configurations
const data = parse(fs.readFileSync(CONFIG).toString()) as {
  translate: {
    [icon: string]: Translate;
  };
  redirect: { [icon: string]: string | { [cat: string]: string } };
  base: {
    [icon: string]: string | { [badge: string]: string | BaseMeta };
  };
  template: { [badge: string]: BadgeTemplate };
  // name#variant
  compose: { [badge: string]: BadgeCompose };
  rtl: {
    [icon: string]: {
      mirror_badge?: boolean;
      mirror_category?: boolean;
    };
  };
};

// reset artifacts
if (fs.existsSync(DEST_DIR)) {
  fs.rmSync(DEST_DIR, { recursive: true });
}

function copy(src: string, dest: string) {
  fs.readdirSync(src).forEach((f) => {
    const src_item = path.join(src, f);
    const dest_item = path.join(dest, f);
    if (path.extname(f) != '.svg' || fs.existsSync(dest_item)) {
      return;
    }

    parser.parseString(fs.readFileSync(src_item), (err, doc: Doc) => {
      if (err) {
        throw src_item;
      }

      const item = new paper.CompoundPath(
        doc.svg.$$.map((e) => getPathData(e as Renderable)).join()
      );
      if (doc.svg.$.height === '16') {
        fs.copyFileSync(src_item, dest_item);
      } else if (doc.svg.$.height === '20') {
        item.translate(align);
        fs.writeFileSync(
          dest_item,
          `<svg width="16" height="16" viewBox="0 0 16 16" xmlns="http://www.w3.org/2000/svg">\n  <path d="${item.pathData}" fill="#212121" />\n</svg>`
        );
      } else if (doc.svg.$.height === '32') {
        item.translate(align);
        fs.writeFileSync(
          dest_item,
          `<svg width="28" height="28" viewBox="0 0 28 28" xmlns="http://www.w3.org/2000/svg">\n  <path d="${item.pathData}" fill="#212121" />\n</svg>`
        );
      } else {
        throw src_item;
      }
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
  modifier?: string;
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
      modifier: matches[3],
      style: style,
      src_name: matches[1], // needs further resolve
      dest_name: undefined,
    };

    if (matches[4]) {
      meta.dest_name = `${matches[4]}-${style}.svg`;
    } else {
      meta.dest_name = `${[meta.base_alias, badge_name, meta.modifier]
        .filter((x) => x)
        .join('_')}-${style}.svg`;
    }

    return meta;
  };
})();

const resolveSrc = (meta: ComposeMeta, base_meta: BaseMeta) => {
  const redirect = data.redirect[meta.src_name];
  if (redirect) {
    if (typeof redirect === 'string') {
      meta.src_name = `${redirect}-${meta.style}.svg`;
    } else {
      meta.src_name = `${redirect[base_meta['0']] ?? redirect['_'] ?? meta.src_name}-${
        meta.style
      }.svg`;
    }
  } else {
    meta.src_name = `${meta.src_name}-${meta.style}.svg`;
  }
};

const compose = (() => {
  function getMask(badge_meta: BadgeMeta, cat: string) {
    if (cat === undefined || cat === null) {
      return new paper.Path();
    }

    const m = badge_meta.mask;
    const window = badge_meta.window;

    if (m === undefined) {
      throw [badge_meta, cat];
    }

    if (typeof m === 'string' || m[cat] === undefined) {
      const item = new paper.CompoundPath(m['_'] ?? m);
      if (window) {
        if (cat === '_') {
          throw [badge_meta, cat]; // missing entry in `base`
        }
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

    const b = badge_meta.badge;
    const window = badge_meta.window;

    if (typeof b === 'string' || b[cat] === undefined) {
      const item = new paper.CompoundPath(b['_'] ?? b);
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
        doc.svg.$$.map((elem) => getPathData(elem as Renderable)).join()
      );
      if (meta.style !== 'light') {
        if (doc.svg.$.width !== '16') {
          item.translate(align);
        }
      } else {
        if (doc.svg.$.width !== '28') {
          item.translate(align);
        }
      }

      if (base_meta.u) {
        item.translate(base_meta.u);
      }

      const mask = getMask(badge_meta, base_meta['0']);
      const badge = getBadge(badge_meta, base_meta['0']);

      if (base_meta.v) {
        mask.translate(base_meta.v);
        badge.translate(base_meta.v);
      }

      let path_data: string = undefined;

      if (!mask.pathData && !badge.pathData && meta.badge_name) {
        console.warn(`[NOP] ${meta.src_name} ==> ${meta.dest_name}`);
      }

      if (meta.modifier === 'off') {
        item = item.subtract(mask).exclude(badge).subtract(off_subtract).unite(off_unite);
        path_data = item.pathData;
      } else if (mask.pathData) {
        item = item.subtract(mask);
        path_data = item.pathData + badge.pathData;
      } else {
        // use XOR if mask is empty
        item = item.exclude(badge);
        path_data = item.pathData;
      }

      let size = meta.style !== 'light' ? 16 : 28;
      fs.writeFileSync(
        path.join(dest_dir, meta.dest_name),
        `<svg width="${size}" height="${size}" viewBox="0 0 ${size} ${size}" xmlns="http://www.w3.org/2000/svg">\n  <path d="${path_data}" fill="#212121" />\n</svg>`
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
    return { '0': '_' };
  }

  if (typeof entry === 'string') {
    return { '0': entry };
  } else {
    let meta = entry[`@${variant}`] ?? entry[badge] ?? entry['_'];
    if (typeof meta === 'string') {
      return { '0': meta };
    } else {
      return meta;
    }
  }
}

const sources = [...SRC_OVERRIDES, SRC_DIR];
Object.entries(data.compose).forEach(([badge, badge_comp]) => {
  const [badge_name, badge_style] = badge.split('#');
  function f(descriptor: string, style: string, badge_meta: BadgeMeta) {
    const meta = resolve(descriptor, style, badge_name, badge_style);
    let base_meta = getBaseMeta([meta.base, meta.base_alias], meta.modifier, meta.badge_name);
    if (style === 'light') {
      base_meta = { '0': base_meta['0'], u: base_meta.ul, v: base_meta.vl };
    }
    resolveSrc(meta, base_meta);
    compose(meta, base_meta, badge_meta, sources, DEST_DIR);
  }

  badge_comp.icons?.forEach((d) => {
    ['regular', 'filled'].forEach((s: Style) =>
      f(d, s, { ...data.template[badge_comp.inherits], ...badge_comp })
    );
  });
  ['regular', 'filled'].forEach((s: Style) => {
    badge_comp[s]?.icons?.forEach((d) =>
      f(d, s, {
        ...data.template[badge_comp.inherits],
        ...data.template[badge_comp.inherits]?.[s],
        ...badge_comp,
        ...badge_comp[s],
      })
    );
  });
  badge_comp.light?.icons?.forEach((d) =>
    f(d, 'light', {
      ...data.template[badge_comp.inherits]?.light,
      ...badge_comp.light,
    })
  );
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

Object.entries(data.compose).forEach(([badge, badge_comp]) => {
  const [badge_name, badge_style] = badge.split('#');
  function f(descriptor: string, style: string, badge_meta: BadgeMeta) {
    const meta = resolve(descriptor, style, badge_name, badge_style);
    let base_meta = getBaseMeta([meta.base, meta.base_alias], meta.modifier, meta.badge_name);
    if (style === 'light') {
      base_meta = { '0': base_meta['0'], u: base_meta.ul, v: base_meta.vl };
    }
    if (data.rtl[meta.base]?.mirror_category) {
      base_meta['0'] = mirrorCategoty(base_meta['0']);
    }
    resolveSrc(meta, base_meta);
    compose(meta, base_meta, badge_meta, rtl_sources, path.join(DEST_DIR, 'RTL'));
  }

  const b = data.rtl[badge]?.mirror_badge ? badge_comp.mirrors : badge;
  badge_comp.icons_rtl?.forEach((d) => {
    ['regular', 'filled'].forEach((s: Style) =>
      f(d, s, {
        ...data.template[data.compose[b].inherits],
        ...data.compose[b],
      })
    );
  });
  ['regular', 'filled'].forEach((s: Style) => {
    badge_comp[s]?.icons_rtl?.forEach((d) =>
      f(d, s, {
        ...data.template[data.compose[b].inherits],
        ...data.template[data.compose[b].inherits]?.[s],
        ...data.compose[b],
        ...data.compose[b][s],
      })
    );
  });
  badge_comp.light?.icons_rtl?.forEach((d) =>
    f(d, 'light', {
      ...data.template[data.compose[b].inherits]?.light,
      ...data.compose[b].light,
    })
  );
});

// copy not modified
copy(SRC_DIR, DEST_DIR);
copy(path.join(SRC_DIR, 'RTL'), path.join(DEST_DIR, 'RTL'));
