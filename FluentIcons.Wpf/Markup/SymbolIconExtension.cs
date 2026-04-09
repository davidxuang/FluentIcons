using System;
using System.Windows.Markup;
using System.Windows.Media;
using FluentIcons.Common;

namespace FluentIcons.Wpf.Markup;

[MarkupExtensionReturnType(typeof(SymbolIcon))]
public sealed class SymbolIconExtension : MarkupExtension
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

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        return icon;
    }
}
