using System;
using FluentIcons.Common;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace FluentIcons.Maui.Markup;

[RequireService([typeof(IProvideValueTarget)])]
public class SymbolImageSourceExtension : IMarkupExtension<SymbolImageSource>
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public double? Size { get; set; }
    public Color? Color { get; set; }

    public SymbolImageSource ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolImageSource();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (Size.HasValue) icon.Size = Size.Value;
        if (Color is not null) icon.Color = Color;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is VisualElement source)
        {
            icon.FlowDirection = source.FlowDirection;
        }

        return icon;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
