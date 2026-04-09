using System;
using FluentIcons.Common;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace FluentIcons.Maui.Markup;

public sealed class FluentIconExtension : IMarkupExtension<FluentIcon>
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public double? FontSize { get; set; }
    public Color? ForegroundColor { get; set; }

    public FluentIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new FluentIcon();

        if (Icon.HasValue) icon.Icon = Icon.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) icon.IconSize = IconSize.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (ForegroundColor is not null) icon.ForegroundColor = ForegroundColor;

        return icon;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        => ProvideValue(serviceProvider);
}
