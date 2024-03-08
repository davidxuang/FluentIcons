using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FluentIcons.WPF
{
    public static class Extensions
    {
        public static Application UseSegoeMetrics(this Application app)
        {
            SymbolIcon.UseSegoeMetricsDefaultValue = true;
            return app;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double Or(this double value, double other)
        {
#if NETFRAMEWORK
            return !double.IsNaN(value) && !double.IsInfinity(value) ? value : other;
#else
            return double.IsFinite(value) ? value : other;
#endif
        }
    }
}
