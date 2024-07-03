using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace FluentIcons.Maui;

public class SymbolImageSource : FontImageSource
{
    public static readonly BindableProperty SymbolProperty
        = BindableProperty.Create(nameof(Symbol), typeof(Symbol), typeof(SymbolImageSource), Symbol.Home, propertyChanged: OnSymbolPropertiesChanged);
    public static readonly BindableProperty IconVariantProperty
        = BindableProperty.Create(nameof(IconVariant), typeof(IconVariant), typeof(SymbolIcon), propertyChanged: OnSymbolPropertiesChanged);
    public static readonly BindableProperty UseSegoeMetricsProperty
        = BindableProperty.Create(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolImageSource), defaultValueCreator: _ => SymbolIcon.UseSegoeMetricsDefaultValue, propertyChanged: OnSymbolPropertiesChanged);
    public static readonly BindableProperty FlowDirectionProperty
        = BindableProperty.Create(nameof(UseSegoeMetrics), typeof(FlowDirection), typeof(SymbolImageSource), default, propertyChanged: OnSymbolPropertiesChanged);

    private string _glyph;

    public SymbolImageSource()
    {
        InvalidateText();
    }

    public Symbol Symbol
    {
        get => (Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public IconVariant IconVariant
    {
        get => (IconVariant)GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }

    public bool UseSegoeMetrics
    {
        get => (bool)GetValue(UseSegoeMetricsProperty);
        set => SetValue(UseSegoeMetricsProperty, value);
    }

    [TypeConverter(typeof(FlowDirectionConverter))]
    public FlowDirection FlowDirection
    {
        get => (FlowDirection)GetValue(FlowDirectionProperty);
        set => SetValue(FlowDirectionProperty, value);
    }

    [Obsolete("Deprecated in favour of IconVariant")]
    public bool IsFilled
    {
        get => IconVariant == IconVariant.Filled;
        set => IconVariant = value ? IconVariant.Filled : IconVariant.Regular;
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        switch(propertyName)
        {
            case nameof(FontFamily):
                FontFamily = UseSegoeMetrics ? "SeagullFluentIcons" : "FluentSystemIcons";
                break;
            case nameof(Glyph):
                Glyph = _glyph;
                break;
            default:
                base.OnPropertyChanged(propertyName);
                break;
        }
    }

    public static void OnSymbolPropertiesChanged(BindableObject bindable, object oldValue, object newValue)
        => (bindable as SymbolImageSource)?.InvalidateText();

    [MemberNotNull(nameof(_glyph))]
    private void InvalidateText()
    {
        FontFamily = UseSegoeMetrics ? "SeagullFluentIcons" : "FluentSystemIcons";
        Glyph = _glyph = Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    }
}

public class SymbolImageSourceExtension : IMarkupExtension<SymbolImageSource>
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? Size { get; set; }
    public Color? Color { get; set; }

    public SymbolImageSource ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolImageSource();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
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
