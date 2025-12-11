using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Avalonia.Internals;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Resources.Avalonia;

namespace FluentIcons.Avalonia;

[TypeConverter(typeof(GenericImageConverter<Symbol, SymbolImage>))]
public class SymbolImage : GenericImage, IValue<Symbol>
{
    public static TypeConverter Converter { get; } = new GenericImageConverter<Symbol, SymbolImage>();

    public Symbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
    public static readonly StyledProperty<Symbol> SymbolProperty
        = SymbolIcon.SymbolProperty.AddOwner<SymbolImage>();

    Symbol IValue<Symbol>.Value
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => TypefaceManager.GetSeagull();

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
