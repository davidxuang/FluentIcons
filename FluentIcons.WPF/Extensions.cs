using System.Windows;

namespace FluentIcons.WPF;

public static class Extensions
{
    public static Application UseSegoeMetrics(this Application app)
    {
        SymbolIcon.UseSegoeMetricsDefaultValue = true;
        return app;
    }
}
