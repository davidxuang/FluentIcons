using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia.Fluent.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericIconSource : FontIconSource
{
    static GenericIconSource()
    {
        IconVariantProperty.Changed.AddClassHandler<GenericIconSource>(OnCorePropertyChanged);
        FontSizeProperty.Changed.AddClassHandler<GenericIconSource>(OnCorePropertyChanged);
        FlowDirectionProperty.Changed.AddClassHandler<GenericIconSource>(OnCorePropertyChanged);

        GlyphProperty.OverrideMetadata<GenericIconSource>(
            new(coerce: static (o, v) => (o as GenericIconSource)?.IconText ?? v));
        FontIconSource.FontSizeProperty.OverrideMetadata<GenericIconSource>(
            new(coerce: static (o, v) => (o as GenericIconSource)?.FontSize ?? v));
        FontFamilyProperty.OverrideMetadata<GenericIconSource>(
            new(coerce: static (o, v) => (o as GenericIconSource)?.IconFont.FontFamily ?? v));
        FontStyleProperty.OverrideMetadata<GenericIconSource>(
            new(coerce: static (o, v) => FontStyle.Normal));
        FontWeightProperty.OverrideMetadata<GenericIconSource>(
            new(coerce: static (o, v) => FontWeight.Regular));
    }

    public GenericIconSource()
    {
        base.FontSize = FontSize;
        FontStyle = FontStyle.Normal;
        FontWeight = FontWeight.Regular;
    }

    public IconVariant IconVariant
    {
        get => GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }
    public static readonly StyledProperty<IconVariant> IconVariantProperty
        = GenericIcon.IconVariantProperty.AddOwner<GenericIconSource>();

    public FlowDirection FlowDirection
    {
        get => GetValue(FlowDirectionProperty);
        set => SetValue(FlowDirectionProperty, value);
    }
    public static readonly StyledProperty<FlowDirection> FlowDirectionProperty
        = Visual.FlowDirectionProperty.AddOwner<GenericIconSource>();

    public new double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    public static new readonly StyledProperty<double> FontSizeProperty
        = GenericIcon.FontSizeProperty.AddOwner<GenericIconSource>();

    protected abstract string IconText { get; }
    protected abstract Typeface IconFont { get; }

    protected static void OnCorePropertyChanged(GenericIconSource element, AvaloniaPropertyChangedEventArgs? _)
    {
        element.InvalidateText();
    }

    protected void InvalidateText()
    {
        Glyph = IconText;
        FontFamily = IconFont.FontFamily;
    }
}

public class GenericIconSourceConverter<V, T> : TypeConverter
    where V : struct, Enum
    where T : GenericIconSource, IValue<V>, new()
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
