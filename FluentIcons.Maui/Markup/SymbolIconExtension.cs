using System;
using FluentIcons.Common;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace FluentIcons.Maui.Markup;

[RequireService([typeof(IProvideValueTarget)])]
public class SymbolIconExtension : IMarkupExtension<SymbolIcon>
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public double? FontSize { get; set; }
    public Color? ForegroundColor { get; set; }

    public SymbolIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (ForegroundColor is not null) icon.ForegroundColor = ForegroundColor;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is VisualElement source)
        {
            icon.SetBinding(VisualElement.FlowDirectionProperty, new Binding(nameof(FlowDirection), source: source));
        }

        return icon;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        => ProvideValue(serviceProvider);
}
