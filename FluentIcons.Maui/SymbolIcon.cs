using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;

namespace FluentIcons.Maui;

public class SymbolIcon : ContentView
{
    internal static bool UseSegoeMetricsDefaultValue = false;

    public static readonly BindableProperty SymbolProperty
        = BindableProperty.Create(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), Symbol.Home, propertyChanged: OnSymbolPropertiesChanged);
    public static readonly BindableProperty IsFilledProperty
        = BindableProperty.Create(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), false, propertyChanged: OnSymbolPropertiesChanged);
    public static readonly BindableProperty UseSegoeMetricsProperty
        = BindableProperty.Create(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), defaultValueCreator: _ => UseSegoeMetricsDefaultValue, propertyChanged: OnSymbolPropertiesChanged);
    public static readonly BindableProperty FontSizeProperty
        = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SymbolIcon), 20d);
    public static readonly BindableProperty ForegroundColorProperty
        = BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(SymbolIcon), null);

    private readonly Label _label;
    private readonly Span _span;

    public SymbolIcon()
    {
        _span = new();
        InvalidateText();
        _span.SetBinding(Span.FontSizeProperty, new Binding(nameof(FontSize), source: this));
        _span.SetBinding(Span.TextColorProperty, new Binding(nameof(ForegroundColor), source: this));

        _label = new Label()
        {
            FormattedText = new(),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
        };
        _label.FormattedText.Spans.Add(_span);
        Content = _label;
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

    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == nameof(FlowDirection))
        {
            InvalidateText();
        }

        base.OnPropertyChanged(propertyName);
    }

    protected override Size ArrangeOverride(Rect bounds)
    {
        var left = HorizontalOptions.Alignment switch
        {
            LayoutAlignment.Start => bounds.Left + Padding.Left,
            LayoutAlignment.End => bounds.Right - FontSize - Padding.Right,
            _ => bounds.Center.X - (FontSize + Padding.Right - Padding.Left) / 2
        };
        var top = VerticalOptions.Alignment switch
        {
            LayoutAlignment.Start => bounds.Top + Padding.Top,
            LayoutAlignment.End => bounds.Bottom - FontSize - Padding.Right,
            _ => bounds.Center.Y - (FontSize + Padding.Bottom - Padding.Top) / 2
        };
        _label.Arrange(new(new(left, top), new(FontSize, FontSize)));
        return base.ArrangeOverride(bounds);
    }

    public static void OnSymbolPropertiesChanged(BindableObject bindable, object oldValue, object newValue)
        => (bindable as SymbolIcon)?.InvalidateText();

    private void InvalidateText()
    {
        _span.FontFamily = UseSegoeMetrics ? "SeagullFluentIcons" : "FluentSystemIcons";
        _span.Text = Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft);
    }
}
