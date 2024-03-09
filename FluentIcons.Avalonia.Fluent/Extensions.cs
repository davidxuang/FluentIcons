using Avalonia;

namespace FluentIcons.Avalonia.Fluent
{
    public static class Extensions
    {
        public static AppBuilder UseSegoeMetrics(this AppBuilder builder)
        {
            SymbolIcon.UseSegoeMetricsDefaultValue = true;
            return builder;
        }
    }
}
