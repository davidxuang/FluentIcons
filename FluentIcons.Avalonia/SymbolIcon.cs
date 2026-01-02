using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
#if FLUENT_AVALONIA
using FluentIcons.Avalonia.Fluent.Internals;
#else
using FluentIcons.Avalonia.Internals;
#endif
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Resources.Avalonia;

#if FLUENT_AVALONIA
namespace FluentIcons.Avalonia.Fluent;
#else
namespace FluentIcons.Avalonia;
#endif

[TypeConverter(typeof(GenericIconConverter<Symbol, SymbolIcon>))]
public class SymbolIcon : GenericIcon, IValue<Symbol>
{
    public static TypeConverter Converter { get; } = new GenericIconConverter<Symbol, SymbolIcon>();

    static SymbolIcon()
    {
        SymbolProperty.Changed.AddClassHandler<SymbolIcon>(OnCorePropertyChanged);
    }

    public Symbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
    public static readonly StyledProperty<Symbol> SymbolProperty
        = AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol), Symbol.Home);

    Symbol IValue<Symbol>.Value
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => TypefaceManager.GetSeagull();
}
