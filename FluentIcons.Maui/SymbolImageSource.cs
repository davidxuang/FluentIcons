using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace FluentIcons.Maui;

public partial class SymbolImageSource : GenericImageSource
{
    public SymbolImageSource()
    {
        InvalidateText();
    }

    public Symbol Symbol
    {
        get => (Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
    public static readonly BindableProperty SymbolProperty
        = BindableProperty.Create(nameof(Symbol), typeof(Symbol), typeof(SymbolImageSource), Symbol.Home, propertyChanged: OnIconPropertiesChanged);

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override string IconFont => "SeagullFluentIcons";
}
