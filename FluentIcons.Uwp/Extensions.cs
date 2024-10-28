using Windows.UI.Xaml;

namespace FluentIcons.Uwp;

public static class Extensions
{
    public static Application UseSegoeMetrics(this Application app)
    {
#if !HAS_UNO
        SymbolIcon.UseSegoeMetricsDefaultValue = true;
#else
        SymbolIcon.UseSegoeMetricsProperty.GetMetadata(typeof(SymbolIcon)).DefaultValue = true;
        SymbolIconSource.UseSegoeMetricsProperty.GetMetadata(typeof(SymbolIconSource)).DefaultValue = true;
#endif
        return app;
    }
}
