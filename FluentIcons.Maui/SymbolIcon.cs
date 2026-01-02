using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui.Controls;

namespace FluentIcons.Maui;

public partial class SymbolIcon : GenericIcon
{
    public SymbolIcon()
    {
        InvalidateText();
    }

    public Symbol Symbol
    {
        get => (Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
    public static readonly BindableProperty SymbolProperty
        = BindableProperty.Create(
            nameof(Symbol),
            typeof(Symbol),
            typeof(SymbolIcon),
            Symbol.Home,
            propertyChanged: OnCorePropertiesChanged);

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == Microsoft.Maui.FlowDirection.RightToLeft);
    protected override string IconFont => FontManager.GetSeagull();
}
