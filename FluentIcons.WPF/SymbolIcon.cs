using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.WPF;

public abstract class SymbolIconBase : FrameworkElement
{
    public static readonly DependencyProperty UseSegoeMetricsProperty =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIconBase), new PropertyMetadata(false, OnSizePropertiesChanged));

    public bool UseSegoeMetrics
    {
        get { return (bool)GetValue(UseSegoeMetricsProperty); }
        set { SetValue(UseSegoeMetricsProperty, value); }
    }

    protected abstract void InvalidateText();
    protected static void OnSizePropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SymbolIconBase icon)
        {
            icon.InvalidateMeasure();
            icon.InvalidateText();
        }
    }
}

[TypeConverter(typeof(SymbolIconConverter))]
public class SymbolIcon : SymbolIconBase
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

    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
    public static readonly DependencyProperty IconVariantProperty =
        DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(SymbolIcon), new PropertyMetadata(default(IconVariant), OnSymbolPropertiesChanged));
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

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }

    public IconVariant IconVariant
    {
        get { return (IconVariant)GetValue(IconVariantProperty); }
        set { SetValue(IconVariantProperty, value); }
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

        var hFlip = FlowDirection == FlowDirection.RightToLeft;
        if (hFlip) context.PushTransform(new MatrixTransform(-1, 0, 0, 1, ActualWidth, 0));
        var hOffset =  HorizontalAlignment switch
        {
            HorizontalAlignment.Left => 0,
            HorizontalAlignment.Right => canvas.Width - _formattedText.Width,
            _ => (canvas.Width - _formattedText.Width) / 2,
        };
        var origin = new Point(
            FlowDirection switch
            {
                FlowDirection.RightToLeft => canvas.Right - hOffset,
                _ => canvas.Left + hOffset,
            },
            canvas.Top + VerticalAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Bottom => canvas.Height - _formattedText.Height,
                _ => (canvas.Height - _formattedText.Height) / 2,
            });
        context.DrawText(_formattedText, origin);
        if (hFlip) context.Pop();
        context.Pop();
    }

    protected override void InvalidateText()
    {
        if (_suspendCreate)
            return;

        _formattedText = new FormattedText(
            Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft),
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

[MarkupExtensionReturnType(typeof(SymbolIcon))]
public class SymbolIconExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.SetBinding(
                FrameworkElement.FlowDirectionProperty,
                new Binding { Source = source, Path = new PropertyPath(nameof(source.FlowDirection)) });
        }

        return icon;
    }
}
