using System.ComponentModel;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Avalonia.Fluent.Internals;
using FluentIcons.Common.Internals;
using FluentIcons.Resources.Avalonia;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(GenericIconSourceConverter<Symbol, SymbolIconSource>))]
public class SymbolIconSource : GenericIconSource, IValue<Symbol>
{
    public static TypeConverter Converter { get; } = new GenericIconSourceConverter<Symbol, SymbolIconSource>();

    static SymbolIconSource()
    {
        FAIconHelpers.RegisterCustomIconSourceFactory(
            typeof(SymbolIconSource),
            static (s) =>
            {
                if (s is SymbolIconSource sis)
                {
                    var si = new SymbolIcon()
                    {
                        [!SymbolIcon.SymbolProperty] = sis[!SymbolProperty],
                        [!SymbolIcon.IconVariantProperty] = sis[!IconVariantProperty],
                        [!SymbolIcon.FontSizeProperty] = sis[!FontSizeProperty],
                    };

                    var observable = sis.GetBindingObservable(ForegroundProperty);
                    if (sis.IsSet(ForegroundProperty))
                    {
                        si.Bind(SymbolIcon.ForegroundProperty, observable, BindingPriority.LocalValue);
                    }
                    else
                    {
                        si.Bind(SymbolIcon.ForegroundProperty, observable.SkipOne(), BindingPriority.LocalValue);
                    }
                    return si;
                }
                return null;
            });
    }

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
    protected override Typeface IconFont => TypefaceManager.GetSeagull();
}
