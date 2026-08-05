using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if ENABLE_LOOKUP_CREATE
using SortSharp;
#endif

namespace FluentIcons.Common.Extensions.Collections;

internal sealed class SortedLookup<K, V> : ILookup<K, V>, IReadOnlyDictionary<K, IReadOnlyList<V>>
{
    private readonly K[] _K;
    private readonly int[] _I;
    private readonly V[] _V;
    private readonly IComparer<K> _keyComparer;

    private SortedLookup(
        K[] k,
        int[] i,
        V[] v,
        IComparer<K> keyComparer)
    {
        _K = k;
        _I = i;
        _V = v;
        _keyComparer = keyComparer;
    }

#if ENABLE_LOOKUP_CREATE
    internal static SortedLookup<K, V> Create(
        Span<(K Key, V Value)> items,
        IComparer<K> keyComparer,
        IComparer<V>? valueComparer = null)
    {
        if (items.Length == 0) return new([], [], [], keyComparer);

        var comparer = new AggregateComparer(keyComparer, valueComparer ?? Comparer<V>.Default);
        items.PDQSort(comparer);

        var V = new V[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            V[i] = items[i].Value;
        }

        int distinct = items.Length > 0 ? 1 : 0;
        for (int i = 1; i < items.Length; i++)
        {
            if (keyComparer.Compare(items[i].Key, items[i - 1].Key) != 0)
            {
                distinct++;
            }
        }

        var K = new K[distinct];
        var I = new int[distinct + 1];
        for (int i = 0, j = 0; i < items.Length; i++)
        {
            if (i == 0 || keyComparer.Compare(items[i].Key, items[i - 1].Key) != 0)
            {
                K[j] = items[i].Key;
                I[j] = i;
                j++;
            }
        }
        I[distinct] = items.Length;

        return new SortedLookup<K, V>(K, I, V, keyComparer);
    }

    internal static SortedLookup<K, V> Create(
        Span<K> keys,
        Span<V> values,
        IComparer<K> keyComparer,
        IComparer<V>? valueComparer = null)
    {
        if (keys.Length != values.Length)
            throw new ArgumentException("Keys and values must have the same length.");

        if (keys.Length == 0) return new([], [], [], keyComparer);

        keys.PDQSort(values, keyComparer);
        var V = values.ToArray();
        int distinct = keys.Length > 0 ? 1 : 0;
        for (int i = 1; i < keys.Length; i++)
        {
            if (keyComparer.Compare(keys[i], keys[i - 1]) != 0)
            {
                distinct++;
            }
        }

        var K = new K[distinct];
        var I = new int[distinct + 1];
        for (int i = 0, j = 0; i < keys.Length; i++)
        {
            if (i == 0 || keyComparer.Compare(keys[i], keys[i - 1]) != 0)
            {
                K[j] = keys[i];
                I[j] = i;
                j++;
            }
        }
        I[distinct] = keys.Length;

        valueComparer ??= Comparer<V>.Default;
        for (int i = 0; i < distinct; i++)
        {
            Array.Sort(V, I[i], I[i + 1] - I[i], valueComparer);
        }

        return new SortedLookup<K, V>(K, I, V, keyComparer);
    }
#endif

    internal static SortedLookup<K, V> Load(K[] k, int[] i, V[] v, IComparer<K> keyComparer)
        => new(k, i, v, keyComparer);
    internal (K[], int[], V[]) Dump() => (_K, _I, _V);

    private bool TryGetRange(K key, out int start, out int end)
    {
        int i = Array.BinarySearch(_K, key, _keyComparer);
        if (i < 0)
        {
            start = 0;
            end = 0;
            return false;
        }

        start = _I[i];
        end = _I[i + 1];
        return true;
    }

    public Span<V> this[K key] => TryGetRange(key, out int start, out int end) ? _V.AsSpan(start, end - start) : [];
    IReadOnlyList<V> IReadOnlyDictionary<K, IReadOnlyList<V>>.this[K key] => TryGetRange(key, out int start, out int end) ? new ArraySegment<V>(_V, start, end - start) : [];
    IEnumerable<V> ILookup<K, V>.this[K key] => TryGetRange(key, out int start, out int end) ? new ArraySegment<V>(_V, start, end - start) : [];

    public IEnumerable<K> Keys => _K;
    public IEnumerable<IReadOnlyList<V>> Values => _K.Select(k => (this as IReadOnlyDictionary<K, IReadOnlyList<V>>)[k]);

    public int Count => _K.Length;

    public bool Contains(K key) => Array.BinarySearch(_K, key, _keyComparer) >= 0;
    public bool ContainsKey(K key) => Array.BinarySearch(_K, key, _keyComparer) >= 0;

    public bool TryGetValue(K key, out IReadOnlyList<V> value)
    {
        if (TryGetRange(key, out int start, out int end))
        {
            value = new ArraySegment<V>(_V, start, end - start);
            return true;
        }
        value = [];
        return false;
    }

    public IEnumerator<IGrouping<K, V>> GetEnumerator()
        => _K.Select<K, IGrouping<K, V>>((key, k) => new Grouping(key, new ArraySegment<V>(_V, _I[k], _I[k + 1] - _I[k]))).GetEnumerator();
    IEnumerator<KeyValuePair<K, IReadOnlyList<V>>> IEnumerable<KeyValuePair<K, IReadOnlyList<V>>>.GetEnumerator()
        => _K.Select((key, k) => new KeyValuePair<K, IReadOnlyList<V>>(key, new ArraySegment<V>(_V, _I[k], _I[k + 1] - _I[k]))).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly struct AggregateComparer(IComparer<K> keyComparer, IComparer<V> valueComparer) : IComparer<(K Key, V Value)>
    {
        public readonly IComparer<K> KC = keyComparer;
        public readonly IComparer<V> VC = valueComparer;

        public int Compare((K Key, V Value) x, (K Key, V Value) y)
        {
            var keyComparison = KC.Compare(x.Key, y.Key);
            if (keyComparison != 0)
                return keyComparison;

            return VC.Compare(x.Value, y.Value);
        }
    }

    private readonly struct Grouping(K key, ArraySegment<V> segment) : IGrouping<K, V>
    {
        public readonly K Key => key;

        public readonly IEnumerator<V> GetEnumerator() => segment.AsEnumerable().GetEnumerator();
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
