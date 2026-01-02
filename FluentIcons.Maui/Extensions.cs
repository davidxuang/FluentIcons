using System;
using FluentIcons.Common;
using Microsoft.Maui.Hosting;
using System.Linq;

namespace FluentIcons.Maui;

public static class Extensions
{
    public static MauiAppBuilder UseFluentIcons(this MauiAppBuilder builder)
    {
        var assembly = typeof(SymbolIcon).Assembly;
        return builder.ConfigureFonts(fonts =>
        {
            foreach (var size in IconSizeValues.Enumerable.Where(size => (byte)size > 0))
            {
                fonts.AddEmbeddedResourceFont(assembly, $"FluentSystemIcons-{size}.otf", $"FluentSystemIcons{size}");
            }
            fonts.AddEmbeddedResourceFont(assembly, "SeagullFluentIcons.otf", "SeagullFluentIcons");
        });
    }
}
