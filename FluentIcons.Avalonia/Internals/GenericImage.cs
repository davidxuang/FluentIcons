using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericImage : AvaloniaObject, IDisposable, IImage
{
    private TextLayout? _textLayout;

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }
    public static readonly StyledProperty<IBrush?> ForegroundProperty
        = TextElement.ForegroundProperty.AddOwner<GenericImage>();

    public IconVariant IconVariant
    {
        get => GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }
    public static readonly StyledProperty<IconVariant> IconVariantProperty
        = GenericIcon.IconVariantProperty.AddOwner<GenericImage>();

    public FlowDirection FlowDirection
    {
        get => GetValue(FlowDirectionProperty);
        set => SetValue(FlowDirectionProperty, value);
    }
    public static readonly StyledProperty<FlowDirection> FlowDirectionProperty
        = Visual.FlowDirectionProperty.AddOwner<GenericImage>();

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    public static readonly StyledProperty<double> FontSizeProperty
        = GenericIcon.FontSizeProperty.AddOwner<GenericImage>();

    protected abstract string IconText { get; }
    protected abstract Typeface IconFont { get; }

    public Size Size => new(FontSize, FontSize);

    public event EventHandler? VisualChanged;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _textLayout?.Dispose();
            _textLayout = null;
        }
    }

    protected void InvalidateText()
    {
        _textLayout?.Dispose();
        _textLayout = new TextLayout(
            IconText,
            IconFont,
            FontSize,
            Foreground,
            TextAlignment.Center,
            flowDirection: FlowDirection
        );

        VisualChanged?.Invoke(this, EventArgs.Empty);
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

public class GenericImageConverter<V, T> : TypeConverter
    where V : struct, Enum
    where T : GenericImage, IValue<V>, new()
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
