using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.WPF;

[TypeConverter(typeof(SymbolIconConverter))]
public class SymbolIcon : FrameworkElement
{
    private static readonly Typeface _system = new(
        new FontFamily(new Uri("pack://application:,,,/FluentIcons.WPF;component/"), "./Assets/#Fluent System Icons"),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);
    private static readonly Typeface _seagull = new(
        new FontFamily(new Uri("pack://application:,,,/FluentIcons.WPF;component/"), "./Assets/#Seagull Fluent Icons"),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);
    internal static bool UseSegoeMetricsDefaultValue = false;

    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
    public static readonly DependencyProperty IsFilledProperty =
        DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
    public static readonly DependencyProperty UseSegoeMetricsProperty =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSizePropertiesChanged));
    public static readonly DependencyProperty FontSizeProperty =
        TextBlock.FontSizeProperty.AddOwner(
            typeof(SymbolIcon),
            new FrameworkPropertyMetadata(
                20d,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnSizePropertiesChanged));
    public static readonly DependencyProperty ForegroundProperty =
        TextBlock.ForegroundProperty.AddOwner(typeof(SymbolIcon), new FrameworkPropertyMetadata(OnSymbolPropertiesChanged));

    private bool _suspendCreate = true;
    private FormattedText? _formattedText;

    public SymbolIcon()
    {
        UseSegoeMetrics = UseSegoeMetricsDefaultValue;
    }

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }

    public bool IsFilled
    {
        get { return (bool)GetValue(IsFilledProperty); }
        set { SetValue(IsFilledProperty, value); }
    }

    public bool UseSegoeMetrics
    {
        get { return (bool)GetValue(UseSegoeMetricsProperty); }
        set { SetValue(UseSegoeMetricsProperty, value); }
    }

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public Brush Foreground
    {
        get { return (Brush)GetValue(ForegroundProperty); }
        set { SetValue(ForegroundProperty, value); }
    }

    private static void OnSizePropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SymbolIcon icon)
        {
            icon.InvalidateMeasure();
            icon.InvalidateText();
        }
    }

    private static void OnSymbolPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as SymbolIcon)?.InvalidateText();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }

        base.OnPropertyChanged(e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_suspendCreate || _formattedText is null)
        {
            _suspendCreate = false;
            InvalidateText();
        }

        double size = FontSize;
        return new Size(
            Width.Or(
                HorizontalAlignment == HorizontalAlignment.Stretch
                    ? availableSize.Width.Or(size)
                    : Math.Min(availableSize.Width, size)),
            Height.Or(
                VerticalAlignment == VerticalAlignment.Stretch
                    ? availableSize.Height.Or(size)
                    : Math.Min(availableSize.Height, size)));
    }

    protected override void OnRender(DrawingContext context)
    {
        if (_formattedText is null)
            return;

        var canvas = RenderTransform.TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));
        context.PushClip(new RectangleGeometry(canvas));
        var origin = new Point(
            canvas.Left + HorizontalAlignment switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Right => canvas.Width - _formattedText.Width,
                _ => (canvas.Width - _formattedText.Width) / 2,
            },
            canvas.Top + VerticalAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Bottom => canvas.Height - _formattedText.Height,
                _ => (canvas.Height - _formattedText.Height) / 2,
            });
        context.DrawText(_formattedText, origin);
        context.Pop();
    }

    private void InvalidateText()
    {
        if (_suspendCreate)
            return;

        _formattedText = new FormattedText(
            Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft),
            CultureInfo.CurrentCulture,
            FlowDirection,
            UseSegoeMetrics ? _seagull : _system,
            FontSize,
            Foreground,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

        InvalidateVisual();
    }
}

public class SymbolIconConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string) || sourceType == typeof(Symbol))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string val)
        {
            return new SymbolIcon { Symbol = (Symbol)Enum.Parse(typeof(Symbol), val) };
        }
        else if (value is Symbol symbol)
        {
            return new SymbolIcon { Symbol = symbol };
        }
        return base.ConvertFrom(context, culture, value);
    }
}
