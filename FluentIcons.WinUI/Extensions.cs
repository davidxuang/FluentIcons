using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace FluentIcons.WinUI;

public static class Extensions
{
    public static Application UseSegoeMetrics(this Application app)
    {
#if !NET || WINDOWS
        SymbolIcon.UseSegoeMetricsDefaultValue = true;
#else
        SymbolIcon.UseSegoeMetricsProperty.GetMetadata(typeof(SymbolIcon)).DefaultValue = true;
        SymbolIconSource.UseSegoeMetricsProperty.GetMetadata(typeof(SymbolIconSource)).DefaultValue = true;
#endif
        return app;
    }

    public static IHostBuilder UseSegoeMetrics(this IHostBuilder builder)
    {
#if !NET || WINDOWS
        SymbolIcon.UseSegoeMetricsDefaultValue = true;
#else
        SymbolIcon.UseSegoeMetricsProperty.GetMetadata(typeof(SymbolIcon)).DefaultValue = true;
        SymbolIconSource.UseSegoeMetricsProperty.GetMetadata(typeof(SymbolIconSource)).DefaultValue = true;
#endif
        return builder;
    }
}
