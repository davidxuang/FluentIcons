using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;

namespace FluentIcons.Maui;

public class SymbolIcon : ContentView
{
    internal static bool UseSegoeMetricsDefaultValue = false;

    public static readonly BindableProperty SymbolProperty
        = BindableProperty.Create(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), Symbol.Home, propertyChanged: OnSymbolPropertiesChanged);
    public static readonly BindableProperty IconVariantProperty
        = BindableProperty.Create(nameof(IconVariant), typeof(IconVariant), typeof(SymbolIcon), propertyChanged: OnSymbolPropertiesChanged);
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
        _span.Text = Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    }
}

public class SymbolIconExtension : IMarkupExtension<SymbolIcon>
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Color? ForegroundColor { get; set; }

    public SymbolIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
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
