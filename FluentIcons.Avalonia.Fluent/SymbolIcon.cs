using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(SymbolIconConverter))]
public partial class SymbolIcon : FAIconElement
{
    internal static readonly Typeface System = new("avares://FluentIcons.Avalonia/Assets#Fluent System Icons");
    internal static readonly Typeface Seagull = new("avares://FluentIcons.Avalonia/Assets#Seagull Fluent Icons");

    public static readonly StyledProperty<Symbol> SymbolProperty =
        AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol), Symbol.Home);
    public static readonly StyledProperty<IconVariant> IconVariantProperty =
        AvaloniaProperty.Register<SymbolIcon, IconVariant>(nameof(IconVariant));
    public static readonly StyledProperty<bool> UseSegoeMetricsProperty =
        AvaloniaProperty.Register<SymbolIcon, bool>(nameof(UseSegoeMetrics));
    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<SymbolIcon, double>(nameof(FontSize), 20d, false);

    private TextLayout? _textLayout;

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

    public double FontSize
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
        _textLayout?.Dispose();
        _textLayout = null;
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
        }

        base.OnPropertyChanged(change);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
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

    public override void Render(DrawingContext context)
    {
        if (_textLayout is null)
            return;

        double size = FontSize;
        Rect bounds = Bounds;
        using (context.PushClip(new Rect(bounds.Size)))
        {
            IDisposable? disposable = null;
            if (FlowDirection == FlowDirection.RightToLeft)
                disposable = context.PushTransform(new Matrix(-1, 0, 0, 1, bounds.Width, 0));
            var origin = new Point(
                HorizontalAlignment switch
                {
                    HorizontalAlignment.Left when FlowDirection != FlowDirection.RightToLeft => 0,
                    HorizontalAlignment.Right when FlowDirection == FlowDirection.RightToLeft => 0,
                    HorizontalAlignment.Left or HorizontalAlignment.Right => bounds.Width - size,
                    _ => (bounds.Width - size) / 2,
                },
                VerticalAlignment switch
                {
                    VerticalAlignment.Top => 0,
                    VerticalAlignment.Bottom => bounds.Height - size,
                    _ => (bounds.Height - size) / 2,
                });
            _textLayout.Draw(context, origin);
            disposable?.Dispose();
        }
    }

    private void InvalidateText()
    {
        if (!IsLoaded)
            return;

        _textLayout?.Dispose();
        _textLayout = new TextLayout(
            Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft),
            UseSegoeMetrics ? Seagull : System,
            FontSize,
            Foreground,
            TextAlignment.Center,
            flowDirection: FlowDirection);

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
