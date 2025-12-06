using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Visuals.Media.Imaging;
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
        = TemplatedControl.ForegroundProperty.AddOwner<GenericImage>();

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
        = GenericIcon.FlowDirectionProperty.AddOwner<GenericImage>();

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
        GC.SuppressFinalize(this);
    }

    protected void InvalidateText()
    {
        _textLayout = new TextLayout(
            IconText,
            IconFont,
            FontSize,
            Foreground,
            TextAlignment.Center
        );

        VisualChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Draw(DrawingContext context, Rect sourceRect, Rect destRect, BitmapInterpolationMode bitmapInterpolationMode)
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

        using (context.PushClip(destRect))
        using (context.PushPreTransform(transform))
        {
            _textLayout.Draw(context);
        }
    }
}

public class GenericImageConverter<V, T> : TypeConverter
    where V : Enum
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
            return new T { Value = (V)Enum.Parse(typeof(V), name) };
        }
        else if (value is V val)
        {
            return new T { Value = val };
        }
        return base.ConvertFrom(context, culture, value);
    }
}
