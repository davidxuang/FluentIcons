using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentIcons.Common;

#if FLUENT_AVALONIA
namespace FluentIcons.Avalonia.Fluent.Markup;
#else
namespace FluentIcons.Avalonia.Markup;
#endif

public sealed class SymbolIconExtension
{
    public SymbolIconExtension() { }
    public SymbolIconExtension(Symbol symbol)
    {
        Symbol = symbol;
    }

    [ConstructorArgument("symbol")]
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is Visual source)
        {
            icon.Bind(Visual.FlowDirectionProperty, source.GetBindingObservable(Visual.FlowDirectionProperty));
        }

        return icon;
    }
}
