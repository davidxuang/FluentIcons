import * as fs from 'fs';
import * as paper from 'paper';
import { Point, Rectangle, Size } from 'paper';
import { Drawable, Elem, Visible } from './types';

const rexPoly =
  /(-?[\d]+(?:\.\d+)?|-?\.\d+),? *(-?[\d]+(?:\.\d+)?|-?\.\d+),? */g;

function getPathData(elem: Visible) {
  switch (elem['#name']) {
    case 'path':
      return elem.$.d;
    case 'rect':
      const r =
        elem.$['rx'] || elem.$['ry']
          ? new paper.Path.Rectangle(
              new Rectangle(
                new Point(
                  parseFloat(elem.$['x'] ?? '0'),
                  parseFloat(elem.$['y'] ?? '0')
                ),
                new Size(
                  parseFloat(elem.$['width'] ?? '0'),
                  parseFloat(elem.$['height'] ?? '0')
                )
              ),
              new Size(
                parseFloat(elem.$['rx'] ?? elem.$['ry']),
                parseFloat(elem.$['ry'] ?? elem.$['rx'])
              )
            )
          : new paper.Path.Rectangle(
              new Point(
                parseFloat(elem.$['x'] ?? '0'),
                parseFloat(elem.$['y'] ?? '0')
              ),
              new Size(
                parseFloat(elem.$['width'] ?? '0'),
                parseFloat(elem.$['height'] ?? '0')
              )
            );
      const rd = r.pathData;
      r.remove();
      return rd;
    case 'circle':
      const c = new paper.Path.Circle(
        new Point(
          parseFloat(elem.$['cx'] ?? '0'),
          parseFloat(elem.$['cy'] ?? '0')
        ),
        parseFloat(elem.$['r'] ?? '0')
      );
      const cd = c.pathData;
      c.remove();
      return cd;
    case 'ellipse':
      var rx = parseFloat(elem.$['rx'] ?? '0');
      var ry = parseFloat(elem.$['ry'] ?? '0');
      const e = new paper.Path.Ellipse(
        new Rectangle(
          new Point(
            parseFloat(elem.$['cx'] ?? '0') - rx,
            parseFloat(elem.$['cy'] ?? '0') - ry
          ),
          new Size(2 * rx, 2 * ry)
        )
      );
      const ed = e.pathData;
      e.remove();
      return ed;
    case 'line':
      return `M${elem.$['x1'] ?? '0'},${elem.$['y1'] ?? '0'}L${
        elem.$['x2'] ?? '0'
      },${elem.$['y2'] ?? '0'}Z`;
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

function ensure(dir: string) {
  if (!fs.existsSync(dir)) {
    fs.mkdirSync(dir, { recursive: true });
  }
}

function forEachDrawable(
  elems: Elem[],
  cb: (drawable: Drawable, index: number) => void
) {
  let index = 0;
  function f(elems: Elem[]) {
    elems.forEach((elem) => {
      switch (elem['#name']) {
        case 'circle':
        case 'ellipse':
        case 'line':
        case 'path':
        case 'polygon':
        case 'polyline':
        case 'rect':
          cb(elem, index);
          index++;
          break;
        case 'g':
          f(elem.$$);
          break;
      }
    });
  }
  f(elems);
}

export { ensure, forEachDrawable, getPathData };
