using System;
using FluentIcons.Common.Internals;

namespace FluentIcons.Common;

public static partial class IconExtensions
{
    public static bool IsAvailable(this Icon icon, IconSize size, IconVariant variant)
    {
        return (size switch
        {
            IconSize.Resizable => _bits0,
            IconSize.Size10 => _bits10,
            IconSize.Size12 => _bits12,
            IconSize.Size16 => _bits16,
            IconSize.Size20 => _bits20,
            IconSize.Size24 => _bits24,
            IconSize.Size28 => _bits28,
            IconSize.Size32 => _bits32,
            IconSize.Size48 => _bits48,
            _ => throw new ArgumentOutOfRangeException(nameof(size))
        }).Get(icon.GetOffset(variant));
    }

    public static bool IsAvailable(this Symbol symbol, IconVariant variant)
    {
        return _bits0.Get(symbol.GetOffset(variant));
    }
}
