using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace FluentIcons.Common.Internals;

internal static class Extensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToString<T>(this T value, IconVariant iconVariant, bool isRtl)
        where T : struct, Enum
        => char.ConvertFromUtf32(0xf0000 + 4 * Convert.ToInt32(value) + (int)iconVariant + (Convert.ToInt32(isRtl) * 0x10000));

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
