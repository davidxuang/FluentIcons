using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(SymbolIconSourceConverter))]
public class SymbolIconSource : FontIconSource
{
    internal static bool UseSegoeMetricsDefaultValue = false;

    public static readonly StyledProperty<Symbol> SymbolProperty =
        SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();
    public static readonly StyledProperty<IconVariant> IconVariantProperty =
        SymbolIcon.IconVariantProperty.AddOwner<SymbolIconSource>();
    public static readonly StyledProperty<bool> UseSegoeMetricsProperty =
        SymbolIcon.UseSegoeMetricsProperty.AddOwner<SymbolIconSource>();
    public static readonly StyledProperty<FlowDirection> FlowDirectionProperty =
        Visual.FlowDirectionProperty.AddOwner<SymbolIconSource>();
    public static new readonly StyledProperty<double> FontSizeProperty =
        SymbolIcon.FontSizeProperty.AddOwner<SymbolIconSource>();

    private string _glyph;

    public SymbolIconSource()
    {
        UseSegoeMetrics = UseSegoeMetricsDefaultValue;
        base.FontSize = FontSize;
        FontFamily = UseSegoeMetrics ? SymbolIcon.Seagull.FontFamily : SymbolIcon.System.FontFamily;
        FontStyle = FontStyle.Normal;
        FontWeight = FontWeight.Regular;
        InvalidateText();
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

    public FlowDirection FlowDirection
    {
        get => GetValue(FlowDirectionProperty);
        set => SetValue(FlowDirectionProperty, value);
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SymbolProperty ||
            change.Property == IconVariantProperty ||
            change.Property == UseSegoeMetricsProperty ||
            change.Property == FontSizeProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }
        else if (change.Property == FontSizeProperty || change.Property == FontIconSource.FontSizeProperty)
        {
            base.FontSize = FontSize;
        }
        else if (change.Property == GlyphProperty)
        {
            Glyph = _glyph;
        }
        else if (change.Property == FontFamilyProperty)
        {
            FontFamily = UseSegoeMetrics ? SymbolIcon.Seagull.FontFamily : SymbolIcon.System.FontFamily;
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

    [MemberNotNull(nameof(_glyph))]
    private void InvalidateText()
    {
        Glyph = _glyph = Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    }
}

public class SymbolIconSourceConverter : TypeConverter
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
            return new SymbolIconSource { Symbol = (Symbol)Enum.Parse(typeof(Symbol), val) };
        }
        else if (value is Symbol symbol)
        {
            return new SymbolIconSource { Symbol = symbol };
        }
        return base.ConvertFrom(context, culture, value);
    }
}
