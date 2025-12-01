type ElemBase = {
  '#name': string;
  $: {
    id?: string;
  };
};

type ProtoRenderable = ElemBase & {
  $: {
    fill?: string;
    'fill-opacity'?: string;
    'fill-rule'?: 'nonzero' | 'evenodd';
    stroke?: string;
    'stroke-opacity'?: string;
    'stroke-width'?: string;
    opacity?: string;
    transform?: string;
  };
};

export type Rect = ProtoRenderable & {
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

export type Circle = ProtoRenderable & {
  '#name': 'circle';
  $: {
    cx?: string;
    cy?: string;
    r: string;
  };
};

export type Ellipse = ProtoRenderable & {
  '#name': 'ellipse';
  $: {
    cx?: string;
    cy?: string;
    rx: string;
    ry: string;
  };
};

export type Line = ProtoRenderable & {
  '#name': 'line';
  $: {
    x1?: string;
    y1?: string;
    x2?: string;
    y2?: string;
  };
};

export type Polyline = ProtoRenderable & {
  '#name': 'polyline';
  $: {
    points?: string;
  };
};

export type Polygon = ProtoRenderable & {
  '#name': 'polygon';
  $: {
    points?: string;
  };
};

export type Path = ProtoRenderable & {
  '#name': 'path';
  $: {
    d: string;
  };
};

export type Use = ProtoRenderable & {
  '#name': 'use';
  $: {
    'xlink:href': string;
  };
};

export type Group = ProtoRenderable & {
  '#name': 'g';
  $: {
    filter?: string;
    ['clip-path']?: string;
  };
  $$: Renderable[];
};

export type Shape = Rect | Circle | Ellipse | Line | Polyline | Polygon | Path;
export type Renderable = Shape | Group | Use;
export type Elem = Def | Defs | Renderable;

export type Stop = {
  '#name': 'stop';
  $: {
    offset?: string;
    'stop-color': string;
    'stop-opacity'?: string;
  };
};

export type LinearGradient = ElemBase & {
  '#name': 'linearGradient';
  $: {
    x1?: string;
    y1?: string;
    x2?: string;
    y2?: string;
    gradientUnits?: 'userSpaceOnUse' | 'objectBoundingBox';
    gradientTransform?: string;
  };
  $$: Stop[];
};

export type RadialGradient = ElemBase & {
  '#name': 'radialGradient';
  $: {
    cx?: string;
    cy?: string;
    r?: string;
    fx?: string;
    fy?: string;
    fr?: string;
    gradientUnits?: 'userSpaceOnUse' | 'objectBoundingBox';
    gradientTransform?: string;
  };
  $$: Stop[];
};

export type ClipPath = ElemBase & {
  '#name': 'clipPath';
  $: {};
  $$?: Renderable[];
};

export type FeFlood = ElemBase & {
  '#name': 'feFlood';
  $: {
    'flood-color': string;
    'flood-opacity': string;
  };
}

export type FeColorMatrix = ElemBase & {
  '#name': 'feColorMatrix';
  $: {
    in: string;
    type: 'matrix' | 'saturate' | 'hueRotate' | 'luminanceToAlpha';
    values: string;
  };
}

export type FeOffset = ElemBase & {
  '#name': 'feOffset';
  $: {
    in: string;
    dx: string;
    dy: string;
  };
}

export type FeGaussianBlur = ElemBase & {
  '#name': 'feGaussianBlur';
  $: {
    in: string;
    stdDeviation: string;
  };
}

export type FeBlend = ElemBase & {
  '#name': 'feBlend';
  $: {
    // dummy
  };
}

export type Fe = FeFlood | FeColorMatrix | FeOffset | FeGaussianBlur | FeBlend;

export type Filter = ElemBase & {
  '#name': 'filter';
  $: {
    x?: string;
    y?: string;
    width?: string;
    height?: string;
    filterUnits?: 'userSpaceOnUse' | 'objectBoundingBox';
    ['color-interpolation-filters']?: 'auto' | 'sRGB' | 'linearRGB';
  };
  $$?: Fe[];
}

export type Def = LinearGradient | RadialGradient | ClipPath | Filter;

export type Defs = ElemBase & {
  '#name': 'defs';
  $$?: Def[];
};

export type Svg = ProtoRenderable & {
  '#name': 'svg';
  $: {
    xmlns: string;
    width?: string;
    height?: string;
    viewBox?: string;
  };
  $$?: Elem[];
};

export type Doc = {
  svg: Svg;
};
