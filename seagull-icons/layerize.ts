import assert from 'assert';
import fs from 'fs';
import path from 'path';
import { BiMap } from 'mnemonist';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import Color from 'colorjs.io';
import { Parser } from 'xml2js';
import * as xmlbuilder from 'xmlbuilder';
import paper from 'paper';
import {
  approximateShape,
  divideTransform,
  ensure,
  getPath,
  getPathData,
  resolveName,
} from './utils.js';
import { PaperOffset } from 'paperjs-offset';
import { parse } from '@std/toml';
import './ext';
import {
  Doc,
  FeColorMatrix,
  LinearGradient,
  Path,
  RadialGradient,
  Renderable,
} from './types.js';

const argv = yargs()
  .string('in')
  .string('override')
  .string('mono')
  .string('extra')
  .string('extra-filter')
  .string('out')
  .number('size')
  .number('shrink')
  .number('upm') // units per em
  .string('config')
  .string('mirror')
  .strict()
  .parseSync(hideBin(process.argv));

paper.setup([64, 64]);

const parser = new Parser({
  preserveChildrenOrder: true,
  explicitChildren: true,
  explicitArray: true,
});

type ColorGlyphLayer = { name: string; color: string; color_solid: string };
type ColorGlyph = { name: string; layers: ColorGlyphLayer[] };
type DropShadow = {
  dx: number;
  dy: number;
  blur: number;
  color: Color;
  opacity: number;
};

type SelectShape = {
  'select-shape': [[number, number], [number, number]];
};
type SelectGradient = {
  'select-gradient': string[];
  'gradient-type'?: 'shadow';
};
type SelectPathData = { 'select-path': string | string[] };

type Transform = {
  scale?: number | [number, number];
  translate: [number, number];
} & (SelectShape | SelectGradient | (SelectShape & SelectGradient));

type Substitution = { subst: string } & SelectPathData;

type Mutation = Transform | Substitution;
type MutationGroup = { 'use-group': string; [override: string]: any };

type Meta = {
  groups: { [group: string]: Mutation };
  icons: { [icon: string]: string | (Mutation | MutationGroup)[] };
};

const meta =
  argv.config !== undefined
    ? (parse(fs.readFileSync(argv.config).toString()) as Meta)
    : undefined;

// integral on $1/2 * (1 - erf(x / sqrt(2)))$
const blur_layers = new Map<number, number>([
  [2, 0.041632],
  [1.5, 0.108017],
  [1, 0.228962],
  [0.5, 0.402291],
  [0, 0.597709],
  [-0.5, 0.771038],
  [-1, 0.891983],
  [-1.5, 0.958368],
  [-2, 1], // truncated
]);
function getShadow(elems: Renderable[], shadow: DropShadow) {
  assert(!elems.some((e) => e.$.opacity ?? '1' !== '1'));
  assert(!elems.some((e) => e.$['fill-opacity'] ?? '1' !== '1'));
  let paths = elems.map((e) => getPathData(e));
  let outline = new paper.CompoundPath(paths.at(0));
  paths.slice(1).forEach((path) => {
    outline = outline.unite(new paper.CompoundPath(path)) as paper.CompoundPath;
  });
  const circles = outline.children
    .flatMap((child) => {
      if (child instanceof paper.Path) {
        return [child];
      } else {
        return child.children as paper.Path[];
      }
    })
    .map((child) => {
      const c = approximateShape(child);
      assert(c);
      return c;
    });

  // stacking blurring layers
  let current = 0;
  return [...blur_layers.entries()]
    .map(([f_offset, f_opacity]) => {
      const offset = shadow.blur * f_offset;
      const target = shadow.opacity * f_opacity;
      const opacity = (target - current) / (1 - current);
      current = target;
      let path_data: string;
      if (offset === 0) {
        path_data = outline.pathData;
      } else {
        path_data = circles
          .map((circle) => {
            const r = circle.r + offset;
            return r > 0 ? new paper.Path.Circle([circle.cx, circle.cy], r).pathData : '';
          })
          .join('');
      }
      if (shadow.dx !== 0 || shadow.dy !== 0) {
        const p = new paper.CompoundPath(path_data);
        p.translate([shadow.dx, shadow.dy]);
        path_data = p.pathData;
      }
      // skip if the shadow is completely covered by the shape
      const revealed = new paper.CompoundPath(path_data).subtract(
        new paper.CompoundPath(outline.pathData)
      );
      if ((revealed as paper.Path | paper.CompoundPath).isEmpty()) {
        path_data = '';
      }
      const fill = new Color(shadow.color);
      fill.alpha *= opacity;
      const layer: Path = {
        '#name': 'path',
        $: {
          d: path_data,
          fill: fill
            .toString({
              format: 'hex',
              alpha: true,
              collapse: false,
            })
            .padEnd(9, 'f'),
        },
      };
      return layer;
    })
    .filter((layer) => layer.$.d !== '');
}

