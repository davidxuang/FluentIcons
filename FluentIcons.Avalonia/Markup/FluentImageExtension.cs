using System;
using Avalonia.Media;
using FluentIcons.Common;

namespace FluentIcons.Avalonia.Markup;

public sealed class FluentImageExtension
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public FluentImage ProvideValue(IServiceProvider serviceProvider)
    {
        var image = new FluentImage();

        if (Icon.HasValue) image.Icon = Icon.Value;
        if (IconVariant.HasValue) image.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) image.IconSize = IconSize.Value;
        if (FontSize.HasValue) image.FontSize = FontSize.Value;
        if (Foreground is not null) image.Foreground = Foreground;

        return image;
    }
}
