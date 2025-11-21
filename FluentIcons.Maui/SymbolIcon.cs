using System;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace FluentIcons.Maui;

public partial class SymbolIcon : GenericIcon
{
    public SymbolIcon()
    {
        InvalidateText();
    }

    public Symbol Symbol
    {
        get => (Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
    public static readonly BindableProperty SymbolProperty
        = BindableProperty.Create(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), Symbol.Home, propertyChanged: OnIconPropertiesChanged);

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override string IconFont => "SeagullFluentIcons";
}

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
    {
        return ProvideValue(serviceProvider);
    }
}
