import fs from 'fs';
import paper from 'paper';
import { Rectangle } from 'paper';
import { Renderable } from './types';

export function resolveName(name: string, upstream = false) {
  if (upstream) {
    const matches = name.match(
      /^ic_fluent_(.+)_(\d+)_(regular|filled|color|light)(\.svg)?$/
    );
    return matches === null
      ? null
      : {
          name: matches[1],
          size: parseInt(matches[2]),
          variant: matches[3],
        };
  } else {
    const matches = name.match(/^(.+)-(regular|filled|color|light)(\.svg)?$/);
    return matches === null
      ? null
      : {
          name: matches[1],
          variant: matches[2],
        };
  }
}

export function getPath(elem: Renderable): paper.Path | paper.CompoundPath {
  switch (elem['#name']) {
    case 'path':
      return new paper.CompoundPath(elem.$.d);
    case 'rect':
      return elem.$['rx'] || elem.$['ry']
        ? new paper.Path.Rectangle(
            new Rectangle(
              [Number(elem.$['x'] ?? '0'), Number(elem.$['y'] ?? '0')],
              [Number(elem.$['width'] ?? '0'), Number(elem.$['height'] ?? '0')]
            ),
            [
              Number(elem.$['rx'] ?? elem.$['ry']),
              Number(elem.$['ry'] ?? elem.$['rx']),
            ]
          )
        : new paper.Path.Rectangle(
            [Number(elem.$['x'] ?? '0'), Number(elem.$['y'] ?? '0')],
            [Number(elem.$['width'] ?? '0'), Number(elem.$['height'] ?? '0')]
          );
    case 'circle':
      return new paper.Path.Circle(
        [Number(elem.$['cx'] ?? '0'), Number(elem.$['cy'] ?? '0')],
        Number(elem.$['r'] ?? '0')
      );
    case 'ellipse':
      const rx = parseFloat(elem.$['rx'] ?? '0');
      const ry = parseFloat(elem.$['ry'] ?? '0');
      return new paper.Path.Ellipse(
        new Rectangle(
          [Number(elem.$['cx'] ?? '0') - rx, Number(elem.$['cy'] ?? '0') - ry],
          [2 * rx, 2 * ry]
        )
      );
    case 'line':
      return new paper.Path.Line(
        [Number(elem.$['x1'] ?? '0'), Number(elem.$['y1'] ?? '0')],
        [Number(elem.$['x2'] ?? '0'), Number(elem.$['y2'] ?? '0')]
      );
    case 'polygon':
    case 'polyline':
    case 'g':
      return new paper.CompoundPath(getPathData(elem));
    default:
      throw elem['#name'];
  }
}

const rexPoly =
  /(-?[\d]+(?:\.\d+)?|-?\.\d+),? *(-?[\d]+(?:\.\d+)?|-?\.\d+),? */g;

export function getPathData(elem: Renderable): string {
  switch (elem['#name']) {
    case 'path':
      return elem.$.d;
    case 'rect':
    case 'circle':
    case 'ellipse':
    case 'line':
      return getPath(elem).pathData;
    case 'polygon':
    case 'polyline':
      var matches: [string, string][] = [];
      var match: RegExpExecArray;
      rexPoly.lastIndex = 0;
      while ((match = rexPoly.exec(elem.$.points))) {
        matches.push([match[1], match[2]]);
      }
      if (elem['#name'] == 'polygon') {
        return `M${matches.map((coor) => `${coor[0]},${coor[1]}`).join('L')}L${
          matches[0][0]
        },${matches[0][1]}Z`;
      } else {
        return `M${matches.map((coor) => `${coor[0]},${coor[1]}`).join('L')}Z`;
      }
    case 'g':
      return elem.$$.map((e) => getPathData(e)).join();
    default:
      throw elem['#name'];
  }
}

export function ensure(dir: string) {
  if (!fs.existsSync(dir)) {
    fs.mkdirSync(dir, { recursive: true });
  }
}

function fitCircle(points: { x: number; y: number }[]) {
  const centroid = {
    x: points.reduce((acc, p) => acc + p.x, 0) / points.length,
    y: points.reduce((acc, p) => acc + p.y, 0) / points.length,
  };

  let A = new Array<[number, number, number]>();
  let B = new Array<[number]>();

  points
    .map((p) => ({
      x: p.x - centroid.x,
      y: p.y - centroid.y,
    }))
    .forEach((p) => {
      A.push([p.x, p.y, 1]);
      B.push([p.x * p.x + p.y * p.y]);
    });

  // solve Ax = B
  const n = A.length;
  const m = A[0].length;

  const aug = A.map((row, i) => [...row, B[i][0]]);
  for (let i = 0; i < n; i++) {
    let M = i;
    for (let j = i + 1; j < n; j++) {
      if (Math.abs(aug[j][i]) > Math.abs(aug[M][i])) {
        M = j;
      }
    }

    if (M !== i) {
      [aug[i], aug[M]] = [aug[M], aug[i]];
    }

    for (let j = i + 1; j < n; j++) {
      const f = aug[j][i] / aug[i][i];
      for (let k = i; k <= m; k++) {
        aug[j][k] -= f * aug[i][k];
      }
    }
  }

  const solution = new Array<number>(m).fill(0);
  for (let i = m - 1; i >= 0; i--) {
    let sum = aug[i][m];
    for (let j = i + 1; j < m; j++) {
      sum -= aug[i][j] * solution[j];
    }
    solution[i] = sum / aug[i][i];
  }

  const [a, b, c] = solution;
  return {
    cx: centroid.x + a / 2,
    cy: centroid.y + b / 2,
    r: Math.sqrt(a * a + b * b + c),
  };
}

export function approximateShape(path: paper.Path, tolerance: number = 1e-3) {
  const points = path.segments.map((s) => s.point);
  const { cx, cy, r } = fitCircle(points);
  const circle = new paper.Path.Circle([cx, cy], r);
  const d_area = (circle.exclude(path) as paper.Path | paper.CompoundPath).area;
  if (
    Math.abs(d_area / circle.area) < tolerance &&
    Math.abs(path.length - circle.length) / circle.length < tolerance
  ) {
    return { cx, cy, r };
  }

  return undefined;
}

export function divideTransform(
  matrix: paper.Matrix
): [paper.Matrix, paper.Matrix | undefined] {
  const d = matrix.decompose() as {
    translation: paper.Point;
    rotation: number;
    scaling: paper.Point;
    skewing: paper.Point;
  };

  if (
    Math.abs(Math.abs(d.scaling.x / d.scaling.y) - 1) < 1e-6 &&
    Math.max(Math.abs(d.skewing.x), Math.abs(d.skewing.y)) < 1e-6
  ) {
    return [new paper.Matrix(matrix), undefined];
  }

  const scaling = Math.max(Math.abs(d.scaling.x), Math.abs(d.scaling.y));
  const first = new paper.Matrix();
  first.translate(d.translation);
  first.rotate(d.rotation, [0, 0]);
  first.scale(
    scaling * Math.sign(d.scaling.x),
    scaling * Math.sign(d.scaling.y)
  );
  first.skew(d.skewing.x, d.skewing.y);

  const second = new paper.Matrix(matrix);
  second.append(first.inverted());

  return [first, second];
}
