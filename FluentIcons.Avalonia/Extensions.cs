using Avalonia;

namespace FluentIcons.Avalonia
{
    public static class Extensions
    {
        public static AppBuilder UseSegoeMetrics(this AppBuilder builder)
        {
            SymbolIcon.UseSegoeMetricsProperty.OverrideDefaultValue(typeof(SymbolIcon), true);
            return builder;
        }
    }
}
