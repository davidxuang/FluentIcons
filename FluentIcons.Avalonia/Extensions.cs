using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Avalonia;

namespace FluentIcons.Avalonia
{
    public static class Extensions
    {
        public static AppBuilder UseSegoeMetrics(this AppBuilder builder)
        {
            SymbolIcon.UseSegoeMetricsDefaultValue = true;
            return builder;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Or(this double value, double other)
        {
#if NETSTANDARD && !NETSTANDARD2_1_OR_GREATER
            return !double.IsNaN(value) && !double.IsInfinity(value) ? value : other;
#else
            return double.IsFinite(value) ? value : other;
#endif
        }
    }
}
