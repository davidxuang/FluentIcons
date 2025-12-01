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
    public GenericIconSource()
    {
        base.FontSize = FontSize;
        FontFamily = IconFont.FontFamily;
        FontStyle = FontStyle.Normal;
        FontWeight = FontWeight.Regular;
        InvalidateText();
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == FontIconSource.FontSizeProperty)
        {
            base.FontSize = FontSize;
        }
        else if (change.Property == GlyphProperty)
        {
            Glyph = IconText;
        }
        else if (change.Property == FontFamilyProperty)
        {
            FontFamily = IconFont.FontFamily;
        }
        else if (change.Property == FontStyleProperty)
        {
            FontStyle = FontStyle.Normal;
        }
        else if (change.Property == FontWeightProperty)
        {
            FontWeight = FontWeight.Regular;
        }
        else
        {
            base.OnPropertyChanged(change);
        }
    }

    protected void InvalidateText()
    {
        Glyph = IconText;
    }
}

public class GenericIconSourceConverter<V, T> : TypeConverter
    where V : Enum
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
            return new T { Value = (V)Enum.Parse(typeof(V), name) };
        }
        else if (value is V val)
        {
            return new T { Value = val };
        }
        return base.ConvertFrom(context, culture, value);
    }
}
