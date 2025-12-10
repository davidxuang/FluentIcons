using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Wpf.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericIcon : FrameworkElement
{
    private bool _suspendCreate = true;
    private FormattedText? _formattedText;

    public IconVariant IconVariant
    {
        get { return (IconVariant)GetValue(IconVariantProperty); }
        set { SetValue(IconVariantProperty, value); }
    }
    public static readonly DependencyProperty IconVariantProperty
        = DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(GenericIcon), new(default(IconVariant), OnIconPropertiesChanged));

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }
    public static readonly DependencyProperty FontSizeProperty
        = TextBlock.FontSizeProperty.AddOwner(
            typeof(GenericIcon),
            new FrameworkPropertyMetadata(
                20d,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnSizePropertiesChanged));

    public Brush Foreground
    {
        get { return (Brush)GetValue(ForegroundProperty); }
        set { SetValue(ForegroundProperty, value); }
    }
    public static readonly DependencyProperty ForegroundProperty
        = TextBlock.ForegroundProperty.AddOwner(typeof(GenericIcon), new FrameworkPropertyMetadata(OnIconPropertiesChanged));

    protected abstract string IconText { get; }
    protected abstract Typeface IconFont { get; }

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

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }

        base.OnPropertyChanged(e);
    }

    protected static void OnSizePropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GenericIcon icon)
        {
            icon.InvalidateMeasure();
            icon.InvalidateText();
        }
    }

    protected static void OnIconPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as GenericIcon)?.InvalidateText();
    }

    protected void InvalidateText()
    {
        if (_suspendCreate)
            return;

        _formattedText = new FormattedText(
            IconText,
            CultureInfo.CurrentCulture,
            FlowDirection,
            IconFont,
            FontSize,
            Foreground,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

        InvalidateVisual();
    }
}

public class GenericIconConverter<V, T> : TypeConverter
    where V : struct, Enum
    where T : GenericIcon, IValue<V>, new()
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string) || sourceType == typeof(V))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string name)
        {
            return new T {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER || NET5_0_OR_GREATER
                Value = Enum.Parse<V>(name)
#else
                Value = (V)Enum.Parse(typeof(V), name)
#endif
            };
        }
        else if (value is V val)
        {
            return new T { Value = val };
        }
        return base.ConvertFrom(context, culture, value);
    }
}
