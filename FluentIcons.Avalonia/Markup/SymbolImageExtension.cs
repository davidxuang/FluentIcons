using System;
using Avalonia.Media;
using FluentIcons.Avalonia.Internals;
using FluentIcons.Common;

namespace FluentIcons.Avalonia.Markup;

public sealed class SymbolImageExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public FlowDirection? FlowDirection { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolImage ProvideValue(IServiceProvider serviceProvider)
    {
        var image = new SymbolImage();

        if (Symbol.HasValue) image.Symbol = Symbol.Value;
        if (IconVariant.HasValue) image.IconVariant = IconVariant.Value;
        if (FlowDirection.HasValue) image.FlowDirection = FlowDirection.Value;
        if (FontSize.HasValue) image.FontSize = FontSize.Value;
        if (Foreground is not null) image.Foreground = Foreground;

        return image;
    }
}
