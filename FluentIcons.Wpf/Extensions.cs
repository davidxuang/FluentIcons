using System.Windows;

namespace FluentIcons.Wpf;

public static class Extensions
{
    public static Application UseSegoeMetrics(this Application app)
    {
        SymbolIconBase.UseSegoeMetricsProperty.OverrideMetadata(
            typeof(SymbolIcon),
            new PropertyMetadata(true, SymbolIconBase.UseSegoeMetricsProperty.DefaultMetadata.PropertyChangedCallback));
        return app;
    }
}
