#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI.Markup;
#else
namespace FluentIcons.Uwp.Markup;
#endif

[MarkupExtensionReturnType(ReturnType = typeof(SymbolIcon))]
public partial class SymbolIconExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

#if WINDOWS_UWP
    protected override object ProvideValue()
    {
        var icon = new SymbolIcon(true);
#else
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();
#endif

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

#if !WINDOWS_UWP
        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.SetBinding(
                FrameworkElement.FlowDirectionProperty,
                new Binding { Source = source, Path = new PropertyPath(nameof(source.FlowDirection)) });
        }
#endif

        return icon;
    }
}
