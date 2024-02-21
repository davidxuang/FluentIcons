using Avalonia;

namespace FluentIcons.Avalonia.Fluent
{
    public static class Extensions
    {
        public static AppBuilder UseSegoeMetrics(this AppBuilder builder)
        {
            SymbolIcon.UseSegoeMetricsProperty.OverrideDefaultValue(typeof(SymbolIcon), true);
            SymbolIcon.UseSegoeMetricsProperty.OverrideDefaultValue(typeof(SymbolIconSource), true);
            return builder;
        }
    }
}
