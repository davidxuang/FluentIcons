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
    public static readonly BindableProperty IsFilledProperty
        = BindableProperty.Create(nameof(IsFilled), typeof(bool), typeof(SymbolImageSource), false, propertyChanged: OnSymbolPropertiesChanged);
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

    public bool IsFilled
    {
        get => (bool)GetValue(IsFilledProperty);
        set => SetValue(IsFilledProperty, value);
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
        Glyph = _glyph = Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft);
    }
}

public class SymbolImageSourceExtension : IMarkupExtension<SymbolImageSource>
{
    public Symbol? Symbol { get; set; }
    public bool? IsFilled { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? Size { get; set; }
    public Color? Color { get; set; }

    public SymbolImageSource ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolImageSource();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IsFilled.HasValue) icon.IsFilled = IsFilled.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (Size.HasValue) icon.Size = Size.Value;
        if (Color is not null) icon.Color = Color;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is VisualElement elem)
        {
            icon.FlowDirection = elem.FlowDirection;
        }

        return icon;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
