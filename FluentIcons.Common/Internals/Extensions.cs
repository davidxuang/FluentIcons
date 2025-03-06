using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace FluentIcons.Common.Internals;

internal static class Extensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToString(this Icon icon, IconVariant iconVariant, bool isRtl)
        => char.ConvertFromUtf32((int)icon + (int)iconVariant + (Convert.ToInt32(isRtl) * 0x10000));
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToString(this Symbol symbol, IconVariant iconVariant, bool isRtl)
        => char.ConvertFromUtf32((int)symbol + (int)iconVariant + (Convert.ToInt32(isRtl) * 0x10000));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double Or(this double value, double other)
    {
#if NETSTANDARD2_1_OR_GREATER
        return double.IsFinite(value) ? value : other;
#else
        return !double.IsNaN(value) && !double.IsInfinity(value) ? value : other;
#endif
    }
}
