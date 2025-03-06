using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentIcons.Common;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;

namespace FluentIcons.Maui.Internals;

public abstract partial class GenericIcon : ContentView
{
    public static readonly BindableProperty IconVariantProperty
        = BindableProperty.Create(nameof(IconVariant), typeof(IconVariant), typeof(GenericIcon), propertyChanged: OnIconPropertiesChanged);
    public static readonly BindableProperty FontSizeProperty
        = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(GenericIcon), 20d);
    public static readonly BindableProperty ForegroundColorProperty
        = BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(GenericIcon), null);

    private readonly Label _label;
    private readonly Span _span;

    public GenericIcon()
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

    public IconVariant IconVariant
    {
        get => (IconVariant)GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
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

    protected abstract string IconText { get; }
    protected abstract string IconFont { get; }

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

    protected static void OnIconPropertiesChanged(BindableObject bindable, object oldValue, object newValue)
        => (bindable as GenericIcon)?.InvalidateText();

    private void InvalidateText()
    {
        _span.FontFamily = IconFont;
        _span.Text = IconText;
    }
}
