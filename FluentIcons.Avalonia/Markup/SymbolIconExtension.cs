using System;
using Avalonia.Media;
using FluentIcons.Avalonia.Internals;
using FluentIcons.Common;

namespace FluentIcons.Avalonia.Markup;

public sealed class SymbolIconExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public FlowDirection? FlowDirection { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FlowDirection.HasValue) icon.FlowDirection = FlowDirection.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        return icon;
    }
}
