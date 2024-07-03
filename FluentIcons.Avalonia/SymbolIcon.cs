using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia;

[TypeConverter(typeof(SymbolIconConverter))]
public class SymbolIcon : IconElement
{
    private static readonly Typeface _system = new("avares://FluentIcons.Avalonia/Assets#Fluent System Icons");
    private static readonly Typeface _seagull = new("avares://FluentIcons.Avalonia/Assets#Seagull Fluent Icons");

    public static readonly StyledProperty<Symbol> SymbolProperty =
        AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol), Symbol.Home);
    public static readonly StyledProperty<IconVariant> IconVariantProperty =
        AvaloniaProperty.Register<SymbolIcon, IconVariant>(nameof(IconVariant));
    public static readonly StyledProperty<bool> UseSegoeMetricsProperty =
        AvaloniaProperty.Register<SymbolIcon, bool>(nameof(UseSegoeMetrics));
    public static new readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<SymbolIcon, double>(nameof(FontSize), 20d, false);

    [Obsolete("Deprecated in favour of IconVariant")]
    public static readonly DirectProperty<SymbolIcon, bool> IsFilledProperty =
        AvaloniaProperty.RegisterDirect<SymbolIcon, bool>(nameof(IsFilled), o => o.IsFilled, (o, v) => o.IsFilled = v);

    private readonly Border _border;
    private readonly Core _core;

    public SymbolIcon()
    {
        _border = new();
        _border.Bind(BackgroundProperty, this.GetBindingObservable(BackgroundProperty));
        _border.Bind(BorderBrushProperty, this.GetBindingObservable(BorderBrushProperty));
        _border.Bind(BorderThicknessProperty, this.GetBindingObservable(BorderThicknessProperty));
        _border.Bind(CornerRadiusProperty, this.GetBindingObservable(CornerRadiusProperty));
        _border.Bind(PaddingProperty, this.GetBindingObservable(PaddingProperty));
        (_border as ISetLogicalParent).SetParent(this);
        VisualChildren.Add(_border);
        LogicalChildren.Add(_border);

        _core = new();
        _core.Bind(FlowDirectionProperty, this.GetBindingObservable(FlowDirectionProperty));
        (_core as ISetLogicalParent).SetParent(this);
        VisualChildren.Add(_core);
        LogicalChildren.Add(_core);
    }

    public Symbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public IconVariant IconVariant
    {
        get => GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }

    public bool UseSegoeMetrics
    {
        get => GetValue(UseSegoeMetricsProperty);
        set => SetValue(UseSegoeMetricsProperty, value);
    }

    public new double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    [Obsolete("Deprecated in favour of IconVariant")]
    public bool IsFilled
    {
        get => IconVariant == IconVariant.Filled;
        set => IconVariant = value ? IconVariant.Filled : IconVariant.Regular;
    }

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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == FontSizeProperty)
        {
            InvalidateMeasure();
            InvalidateText();
        }
        else if (change.Property == ForegroundProperty ||
            change.Property == SymbolProperty ||
            change.Property == IconVariantProperty ||
            change.Property == UseSegoeMetricsProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();

            if (change.Property == IconVariantProperty)
            {
                switch (change.NewValue)
                {
#pragma warning disable CS0618
                    case IconVariant.Regular:
                        RaisePropertyChanged(IsFilledProperty, true, false);
                        break;
                    case IconVariant.Filled:
                        RaisePropertyChanged(IsFilledProperty, false, true);
                        break;
#pragma warning restore CS0618
                    default:
                        break;
                }
            }
        }

        base.OnPropertyChanged(change);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double fs = FontSize;
        Size size = new Size(fs, fs).Inflate(Padding).Inflate(BorderThickness);
        return new Size(
            Width.Or(
                HorizontalAlignment == HorizontalAlignment.Stretch
                    ? availableSize.Width.Or(size.Width)
                    : Math.Min(availableSize.Width, size.Width)),
            Height.Or(
                VerticalAlignment == VerticalAlignment.Stretch
                    ? availableSize.Height.Or(size.Height)
                    : Math.Min(availableSize.Height, size.Height)));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double fs = FontSize;
        Size size = new Size(fs, fs).Inflate(Padding).Inflate(BorderThickness);
        Rect rect = new(
            HorizontalAlignment switch
            {
                HorizontalAlignment.Center => (finalSize.Width - fs) / 2,
                HorizontalAlignment.Right => finalSize.Width - fs,
                _ => 0
            },
            VerticalAlignment switch
            {
                VerticalAlignment.Center => (finalSize.Height - fs) / 2,
                VerticalAlignment.Bottom => finalSize.Height - fs,
                _ => 0
            },
            HorizontalAlignment switch
            {
                HorizontalAlignment.Stretch => finalSize.Width,
                _ => size.Width,
            },
            VerticalAlignment switch
            {
                VerticalAlignment.Stretch => finalSize.Height,
                _ => size.Height,
            });
        _border.Arrange(rect);
        _core.Arrange(rect.Deflate(BorderThickness).Deflate(Padding));

        return finalSize;
    }

    private void InvalidateText()
    {
        if (!IsLoaded)
            return;

        _core.InvalidateText(
            Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft),
            UseSegoeMetrics ? _seagull : _system,
            FontSize,
            Foreground);
    }

    private sealed class Core : Control
    {
        private double _size;
        private TextLayout? _textLayout;

        public override void Render(DrawingContext context)
        {
            if (_textLayout is null)
                return;

            Rect bounds = Bounds;
            using (context.PushClip(new Rect(bounds.Size)))
            {
                IDisposable? disposable = null;
                if (FlowDirection == FlowDirection.RightToLeft)
                    disposable = context.PushTransform(new Matrix(-1, 0, 0, 1, bounds.Width, 0));
                var origin = new Point(
                    (bounds.Width - _size) / 2,
                    (bounds.Height - _size) / 2);
                _textLayout.Draw(context, origin);
                disposable?.Dispose();
            }
        }

        public void InvalidateText(string text, Typeface typeface, double fontSize, IBrush? foreground)
        {
            _size = fontSize;

            _textLayout?.Dispose();
            _textLayout = new TextLayout(
                text,
                typeface,
                fontSize,
                foreground,
                TextAlignment.Center,
                flowDirection: FlowDirection);

            InvalidateVisual();
        }

        public void Clear()
        {
            _textLayout?.Dispose();
            _textLayout = null;
        }
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
