using System;
using Avalonia.Media;
using FluentIcons.Avalonia.Internals;
using FluentIcons.Common;

namespace FluentIcons.Avalonia.Markup;

public sealed class FluentIconExtension
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public FlowDirection? FlowDirection { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public FluentIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new FluentIcon();

        if (Icon.HasValue) icon.Icon = Icon.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) icon.IconSize = IconSize.Value;
        if (FlowDirection.HasValue) icon.FlowDirection = FlowDirection.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        return icon;
    }
}
