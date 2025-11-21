using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Avalonia.Fluent.Internals;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(GenericIconSourceConverter<Symbol, SymbolIconSource>))]
public class SymbolIconSource : GenericIconSource, IValue<Symbol>
{
    public static TypeConverter Converter { get; } = new GenericIconSourceConverter<Symbol, SymbolIconSource>();

    public Symbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
    public static readonly StyledProperty<Symbol> SymbolProperty
        = SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();

    Symbol IValue<Symbol>.Value
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
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
