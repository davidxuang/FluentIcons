#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI.Markup;
#else
namespace FluentIcons.Uwp.Markup;
#endif

[MarkupExtensionReturnType(ReturnType = typeof(FluentIconSource))]
public partial class FluentIconSourceExtension : MarkupExtension
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
#if WINDOWS_UWP
    public FlowDirection? FlowDirection { get; set; }
#endif
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

#if WINDOWS_UWP
    protected override object ProvideValue()
#else
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
#endif
    {
        var icon = new FluentIconSource();

        if (Icon.HasValue) icon.Icon = Icon.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) icon.IconSize = IconSize.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

#if WINDOWS_UWP
        if (FlowDirection.HasValue) icon.FlowDirection = FlowDirection.Value;
#else
        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.FlowDirection = source.FlowDirection;
            source.RegisterPropertyChangedCallback(FrameworkElement.FlowDirectionProperty, (obj, args) => {
                if (obj is FrameworkElement f) icon.FlowDirection = f.FlowDirection;
            });
        }
#endif

        return icon;
    }
}
