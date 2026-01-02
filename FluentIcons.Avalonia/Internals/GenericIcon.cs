using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
#if FLUENT_AVALONIA
using FluentAvalonia.UI.Controls;
#endif
using FluentIcons.Common;
using FluentIcons.Common.Internals;

#if FLUENT_AVALONIA
namespace FluentIcons.Avalonia.Fluent.Internals;
#else
namespace FluentIcons.Avalonia.Internals;
#endif

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericIcon
#if FLUENT_AVALONIA
    : FAIconElement
#else
    : IconElement
#endif
{
    static GenericIcon()
    {
        IconVariantProperty.Changed.AddClassHandler<GenericIcon>(OnCorePropertyChanged);
        FontSizeProperty.Changed.AddClassHandler<GenericIcon>(OnCorePropertyChanged);
        ForegroundProperty.Changed.AddClassHandler<GenericIcon>(OnCorePropertyChanged);
        FlowDirectionProperty.Changed.AddClassHandler<GenericIcon>(OnCorePropertyChanged);
    }

    private readonly Panel _panel;
    private readonly Core _core;

    internal GenericIcon()
    {
        _panel = new();
        _panel.Bind(WidthProperty, this.GetBindingObservable(WidthProperty));
        _panel.Bind(HeightProperty, this.GetBindingObservable(HeightProperty));
        _panel.Bind(HorizontalAlignmentProperty, this.GetBindingObservable(HorizontalAlignmentProperty));
        _panel.Bind(VerticalAlignmentProperty, this.GetBindingObservable(VerticalAlignmentProperty));
#if !FLUENT_AVALONIA
        _panel.Bind(BackgroundProperty, this.GetBindingObservable(BackgroundProperty));
        _panel.Bind(BorderBrushProperty, this.GetBindingObservable(BorderBrushProperty));
        _panel.Bind(BorderThicknessProperty, this.GetBindingObservable(BorderThicknessProperty));
        _panel.Bind(CornerRadiusProperty, this.GetBindingObservable(CornerRadiusProperty));
        _panel.Bind(PaddingProperty, this.GetBindingObservable(PaddingProperty));
#endif
        (_panel as ISetLogicalParent).SetParent(this);
        VisualChildren.Add(_panel);
        LogicalChildren.Add(_panel);

        _core = new(FontSize);
        _core.Bind(FlowDirectionProperty, this.GetBindingObservable(FlowDirectionProperty));
        _panel.Children.Add(_core);
    }

    public IconVariant IconVariant
    {
        get => GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }
    public static readonly StyledProperty<IconVariant> IconVariantProperty
        = AvaloniaProperty.Register<GenericIcon, IconVariant>(nameof(IconVariant));

#if FLUENT_AVALONIA
    public double FontSize
#else
    public new double FontSize
#endif
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
#if FLUENT_AVALONIA
    public static readonly StyledProperty<double> FontSizeProperty
#else
    public static new readonly StyledProperty<double> FontSizeProperty
#endif
        = AvaloniaProperty.Register<GenericIcon, double>(nameof(FontSize), 20d, false);

    protected abstract string IconText { get; }
    protected abstract Typeface IconFont { get; }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        InvalidateText();
        base.OnLoaded(e);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _core.Clear();
        base.OnUnloaded(e);
    }

    protected static void OnCorePropertyChanged(GenericIcon element, AvaloniaPropertyChangedEventArgs? _)
    {
        element.InvalidateText();
    }

    protected void InvalidateText()
        => _core.Update(IconText, IconFont, FontSize, Foreground);

    internal sealed class Core(double size) : Control
    {
        private bool _updating = false;

        private string? _text;
        private Typeface _typeface;
        private double _size = size;
        private IBrush? _foreground;

        private TextLayout? _textLayout;

        protected override Size MeasureOverride(Size availableSize)
            => new(Math.Min(_size, availableSize.Width),
                   Math.Min(_size, availableSize.Height));

        public void Update(string text, Typeface typeface, double fontSize, IBrush? foreground)
        {
            if (_size != fontSize) InvalidateMeasure();
            _text = text;
            _typeface = typeface;
            _size = fontSize;
            _foreground = foreground;

            _updating = true;
            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            if (_updating || _textLayout is null)
            {
                _updating = false;
                _textLayout?.Dispose();
                _textLayout = new TextLayout(
                    _text,
                    _typeface,
                    _size,
                    _foreground,
                    TextAlignment.Center,
                    flowDirection: FlowDirection);
            }

            Rect bounds = Bounds;
            using (context.PushClip(new Rect(bounds.Size)))
            {
                IDisposable? flip = null;
                if (FlowDirection == FlowDirection.RightToLeft)
                    flip = context.PushTransform(new Matrix(-1, 0, 0, 1, bounds.Width, 0));
                var origin = new Point(
                    (bounds.Width - _size) / 2,
                    (bounds.Height - _size) / 2);
                _textLayout.Draw(context, origin);
                flip?.Dispose();
            }
        }

        public void Clear()
        {
            _textLayout?.Dispose();
            _textLayout = null;
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
            return new T
            {
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
