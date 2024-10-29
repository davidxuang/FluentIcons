using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia;

[TypeConverter(typeof(SymbolImageConverter))]
public class SymbolImage : AvaloniaObject, IDisposable, IImage
{
    public static readonly StyledProperty<Symbol> SymbolProperty =
        SymbolIcon.SymbolProperty.AddOwner<SymbolImage>();
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<SymbolImage>();
    public static readonly StyledProperty<IconVariant> IconVariantProperty =
        SymbolIcon.IconVariantProperty.AddOwner<SymbolImage>();
    public static readonly StyledProperty<bool> UseSegoeMetricsProperty =
        SymbolIcon.UseSegoeMetricsProperty.AddOwner<SymbolImage>();
    public static readonly StyledProperty<FlowDirection> FlowDirectionProperty =
        Visual.FlowDirectionProperty.AddOwner<SymbolImage>();
    public static readonly StyledProperty<double> FontSizeProperty =
        SymbolIcon.FontSizeProperty.AddOwner<SymbolImage>();

    private TextLayout? _textLayout;

    public Symbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
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

    public FlowDirection FlowDirection
    {
        get => GetValue(FlowDirectionProperty);
        set => SetValue(FlowDirectionProperty, value);
    }

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public Size Size => new(FontSize, FontSize);

    public event EventHandler? VisualChanged;

    public void Dispose()
    {
        _textLayout?.Dispose();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == FontSizeProperty ||
            change.Property == ForegroundProperty ||
            change.Property == SymbolProperty ||
            change.Property == IconVariantProperty ||
            change.Property == UseSegoeMetricsProperty ||
            change.Property == FlowDirectionProperty)
        {
            _textLayout?.Dispose();
            _textLayout = new TextLayout(
                Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft),
                UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System,
                FontSize,
                Foreground,
                TextAlignment.Center,
                flowDirection: FlowDirection
            );

            VisualChanged?.Invoke(this, EventArgs.Empty);
        }

        base.OnPropertyChanged(change);
    }

    public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
    {
        if (_textLayout is null)
            return;

        var scale = Matrix.CreateScale(
            destRect.Width / sourceRect.Width,
            destRect.Height / sourceRect.Height);
        var translate = Matrix.CreateTranslation(
            -sourceRect.X + destRect.X,
            -sourceRect.Y + destRect.Y);
        var transform = translate * scale;
        if (FlowDirection == FlowDirection.RightToLeft)
            transform *= new Matrix(-1, 0, 0, 1, FontSize, 0);

        using (context.PushClip(destRect))
        using (context.PushTransform(transform))
        {
            _textLayout.Draw(context, new Point(0, 0));
        }
    }
}

public class SymbolImageConverter : TypeConverter
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
            return new SymbolImage { Symbol = (Symbol)Enum.Parse(typeof(Symbol), val) };
        }
        else if (value is Symbol symbol)
        {
            return new SymbolImage { Symbol = symbol };
        }
        return base.ConvertFrom(context, culture, value);
    }
}
