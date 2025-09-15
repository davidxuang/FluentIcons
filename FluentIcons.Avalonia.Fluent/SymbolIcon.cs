using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Avalonia.Fluent.Internals;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(GenericIconConverter<Symbol, SymbolIcon>))]
public class SymbolIcon : GenericIcon, IValue<Symbol>
{
    public static TypeConverter Converter { get; } = new GenericIconConverter<Symbol, SymbolIcon>();

    public static readonly StyledProperty<Symbol> SymbolProperty
        = AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol), Symbol.Home);

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

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => Avalonia.SymbolIcon.STypeface;

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
