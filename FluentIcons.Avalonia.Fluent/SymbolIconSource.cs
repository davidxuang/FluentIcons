using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(SymbolIconSourceConverter))]
public class SymbolIconSource : FontIconSource
{
    public static readonly StyledProperty<Symbol> SymbolProperty = SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();
    public static readonly StyledProperty<bool> IsFilledProperty = SymbolIcon.IsFilledProperty.AddOwner<SymbolIconSource>();
    public static readonly StyledProperty<bool> UseSegoeMetricsProperty = SymbolIcon.UseSegoeMetricsProperty.AddOwner<SymbolIconSource>();
    public static readonly StyledProperty<FlowDirection> FlowDirectionProperty = Visual.FlowDirectionProperty.AddOwner<SymbolIconSource>();
    public static new readonly StyledProperty<double> FontSizeProperty = SymbolIcon.FontSizeProperty.AddOwner<SymbolIconSource>();

    private string _glyph;

    public SymbolIconSource()
    {
        UseSegoeMetrics = SymbolIcon.UseSegoeMetricsDefaultValue;
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

    public bool IsFilled
    {
        get => GetValue(IsFilledProperty);
        set => SetValue(IsFilledProperty, value);
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SymbolProperty ||
            change.Property == IsFilledProperty ||
            change.Property == UseSegoeMetricsProperty ||
            change.Property == FontSizeProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();
            return;
        }
        else if (change.Property == FontSizeProperty || change.Property == FontIconSource.FontSizeProperty)
        {
            base.FontSize = FontSize;
            return;
        }
        else if (change.Property == GlyphProperty)
        {
            Glyph = _glyph;
            return;
        }
        else if (change.Property == FontFamilyProperty)
        {
            FontFamily = UseSegoeMetrics ? SymbolIcon.Seagull.FontFamily : SymbolIcon.System.FontFamily;
            return;
        }
        else if (change.Property == FontStyleProperty)
        {
            FontStyle = FontStyle.Normal;
            return;
        }
        else if (change.Property == FontWeightProperty)
        {
            FontWeight = FontWeight.Regular;
            return;
        }

        base.OnPropertyChanged(change);
    }

    [MemberNotNull(nameof(_glyph))]
    private void InvalidateText()
    {
        Glyph = _glyph = Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft);
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

public class SymbolIconSourceExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public bool? IsFilled { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIconSource();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IsFilled.HasValue) icon.IsFilled = IsFilled.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is Visual elem)
        {
            icon.FlowDirection = elem.FlowDirection;
        }

        return icon;
    }
}
