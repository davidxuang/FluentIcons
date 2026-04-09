#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI.Markup;
#else
namespace FluentIcons.Uwp.Markup;
#endif

[MarkupExtensionReturnType(ReturnType = typeof(FluentIconSource))]
public sealed partial class FluentIconSourceExtension : MarkupExtension
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    protected override object ProvideValue()
    {
        var icon = new FluentIconSource();

        if (Icon.HasValue) icon.Icon = Icon.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) icon.IconSize = IconSize.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        return icon;
    }
}
