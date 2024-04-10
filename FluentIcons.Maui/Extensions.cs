using Microsoft.Maui.Hosting;

namespace FluentIcons.Maui;

public static class Extensions
{
    public static MauiAppBuilder UseFluentIcons(this MauiAppBuilder builder, bool useSegoeMetrics)
    {
        var assembly = typeof(SymbolIcon).Assembly;
        SymbolIcon.UseSegoeMetricsDefaultValue = useSegoeMetrics;
        return builder.ConfigureFonts(fonts =>
        {
            fonts.AddEmbeddedResourceFont(assembly, "FluentSystemIcons.ttf", "FluentSystemIcons");
            fonts.AddEmbeddedResourceFont(assembly, "SeagullFluentIcons.ttf", "SeagullFluentIcons");
        });
    }
}

