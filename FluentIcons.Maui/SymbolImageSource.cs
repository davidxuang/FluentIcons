using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

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
