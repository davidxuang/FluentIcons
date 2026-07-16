using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace FluentIcons.Gallery.Controls;

public partial class ShowcaseIcon : UserControl
{
    public ShowcaseIcon()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<bool> UsesSymbolProperty =
        AvaloniaProperty.Register<ShowcaseIcon, bool>(nameof(UsesSymbol), false);
    public static readonly StyledProperty<Icon> IconProperty =
        AvaloniaProperty.Register<ShowcaseIcon, Icon>(nameof(Icon), (Icon)int.MaxValue);
    public static readonly StyledProperty<IconSize> IconSizeProperty =
        AvaloniaProperty.Register<ShowcaseIcon, IconSize>(nameof(IconSize), IconSize.Resizable);
    public static readonly StyledProperty<IconVariant> IconVariantProperty =
        AvaloniaProperty.Register<ShowcaseIcon, IconVariant>(nameof(IconVariant), IconVariant.Regular);
    public new static readonly StyledProperty<double?> FontSizeProperty =
        AvaloniaProperty.Register<ShowcaseIcon, double?>(nameof(FontSize));

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
    public IconSize IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    public IconVariant IconVariant
    {
        get => GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }
    public new double? FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static FuncMultiValueConverter<object?, double> FluentIconSizeConverter { get; } = new(values => values switch
    {
        [IconSize.Resizable, IconVariant.Light] => 32.0,
        [IconSize.Resizable, _] => 20.0,
        [IconSize size, _] => (byte)size,
        _ => double.NaN,
    });
    public static FuncValueConverter<Icon, Symbol> IconToSymbolConverter { get; } = new(icon => (Symbol)(int)icon);
}
