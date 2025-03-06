using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(Internals.GenericIconSourceConverter<Symbol, SymbolIconSource>))]
public class SymbolIconSource : Internals.GenericIconSource, IValue<Symbol>
{
    public static readonly StyledProperty<Symbol> SymbolProperty 
        = SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();
    [Obsolete(Extensions.Message)]
    public static readonly StyledProperty<bool> UseSegoeMetricsProperty 
        = SymbolIcon.UseSegoeMetricsProperty.AddOwner<SymbolIconSource>();

    public Symbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    Symbol IValue<Symbol>.Value
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    [Obsolete(Extensions.Message)]
    public bool UseSegoeMetrics
    {
        get => GetValue(UseSegoeMetricsProperty);
        set => SetValue(UseSegoeMetricsProperty, value);
    }

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => Avalonia.SymbolIcon.STypeface;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SymbolProperty ||
            change.Property == IconVariantProperty ||
            change.Property == FontSizeProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }
        else
        {
            base.OnPropertyChanged(change);
        }
    }
}
