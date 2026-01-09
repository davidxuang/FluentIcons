using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentIcons.Common;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;

namespace FluentIcons.Maui.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract partial class GenericIcon : ContentView
{
    private readonly Grid _grid;
    private readonly Core _core;

    public GenericIcon()
    {
        Content = _grid = new();

        _core = new(FontSize);
        _core.SetBinding(FlowDirectionProperty, new Binding(nameof(FlowDirection), source: this));
        _grid.Children.Add(_core);

        Loaded += static (s, e) => (s as GenericIcon)?.InvalidateText();
    }

    public IconVariant IconVariant
    {
        get => (IconVariant)GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }
    public static readonly BindableProperty IconVariantProperty
        = BindableProperty.Create(
            nameof(IconVariant),
            typeof(IconVariant),
            typeof(GenericIcon),
            propertyChanged: OnCorePropertiesChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    public static readonly BindableProperty FontSizeProperty
        = BindableProperty.Create(
            nameof(FontSize),
            typeof(double),
            typeof(GenericIcon),
            20d,
            propertyChanged: OnCorePropertiesChanged);

    [TypeConverter(typeof(ColorTypeConverter))]
    public Color? ForegroundColor
    {
        get => (Color?)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }
    public static readonly BindableProperty ForegroundColorProperty
        = BindableProperty.Create(
            nameof(ForegroundColor),
            typeof(Color),
            typeof(GenericIcon),
            propertyChanged: OnCorePropertiesChanged);

    protected abstract string IconText { get; }
    protected abstract string IconFont { get; }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == nameof(FlowDirection))
        {
            InvalidateText();
        }
        else if (propertyName == nameof(Content))
        {
            Content = _grid;
        }

        base.OnPropertyChanged(propertyName);
    }

    protected static void OnCorePropertiesChanged(BindableObject bindable, object oldValue, object newValue)
        => (bindable as GenericIcon)?.InvalidateText();

    protected void InvalidateText()
        => _core.Update(IconText, IconFont, FontSize, ForegroundColor);

    internal void AddHandIn(VisualElement element)
        => _grid.Children.Add(element);

    internal bool RemoveHandIn(VisualElement element)
        => _grid.Children.Remove(element);

    internal sealed partial class Core : Label
    {
        private readonly Span _span;

        public Core(double size)
        {
            _span = new()
            {
                FontSize = size,
            };

            FormattedText = new();
            HorizontalTextAlignment = TextAlignment.Center;
            VerticalTextAlignment = TextAlignment.Center;
            FormattedText.Spans.Add(_span);
        }

        protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            var size = _span.FontSize;
            return new Size(Math.Min(size, widthConstraint),
                            Math.Min(size, heightConstraint));
        }

        public void Update(string text, string fontFamily, double fontSize, Color? foreground)
        {
            if (_span.FontSize != fontSize)
            {
                _span.FontSize = fontSize;
                InvalidateMeasure();
            }

            _span.FontFamily = fontFamily;
            _span.Text = text;
            _span.TextColor = foreground;
        }
    }
}
