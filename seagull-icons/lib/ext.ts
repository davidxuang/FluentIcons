export {};

declare global {
  interface Array<T> {
    single<S extends T>(predicate?: (value: T, index: number, array: T[]) => value is S): S;
    single(predicate?: (value: T, index: number, array: T[]) => boolean): T;
    distinct(): T[];
  }
}

if (!Array.prototype.single) {
  Array.prototype.single = function (predicate) {
    const a = predicate ? this.filter(predicate) : this;
    if (a.length !== 1) {
      throw new Error("Expected single element");
    }
    return a[0];
  };
}

if (!Array.prototype.distinct) {
  Array.prototype.distinct = function () {
    return Array.from(new Set(this));
  };
}
