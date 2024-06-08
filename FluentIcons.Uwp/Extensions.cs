using Windows.UI.Xaml;

namespace FluentIcons.Uwp;

public static class Extensions
{
    public static Application UseSegoeMetrics(this Application app)
    {
#if !NET || WINDOWS
        SymbolIcon.UseSegoeMetricsDefaultValue = true;
#else
        var defalutValueProperty = typeof(PropertyMetadata).GetProperty(nameof(PropertyMetadata.DefaultValue));
        defalutValueProperty!.SetValue(SymbolIcon.UseSegoeMetricsProperty.GetMetadata(typeof(SymbolIcon)), true);
        defalutValueProperty!.SetValue(SymbolIconSource.UseSegoeMetricsProperty.GetMetadata(typeof(SymbolIconSource)), true);
#endif
        return app;
    }
}
