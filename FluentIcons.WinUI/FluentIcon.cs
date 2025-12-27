#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI;
#else
namespace FluentIcons.Uwp;
#endif

public partial class FluentIcon : GenericIcon
{
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
    protected override FontFamily IconFont => FontManager.GetFluent(IconSize, IconVariant);
}
