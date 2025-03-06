using System;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace FluentIcons.Maui;

public partial class SymbolImageSource : GenericImageSource
{
    public static readonly BindableProperty SymbolProperty
        = BindableProperty.Create(nameof(Symbol), typeof(Symbol), typeof(SymbolImageSource), Symbol.Home, propertyChanged: OnIconPropertiesChanged);
    [Obsolete(Extensions.Message)]
    public static readonly BindableProperty UseSegoeMetricsProperty
        = BindableProperty.Create(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolImageSource), defaultValue: false, propertyChanged: OnIconPropertiesChanged);

    public Symbol Symbol
    {
        get => (Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    [Obsolete(Extensions.Message)]
    public bool UseSegoeMetrics
    {
        get => (bool)GetValue(UseSegoeMetricsProperty);
        set => SetValue(UseSegoeMetricsProperty, value);
    }

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override string IconFont => "SeagullFluentIcons";
}

public class SymbolImageSourceExtension : IMarkupExtension<SymbolImageSource>
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    [Obsolete(Extensions.Message)]
    public bool? UseSegoeMetrics { get; set; }
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
