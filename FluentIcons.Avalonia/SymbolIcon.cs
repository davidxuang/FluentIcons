using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia;

[TypeConverter(typeof(Internals.GenericIconConverter<Symbol, SymbolIcon>))]
public class SymbolIcon : Internals.GenericIcon, IValue<Symbol>
{
    internal static readonly Typeface STypeface = new("avares://FluentIcons.Avalonia/Assets#Seagull Fluent Icons");

    public static readonly StyledProperty<Symbol> SymbolProperty
        = AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol), Symbol.Home);
    [Obsolete(Extensions.Message)]
    public static readonly StyledProperty<bool> UseSegoeMetricsProperty
        = AvaloniaProperty.Register<SymbolIcon, bool>(nameof(UseSegoeMetrics));

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
    protected override Typeface IconFont => STypeface;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == FontSizeProperty)
        {
            InvalidateMeasure();
            InvalidateText();
        }
        else if (change.Property == ForegroundProperty ||
            change.Property == SymbolProperty ||
            change.Property == IconVariantProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }

        base.OnPropertyChanged(change);
    }
}
