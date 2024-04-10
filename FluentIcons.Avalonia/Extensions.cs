using Avalonia.Controls;

namespace FluentIcons.Avalonia;

public static class Extensions
{
    public static T UseSegoeMetrics<T>(this T builder)
        where T : AppBuilderBase<T>, new()
    {
        SymbolIcon.UseSegoeMetricsDefaultValue = true;
        return builder;
    }
}
