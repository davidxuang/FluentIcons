using Avalonia;
using Avalonia.Controls;

namespace FluentIcons.Gallery.Controls;

public partial class FlexibleIcon : UserControl
{
    public FlexibleIcon()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<bool> UsesSymbolProperty =
        AvaloniaProperty.Register<FlexibleIcon, bool>(nameof(UsesSymbol), false);
    public static readonly StyledProperty<Icon> IconProperty =
        AvaloniaProperty.Register<FlexibleIcon, Icon>(nameof(Icon), Icon.Home);
    public static readonly StyledProperty<Symbol> SymbolProperty =
        AvaloniaProperty.Register<FlexibleIcon, Symbol>(nameof(Symbol), Symbol.Home);
    public static readonly StyledProperty<IconVariant> IconVariantProperty =
        AvaloniaProperty.Register<FlexibleIcon, IconVariant>(nameof(IconVariant), IconVariant.Regular);
    public static new readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<FlexibleIcon, double>(nameof(FontSize), 20.0);

    public bool UsesSymbol
    {
        get => GetValue(UsesSymbolProperty);
        set => SetValue(UsesSymbolProperty, value);
    }
    public Icon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public Symbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
    public IconVariant IconVariant
    {
        get => GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }
    public new double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
}