function getDef(s: string) {
  const matches = s.match(/url\(#(.+)\)/);
  return matches ? matches[1] : undefined;
}

function selectGradient(gradient: LinearGradient | RadialGradient, transform: Transform) {
  if (!('select-gradient' in transform)) return false;

  const c = gradient.$$.some((stop) =>
    transform['select-gradient'].includes(stop.$['stop-color'] ?? 'black')
  );
  switch (transform['gradient-type']) {
    case 'shadow':
      return c && gradient.$$.some((stop) => stop.$['stop-opacity'] === '0');
    default:
      return c;
  }
}

function transformGradient(gradient: LinearGradient | RadialGradient, transform: Transform) {
  if ('scale' in transform) {
    gradient.$.gradientTransform = `translate(${transform.translate}) scale(${
      transform.scale
    }) ${gradient.$.gradientTransform ?? ''}`;
  } else {
    gradient.$.gradientTransform = `translate(${transform.translate}) ${
      gradient.$.gradientTransform ?? ''
    }`;
  }
  return gradient;
}

const glyphs = new BiMap<string, string>(); // layer_name -> path_data
const color_glyphs: ColorGlyph[] = [];
const output_size = argv.size - (argv.shrink ?? 0) * 2;

fs.readdirSync(argv.mono)
  .filter((f) => f.endsWith('.svg'))
  .forEach((f) => {
    const file = path.join(argv.mono, f);
    parser.parseString(fs.readFileSync(file), (err, doc: Doc) => {
      if (err) {
        throw file;
      }

      glyphs.set(
        path.basename(f, '.svg'),
        doc.svg.$$.filter(
          (elem) =>
            elem['#name'] === 'circle' ||
            elem['#name'] === 'ellipse' ||
            elem['#name'] === 'line' ||
            elem['#name'] === 'path' ||
            elem['#name'] === 'polygon' ||
            elem['#name'] === 'polyline' ||
            elem['#name'] === 'rect' ||
            elem['#name'] === 'g'
        )
          .map((elem) => getPathData(elem))
          .join('')
      );
    });
  });

[
  ...fs.readdirSync(argv.in).map((f) => {
    const spec = resolveName(f);
    const s = [`${spec.name}-filled.svg`, `${spec.name}-regular.svg`]
      .map((x) => path.join(argv.mono, x))
      .find((x) => fs.existsSync(x));
    if (s) {
      fs.cpSync(s, path.join(argv.mono, f), { force: true });
    } else {
      console.warn(`[NOT_FOUND] ${spec.name}-{filled|regular}.svg => ${f}`);
      fs.writeFileSync(
        path.join(argv.mono, f),
        `<svg width="1" height="1" viewBox="0 0 1 1" xmlns="http://www.w3.org/2000/svg">\n  <path d="M0 0Z" fill="#212121" />\n</svg>`
      );
    }

    if (argv.override && fs.existsSync(path.join(argv.override, f))) {
      return path.join(argv.override, f);
    }
    return path.join(argv.in, f);
  }),
  ...fs.readdirSync(argv.extraFilter).map((f) => path.join(argv.extra, f)),
].forEach((file) => {
  const icon_name = resolveName(path.basename(file)).name;
  let mutations = meta?.icons?.[icon_name];
  if (typeof mutations === 'string') {
    mutations = [meta.groups[mutations]];
  } else {
    mutations = mutations?.map((tf) => {
      if ('use-group' in tf) {
        const props = Object.entries(tf);
        tf = structuredClone(meta.groups[tf['use-group']]);
        // override
        props.forEach(([k, v]) => {
          if (k in tf) {
            tf[k] = v;
          }
        });
      }
      return tf;
    });
  }

  parser.parseString(fs.readFileSync(file), (err, doc: Doc) => {
    if (err) {
      throw file;
    }

    const name = path.parse(file).name;
    const gradients = new Map<string, LinearGradient | RadialGradient>();
    const shadows = new Map<string, DropShadow>();
    const may_shrink = argv.shrink && Number(doc.svg.$.width) !== output_size;

    // scan for gradient colors, assuming they are all direct children of defs
    doc.svg.$$.filter((elem) => elem['#name'] === 'defs')
      .flatMap((defs) => defs.$$)
      .forEach((def) => {
        if (def['#name'] === 'linearGradient' || def['#name'] === 'radialGradient') {
          if (may_shrink) {
            def.$.gradientTransform = `translate(-${argv.shrink},-${argv.shrink}) ${
              def.$.gradientTransform ?? ''
            }`;
          }

          gradients.set(def.$.id, def);
          delete def.$.id; // remove id to help de-duplication in the next step
        } else if (def['#name'] === 'filter') {
          const offset = def.$$.single((fe) => fe['#name'] === 'feOffset');
          const blur = def.$$.single((fe) => fe['#name'] === 'feGaussianBlur');
          const matix = def.$$.single(
            (fe): fe is FeColorMatrix =>
              fe['#name'] === 'feColorMatrix' && fe.$.in === undefined
          );
          const matches = matix.$.values.match(
            /^0 0 0 0 ([\d.]+) 0 0 0 0 ([\d.]+) 0 0 0 0 ([\d.]+) 0 0 0 ([\d.]+) 0$/
          );
          shadows.set(def.$.id, {
            dx: Number(offset.$.dx ?? '0'),
            dy: Number(offset.$.dy ?? '0'),
            blur: Number(blur.$.stdDeviation),
            color: new Color('sRGB', [
              Number(matches[1]),
              Number(matches[2]),
              Number(matches[3]),
            ]),
            opacity: Number(matches[4]),
          });
        }
      });

    // scan for shapes, assuming they are all direct children of svg
    const layers = doc.svg.$$.flatMap((elem) => {
      switch (elem['#name']) {
        case 'circle':
        case 'ellipse':
        case 'line':
        case 'path':
        case 'polygon':
        case 'polyline':
        case 'rect':
          return [elem];
        case 'g':
          if (elem.$.filter) {
            const shadow = shadows.get(getDef(elem.$.filter));
            assert(elem.$.opacity === undefined || elem.$.opacity === '1');
            assert(elem.$['fill-opacity'] === undefined || elem.$['fill-opacity'] === '1');
            return [...getShadow(elem.$$, shadow), ...elem.$$];
          } else {
            return elem.$$;
          }
        case 'defs':
          return [];
        default:
          throw elem['#name'];
      }
    }).map((elem) => ({
      path: getPath(elem),
      xml: elem,
      fill: undefined,
      fill_solid: undefined,
    }));

    mutations?.forEach((mut: Mutation) => {
      if ('select-path' in mut) {
        if (typeof mut['select-path'] === 'string') {
          mut['select-path'] = [mut['select-path']];
        }
        const T = layers
          .filter((layer) => mut['select-path'].includes(layer.xml.$['d']))
          .map((layer) => (layer.path = new paper.CompoundPath(mut.subst)));
        assert(T.length > 0, `Cannot find path to substitute in ${file}`);
      } else {
        const matrix = new paper.Matrix();
        matrix.translate(mut.translate);
        if (typeof mut.scale === 'number') {
          matrix.scale(mut.scale);
        } else if (Array.isArray(mut.scale)) {
          matrix.scale(mut.scale[0], mut.scale[1]);
        }

        const gids = new Set<string>();
        if ('select-shape' in mut) {
          const bound = new paper.Rectangle(
            new paper.Point(mut['select-shape'][0]),
            new paper.Point(mut['select-shape'][1])
          );
          const T = layers
            .filter((layer) => bound.contains(layer.path.bounds))
            .map((layer) => {
              layer.path.transform(matrix);
              gids.add(getDef(layer.xml.$.fill));
            });
          assert(T.length > 0, `Cannot find shape to transform in ${file}`);
        }

        {
          const T = [...gradients.entries()]
            .filter(
              ([id, gradient]) =>
                gids.has(id) ||
                mut['select-gradient']?.includes(id) ||
                selectGradient(gradient, mut)
            )
            .map(([_, gradient]) => gradient)
            .distinct()
            .map((gradient) => transformGradient(gradient, mut));
          if ('select-gradient' in mut) {
            assert(
              T.length > 0 || mut['gradient-type'] === 'shadow',
              `Cannot find gradient to transform in ${file}`
            );
          }
        }
      }
    });

    layers.forEach((layer) => {
      const props = layer.xml.$;
      let fill: string;
      const fill_attr = props.fill ?? doc.svg.$.fill ?? 'black';
      const stroke_attr = props.stroke ?? doc.svg.$.stroke ?? 'none';
      let opacity_attr = Number(props.opacity ?? '1') * Number(doc.svg.$.opacity ?? '1');
      if (fill_attr !== 'none') {
        opacity_attr *= Number(props['fill-opacity'] ?? '1');
        assert(stroke_attr === 'none', `Unexpected stroke attribute in ${file}`);
        let gid = getDef(fill_attr);
        if (gid) {
          let gd = gradients.get(gid);
          if (opacity_attr !== 1) {
            gd = structuredClone(gd);
            gd.$$.forEach((stop) => {
              stop.$['stop-opacity'] = String(
                Number(stop.$['stop-opacity'] ?? '1') * opacity_attr
              );
            });
          }
          fill = JSON.stringify(gd);
        } else {
          const color = new Color(fill_attr);
          color.alpha *= opacity_attr;
          fill = color
            .toString({
              format: 'hex',
              alpha: true,
              collapse: false,
            })
            .padEnd(9, 'f');
        }
      } else {
        opacity_attr *= Number(props['stroke-opacity'] ?? '1');
        assert(stroke_attr !== 'none', `Unexpected stroke attribute in ${file}`);
        const color = new Color(stroke_attr);
        color.alpha *= opacity_attr;
        fill = color
          .toString({
            format: 'hex',
            alpha: true,
            collapse: false,
          })
          .padEnd(9, 'f');
        const width = Number(props['stroke-width'] ?? doc.svg.$['stroke-width'] ?? '1');
        layer.path = PaperOffset.offsetStroke(layer.path, width / 2);
      }

      if (may_shrink) {
        layer.path.translate([-argv.shrink, -argv.shrink]);
      }

      // try to fix compound path
      if (layer.path.fillRule === 'evenodd' && layer.path.children?.length > 1) {
        const first = layer.path.children[0] as paper.Path;
        layer.path.children.slice(1).forEach((child) => {
          const next = child as paper.Path;
          if (
            (next.intersect(first, { insert: false }) as paper.CompoundPath).area > 0 &&
            first.clockwise == next.clockwise
          ) {
            next.reverse();
          }
        });
      }

      let fill_solid: string;
      if (fill.startsWith('#')) {
        fill_solid = fill;
      } else {
        const gradient = JSON.parse(fill) as LinearGradient | RadialGradient;
        let opacity = gradient.$$.map((stop) => Number(stop.$?.['stop-opacity'] ?? '1')).reduce(
          (min, o) => Math.min(min, o),
          1
        );
        let color: Color;

        const stops = gradient.$$.map((stop) => ({
          offset: Number(stop.$?.['offset'] ?? '0'),
          color: new Color(stop.$?.['stop-color'] ?? 'black').to('oklab'),
        }));

        // lerp at offset 0.5
        stops.sort((a, b) => a.offset - b.offset);
        assert(stops.length > 0, `Gradient without stops in ${file}`);
        if (stops.length == 1 || stops.at(0).offset >= 0.5) {
          color = stops.at(0).color.to('sRGB');
        } else if (stops.at(-1).offset <= 0.5) {
          color = stops.at(-1).color.to('sRGB');
        } else {
          const s = stops.findIndex((stop) => stop.offset >= 0.5);
          const l = stops[s - 1];
          const r = stops[s];
          const p = (0.5 - l.offset) / (r.offset - l.offset);
          color = new Color('oklab', [
            l.color.l + p * (r.color.l - l.color.l),
            l.color.a + p * (r.color.a - l.color.a),
            l.color.b + p * (r.color.b - l.color.b),
          ]).to('sRGB');
        }

        color.alpha *= opacity;
        fill_solid = color
          .toString({
            format: 'hex',
            alpha: true,
            collapse: false,
          })
          .padEnd(9, 'f');
      }
      if (fill_solid.endsWith('00')) {
        fill_solid = '#00000000';
      }

      layer.fill = fill;
      layer.fill_solid = fill_solid;
    });

    for (let i = 0; i < layers.length; i++) {
      while (i + 1 < layers.length) {
        if (layers[i].fill !== layers[i + 1].fill) {
          break;
        }
        const fill = layers[i].fill;
        if (fill.startsWith('#') && !fill.endsWith('ff')) {
          break;
        }
        layers[i].path = layers[i].path.unite(layers[i + 1].path) as paper.CompoundPath;
        layers.splice(i + 1, 1);
      }
    }

    color_glyphs.push({
      name,
      layers: layers.map((layer, l) => {
        let g = glyphs.inverse.get(layer.path.pathData);
        if (g === undefined) {
          g = `_${name}_${l.toString().padStart(2, '0')}`;
          glyphs.set(g, layer.path.pathData);
        }
        return {
          name: g,
          color: layer.fill,
          color_solid: layer.fill_solid,
        };
      }),
    });
  });
});

const mirror_set = new Set<string>(JSON.parse(fs.readFileSync(argv.mirror).toString()));

const colors = new Array<string>();
let g_matrix = new paper.Matrix();
g_matrix.scale(argv['upm'] / output_size);
g_matrix.append(new paper.Matrix(1, 0, 0, -1, 0, output_size)); // flip y

const ttFont = xmlbuilder.create('ttFont', { encoding: 'UTF-8' });
ttFont.att('sfntVersion', '\\x00\\x01\\x00\\x00');
ttFont.att('ttLibVersion', '3.0');

// COLR
const COLR = ttFont.ele('COLR');
COLR.ele('Version', { value: 1 });
const v0_glyphs = COLR.ele('BaseGlyphRecordArray');
const v0_layers = COLR.ele('LayerRecordArray');
const v1_glyphs = COLR.ele('BaseGlyphList');
const v1_layers = COLR.ele('LayerList');
color_glyphs.forEach((record) => {
  const spec = resolveName(record.name);
  const rtl_name = mirror_set.has(spec.name) ? `${spec.name}_rtl-${spec.variant}` : undefined;

  // v0
  {
    const gr = v0_glyphs.ele('BaseGlyphRecord', {
      index: v0_glyphs.children.length,
    });
    gr.ele('BaseGlyph', { value: record.name });
    gr.ele('FirstLayerIndex', { value: v0_layers.children.length });
    gr.ele('NumLayers', { value: record.layers.length });
    record.layers.forEach((layer) => {
      const lr = v0_layers.ele('LayerRecord', {
        index: v0_layers.children.length,
      });
      lr.ele('LayerGlyph', { value: layer.name });
      let c = colors.indexOf(layer.color_solid);
      if (c === -1) {
        c = colors.length;
        colors.push(layer.color_solid);
      }
      lr.ele('PaletteIndex', { value: c });
    });

    if (rtl_name) {
      const rtl_gr = v0_glyphs.ele('BaseGlyphRecord', {
        index: v0_glyphs.children.length,
      });
      rtl_gr.ele('BaseGlyph', { value: `${spec.name}_rtl-${spec.variant}` });
      rtl_gr.ele('FirstLayerIndex', { value: v0_layers.children.length });
      rtl_gr.ele('NumLayers', { value: record.layers.length });
      record.layers.forEach((layer) => {
        const rtl_lr = v0_layers.ele('LayerRecord', {
          index: v0_layers.children.length,
        });
        const path = new paper.CompoundPath(glyphs.get(layer.name));
        path.transform(new paper.Matrix(-1, 0, 0, 1, output_size, 0));
        let glyph_name = glyphs.inverse.get(path.pathData);
        if (glyph_name === undefined) {
          glyph_name = layer.name.replace('-', '_rtl-');
          glyphs.set(glyph_name, path.pathData);
        }
        rtl_lr.ele('LayerGlyph', { value: glyph_name });
        const c = colors.indexOf(layer.color_solid);
        assert(c !== -1, `Color not found for ${record.name}`);
        rtl_lr.ele('PaletteIndex', { value: c });
      });
    }
  }
  // v1
  {
    const gr = v1_glyphs.ele('BaseGlyphPaintRecord', {
      index: v1_glyphs.children.length,
    });
    gr.ele('BaseGlyph', { value: record.name });
    const gp = gr.ele('Paint', { Format: 1 }); // PaintColorLayers
    gp.ele('NumLayers', { value: record.layers.length });
    gp.ele('FirstLayerIndex', { value: v1_layers.children.length });
    record.layers.forEach((layer) => {
      const lr = v1_layers.ele('Paint', {
        index: v1_layers.children.length,
        Format: '10',
      });
      if (layer.color.startsWith('#')) {
        const solid = lr.ele('Paint', { Format: 2 }); // PaintSolid
        let c = colors.indexOf(layer.color);
        if (c === -1) {
          c = colors.length;
          colors.push(layer.color);
        }
        solid.ele('PaletteIndex', {
          value: c,
        });
        solid.ele('Alpha', { value: 1 });
      } else {
        const gradient = JSON.parse(layer.color) as LinearGradient | RadialGradient;
        const matrix = new paper.Matrix();
        if (gradient.$.gradientTransform) {
          gradient.$.gradientTransform
            .trim()
            .split(/(?<=\))\s+/)
            .forEach((t) => {
              let sub: paper.Matrix;
              const matches = t.match(/([a-zA-Z]+)\(((?:[\w\-.]+[\s,]+)*[\w\-.]+)\)/);
              const nums = matches?.[2]?.split(/[\s,]+/)?.map(Number);
              assert(matches && nums, `Cannot parse transform ${t} in ${layer.name}`);
              if (matches[1] === 'matrix' && nums.length === 6) {
                sub = new paper.Matrix(nums);
              } else if (matches[1] === 'translate' && (nums.length == 1 || nums.length == 2)) {
                sub = new paper.Matrix().translate(nums[0], nums[1] ?? 0);
              } else if (matches[1] === 'scale' && (nums.length == 1 || nums.length == 2)) {
                sub = new paper.Matrix().scale(nums[0], nums[1] ?? nums[0]);
              } else if (matches[1] === 'rotate' && (nums.length == 1 || nums.length == 3)) {
                sub = new paper.Matrix().rotate(nums[0], [nums[1] ?? 0, nums[2] ?? 0]);
              }
              assert(sub, `Cannot parse transform ${t} in ${layer.name}`);
              matrix.append(sub);
            });
        }
        matrix.prepend(g_matrix);
        const [pre, post] = divideTransform(matrix);
        let gr_parent = lr;
        if (post) {
          gr_parent = lr.ele('Paint', { Format: 12 }); // PaintTransform
          const transform = gr_parent.ele('Transform');
          transform.ele('xx', { value: post.a });
          transform.ele('xy', { value: post.b });
          transform.ele('yx', { value: post.c });
          transform.ele('yy', { value: post.d });
          transform.ele('dx', { value: post.tx });
          transform.ele('dy', { value: post.ty });
        }

        let cl: xmlbuilder.XMLElement;
        if (gradient['#name'] === 'linearGradient') {
          assert(
            gradient.$.x1 && gradient.$.y1 && gradient.$.x2 && gradient.$.y2,
            `Invalid position for linear gradient in ${layer.name}`
          );
          const p1 = new paper.Point(Number(gradient.$.x1), Number(gradient.$.y1)).transform(
            pre
          );
          const p2 = new paper.Point(Number(gradient.$.x2), Number(gradient.$.y2)).transform(
            pre
          );
          const gr = gr_parent.ele('Paint', { Format: 4 }); // PaintLinearGradient
          cl = gr.ele('ColorLine');
          gr.ele('x0', { value: Math.round(p1.x).toString() });
          gr.ele('y0', { value: Math.round(p1.y).toString() });
          gr.ele('x1', { value: Math.round(p2.x).toString() });
          gr.ele('y1', { value: Math.round(p2.y).toString() });
          gr.ele('x2', { value: Math.round(p1.x + (p2.y - p1.y)).toString() });
          gr.ele('y2', { value: Math.round(p1.y - (p2.x - p1.x)).toString() });
        } else {
          assert(
            gradient.$.cx && gradient.$.cy && gradient.$.r,
            `Invalid position for radial gradient in ${layer.name}`
          );
          const cp = new paper.Point(Number(gradient.$.cx), Number(gradient.$.cy)).transform(
            pre
          );
          const fp = new paper.Point(
            Number(gradient.$.fx ?? gradient.$.cx),
            Number(gradient.$.fy ?? gradient.$.cy)
          ).transform(pre);
          const scaling = Math.sqrt(pre.a * pre.a + pre.b * pre.b);
          const cr = scaling * Number(gradient.$.r);
          const fr = scaling * Number(gradient.$.fr ?? '0');
          const gr = gr_parent.ele('Paint', { Format: 6 }); // PaintRadialGradient
          cl = gr.ele('ColorLine');
          gr.ele('x0', { value: Math.round(fp.x).toString() });
          gr.ele('y0', { value: Math.round(fp.y).toString() });
          gr.ele('r0', { value: Math.round(fr).toString() });
          gr.ele('x1', { value: Math.round(cp.x).toString() });
          gr.ele('y1', { value: Math.round(cp.y).toString() });
          gr.ele('r1', { value: Math.round(cr).toString() });
        }
        cl.ele('Extend', { value: 'pad' });
        gradient.$$.forEach((stop, s) => {
          const color = new Color(stop.$?.['stop-color'] ?? 'black');
          const alpha = Number(stop.$?.['stop-opacity'] ?? '1') * color.alpha;
          color.alpha = 1;
          const hex = color
            .toString({
              format: 'hex',
              alpha: true,
              collapse: false,
            })
            .padEnd(9, 'f');
          let c = colors.indexOf(hex);
          if (c === -1) {
            c = colors.length;
            colors.push(hex);
          }
          const cs = cl.ele('ColorStop', { index: s });
          cs.ele('StopOffset', { value: stop.$?.['offset'] ?? '0' });
          cs.ele('PaletteIndex', { value: c });
          cs.ele('Alpha', { value: alpha });
        });
      }
      lr.ele('Glyph', { value: layer.name });
    });

    if (rtl_name) {
      const rtl_gr = v1_glyphs.ele('BaseGlyphPaintRecord', {
        index: v1_glyphs.children.length,
      });
      rtl_gr.ele('BaseGlyph', { value: rtl_name });
      const rtl_gp = rtl_gr.ele('Paint', { Format: 1 }); // PaintColorLayers
      rtl_gp.ele('NumLayers', { value: 1 });
      rtl_gp.ele('FirstLayerIndex', { value: v1_layers.children.length });

      const rtl_tf = v1_layers.ele('Paint', {
        index: v1_layers.children.length,
        Format: '12',
      });
      const transform = rtl_tf.ele('Transform');
      transform.ele('xx', { value: -1 });
      transform.ele('xy', { value: 0 });
      transform.ele('yx', { value: 0 });
      transform.ele('yy', { value: 1 });
      transform.ele('dx', { value: argv['upm'] });
      transform.ele('dy', { value: 0 });

      const rtl_cg = rtl_tf.ele('Paint', { Format: 11 }); // PaintColrGlyph
      rtl_cg.ele('Glyph', { value: record.name });
    }
  }
});

// CPAL
const CPAL = ttFont.ele('CPAL');
CPAL.ele('version', { value: 0 });
CPAL.ele('numPaletteEntries', { value: colors.length });
const palette = CPAL.ele('palette', { index: 0 });
colors.forEach((color, c) => {
  palette.ele('color', { index: c, value: color });
});

const ttx = fs.createWriteStream(path.join(argv.out, 'colr.ttx'));
ttx.write(ttFont.end({ pretty: true }));
ttx.end();

if (fs.existsSync(argv.out)) {
  fs.rmSync(argv.out, { recursive: true, force: true });
}
ensure(argv.out);
[...glyphs.entries()]
  .filter(([name, _]) => name.startsWith('_'))
  .forEach(([name, path_data]) => {
    fs.writeFileSync(
      path.join(argv.out, `${name}.svg`),
      `<svg width="${output_size}" height="${output_size}" viewBox="0 0 ${output_size} ${output_size}" xmlns="http://www.w3.org/2000/svg">\n  <path d="${path_data}" fill="#212121" />\n</svg>`
    );
  });
