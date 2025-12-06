using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia.Fluent.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericIcon : IconElement
{
    private TextLayout? _textLayout;

    public IconVariant IconVariant
    {
        get => GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }
    public static readonly StyledProperty<IconVariant> IconVariantProperty
        = AvaloniaProperty.Register<GenericIcon, IconVariant>(nameof(IconVariant));

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    public static readonly StyledProperty<double> FontSizeProperty
        = AvaloniaProperty.Register<GenericIcon, double>(nameof(FontSize), 20d, false);

    public FlowDirection FlowDirection
    {
        get => GetValue(FlowDirectionProperty);
        set => SetValue(FlowDirectionProperty, value);
    }
    public static readonly StyledProperty<FlowDirection> FlowDirectionProperty =
        AvaloniaProperty.Register<SymbolIcon, FlowDirection>(nameof(FlowDirection), FlowDirection.LeftToRight);

    protected abstract string IconText { get; }
    protected abstract Typeface IconFont { get; }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        InvalidateText();
        base.OnAttachedToVisualTree(e);
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
        using (context.PushPreTransform(Matrix.CreateTranslation(
            HorizontalAlignment switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Right => bounds.Width - size,
                _ => (bounds.Width - size) / 2,
            },
            VerticalAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Bottom => bounds.Height - size,
                _ => (bounds.Height - size) / 2,
            })))
        {
            _textLayout.Draw(context);
        }
    }

    protected void InvalidateText()
    {
        _textLayout = new TextLayout(
            IconText,
            IconFont,
            FontSize,
            Foreground,
            TextAlignment.Center);

        InvalidateVisual();
    }
}

public class GenericIconConverter<V, T> : TypeConverter
    where V : Enum
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
            return new T { Value = (V)Enum.Parse(typeof(V), name) };
        }
        else if (value is V val)
        {
            return new T { Value = val };
        }
        return base.ConvertFrom(context, culture, value);
    }
}

public enum FlowDirection
{
    LeftToRight,
    RightToLeft,
}
