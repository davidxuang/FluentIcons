#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI;
#else
namespace FluentIcons.Uwp;
#endif

public partial class FluentIcon : GenericIcon
{
    private static readonly Dictionary<IconSize, FontFamily> _fontfamilies = IconSizeValues.Enumerable
        .Where(size => (byte)size > 0)
        .ToDictionary(k => k, k => new FontFamily($"ms-appx:///{AssetsAssembly}/Assets/FluentSystemIcons-{k}.ttf#Fluent System Icons {k}"));

    internal static FontFamily GetFontFamily(IconSize size, IconVariant variant) => size switch
    {
        IconSize.Resizable when variant != IconVariant.Light => _fontfamilies[IconSize.Size20],
        IconSize.Resizable => _fontfamilies[IconSize.Size32],
        _ => _fontfamilies[size]
    };

    public FluentIcon()
    {
        InvalidateText();
    }

#if WINDOWS_UWP
    internal FluentIcon(bool bindFlowDirection) : base(bindFlowDirection)
    {
        InvalidateText();
    }
#endif

    public Icon Icon
    {
        get { return (Icon)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    public static DependencyProperty IconProperty { get; }
        = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(FluentIcon), new(Icon.Home, OnIconPropertiesChanged));

    public IconSize IconSize
    {
        get { return (IconSize)GetValue(IconSizeProperty); }
        set { SetValue(IconSizeProperty, value); }
    }
    public static DependencyProperty IconSizeProperty { get; }
        = DependencyProperty.Register(nameof(IconSize), typeof(IconSize), typeof(FluentIcon), new(default(IconSize), OnIconPropertiesChanged));

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => GetFontFamily(IconSize, IconVariant);
}
