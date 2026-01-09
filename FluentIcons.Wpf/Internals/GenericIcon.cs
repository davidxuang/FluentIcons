using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Wpf.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericIcon : FrameworkElement
{
    private readonly Grid _grid;
    private readonly Core _core;

    internal GenericIcon()
    {
        _grid = new();
        _grid.SetBinding(WidthProperty, new Binding(nameof(Width)) { Source = this });
        _grid.SetBinding(HeightProperty, new Binding(nameof(Height)) { Source = this });
        _grid.SetBinding(HorizontalAlignmentProperty, new Binding(nameof(HorizontalAlignment)) { Source = this });
        _grid.SetBinding(VerticalAlignmentProperty, new Binding(nameof(VerticalAlignment)) { Source = this });
        AddLogicalChild(_grid);
        AddVisualChild(_grid);

        _core = new(FontSize);
        _core.SetBinding(FlowDirectionProperty, new Binding(nameof(FlowDirection)) { Source = this });
        _grid.Children.Add(_core);

        Loaded += static (s, e) => (s as GenericIcon)?.InvalidateText();
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
        => index switch
        {
            0 => _grid,
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };

    public IconVariant IconVariant
    {
        get { return (IconVariant)GetValue(IconVariantProperty); }
        set { SetValue(IconVariantProperty, value); }
    }
    public static readonly DependencyProperty IconVariantProperty
        = DependencyProperty.Register(
            nameof(IconVariant),
            typeof(IconVariant),
            typeof(GenericIcon),
            new(default(IconVariant), OnCorePropertyChanged));

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }
    public static readonly DependencyProperty FontSizeProperty
        = TextBlock.FontSizeProperty.AddOwner(typeof(GenericIcon), new FrameworkPropertyMetadata(20d, OnCorePropertyChanged));

    public Brush Foreground
    {
        get { return (Brush)GetValue(ForegroundProperty); }
        set { SetValue(ForegroundProperty, value); }
    }
    public static readonly DependencyProperty ForegroundProperty
        = TextBlock.ForegroundProperty.AddOwner(typeof(GenericIcon), new FrameworkPropertyMetadata(OnCorePropertyChanged));

    protected abstract string IconText { get; }

    protected abstract Typeface IconFont { get; }

    protected override Size MeasureOverride(Size availableSize)
    {
        _grid.Measure(availableSize);
        return _grid.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _grid.Arrange(new Rect(new Point(0, 0), finalSize));
        return base.ArrangeOverride(finalSize);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }

        base.OnPropertyChanged(e);
    }

    protected static void OnCorePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GenericIcon element) element.InvalidateText();
    }

    protected void InvalidateText()
        => _core.Update(IconText, IconFont, FontSize, Foreground);

    internal void AddHandIn(FrameworkElement element)
        => _grid.Children.Add(element);

    internal void RemoveHandIn(FrameworkElement element)
        => _grid.Children.Remove(element);

    internal sealed class Core(double size) : FrameworkElement
    {
        private bool _updating = false;

        private string? _text;
        private Typeface? _typeface;
        private double _size = size;
        private Brush? _foreground;

        private FormattedText? _formattedText;

        protected override Size MeasureOverride(Size availableSize)
            => new(Math.Min(_size, availableSize.Width),
                   Math.Min(_size, availableSize.Height));

        public void Update(string text, Typeface typeface, double fontSize, Brush? foreground)
        {
            if (_size != fontSize) InvalidateMeasure();
            _text = text;
            _typeface = typeface;
            _size = fontSize;
            _foreground = foreground;

            _updating = true;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext context)
        {
            if (_updating)
            {
                _updating = false;
                _formattedText = new FormattedText(
                    _text!,
                    CultureInfo.CurrentCulture,
                    FlowDirection,
                    _typeface!,
                    _size,
                    _foreground,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);
            }
            else if (_formattedText is null)
            {
                return;
            }

            var canvas = new Rect(0, 0, ActualWidth, ActualHeight);
            context.PushClip(new RectangleGeometry(canvas));

            var flip = FlowDirection == FlowDirection.RightToLeft;
            if (flip) context.PushTransform(new MatrixTransform(-1, 0, 0, 1, canvas.Width, 0));
            var origin = new Point(
                (flip ? canvas.Width + _formattedText.Width : canvas.Width - _formattedText.Width) / 2,
                (canvas.Height - _formattedText.Height) / 2);
            context.DrawText(_formattedText, origin);
            if (flip) context.Pop();

            context.Pop();
        }
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
