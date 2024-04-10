type ElemBase = {
  '#name': string;
  $: {
    id?: string;
  };
};

type ProtoVisible = ElemBase & {
  $: {
    fill?: string;
    stroke?: string;
    'stroke-width'?: string;
    opacity?: string;
    transform?: string;
  };
};

type Rect = ProtoVisible & {
  '#name': 'rect';
  $: {
    x?: string;
    y?: string;
    width?: string;
    height?: string;
    rx?: string;
    ry?: string;
  };
};

type Circle = ProtoVisible & {
  '#name': 'circle';
  $: {
    cx?: string;
    cy?: string;
    r: string;
  };
};

type Ellipse = ProtoVisible & {
  '#name': 'ellipse';
  $: {
    cx?: string;
    cy?: string;
    rx: string;
    ry: string;
  };
};

type Line = ProtoVisible & {
  '#name': 'line';
  $: {
    x1?: string;
    y1?: string;
    x2?: string;
    y2?: string;
  };
};

type Polyline = ProtoVisible & {
  '#name': 'polyline';
  $: {
    points?: string;
  };
};

type Polygon = ProtoVisible & {
  '#name': 'polygon';
  $: {
    points?: string;
  };
};

type Path = ProtoVisible & {
  '#name': 'path';
  $: {
    d: string;
  };
};

type Use = ProtoVisible & {
  '#name': 'use';
  $: {
    'xlink:href': string;
  };
};

type Group = ProtoVisible & {
  '#name': 'g';
  $: {};
  $$: Visible[];
};

type Drawable = Rect | Circle | Ellipse | Line | Polyline | Polygon | Path;
type Visible = Drawable | Group | Use;

type Metadata = ProtoVisible & {
  '#name': 'metadata';
  $: {};
};

type LinearGradient = ElemBase & {
  '#name': 'linearGradient';
  $: {};
};

type ClipPath = ElemBase & {
  '#name': 'clipPath';
  $: {};
  $$?: Visible[];
};

type Def = LinearGradient | ClipPath;

type Defs = ElemBase & {
  '#name': 'defs';
  $$?: Def[];
};

type Elem = Metadata | Def | Defs | Visible;

type Svg = {
  '#name': 'svg';
  $: {
    xmlns: string;
    width?: string;
    height?: string;
    viewBox?: string;
  };
  $$?: Elem[];
};

type Doc = {
  svg: Svg;
};

export type {
  Doc,
  Visible,
  Drawable,
  Rect,
  Circle,
  Ellipse,
  Line,
  Polyline,
  Polygon,
  Path,
  Group,
  Metadata,
  LinearGradient,
  Def,
  Defs,
  Elem,
  Svg,
};
