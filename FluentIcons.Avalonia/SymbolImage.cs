using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Avalonia.Internals;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia;

[TypeConverter(typeof(GenericIconConverter<Symbol, SymbolIcon>))]
public class SymbolImage : GenericImage, IValue<Symbol>
{
    public static readonly StyledProperty<Symbol> SymbolProperty
        = SymbolIcon.SymbolProperty.AddOwner<SymbolImage>();
    [Obsolete(Extensions.Message)]
    public static readonly StyledProperty<bool> UseSegoeMetricsProperty
        = SymbolIcon.UseSegoeMetricsProperty.AddOwner<SymbolImage>();

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
    protected override Typeface IconFont => SymbolIcon.STypeface;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == FontSizeProperty ||
            change.Property == ForegroundProperty ||
            change.Property == SymbolProperty ||
            change.Property == IconVariantProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }

        base.OnPropertyChanged(change);
    }
}
