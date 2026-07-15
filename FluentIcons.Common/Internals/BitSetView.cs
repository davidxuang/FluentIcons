using System;
using System.Runtime.CompilerServices;

namespace FluentIcons.Common.Internals;

internal readonly struct BitSetView(byte[] bytes)
{
    private readonly byte[] _bytes = bytes;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Get(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        return index < _bytes.Length * 8
            && (_bytes[index / 8] & (1 << (index % 8))) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int index, bool value)
    {
        if (index < 0 || index >= _bytes.Length * 8)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (value)
            _bytes[index / 8] |= (byte)(1 << (index % 8));
        else
            _bytes[index / 8] &= (byte)~(1 << (index % 8));
    }

    public bool this[int index]
    {
        get => Get(index);
        set => Set(index, value);
    }
}
