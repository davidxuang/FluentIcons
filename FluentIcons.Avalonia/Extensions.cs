using Avalonia;

namespace FluentIcons.Avalonia;

public static class Extensions
{
    public static AppBuilder UseSegoeMetrics(this AppBuilder builder)
    {
        SymbolIcon.UseSegoeMetricsDefaultValue = true;
        return builder;
    }
}
