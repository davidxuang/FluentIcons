#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI;
#else
namespace FluentIcons.Uwp;
#endif

public partial class FluentIconSource : GenericIconSource
{
    public FluentIconSource()
    {
        InvalidateText();
    }

    public Icon Icon
    {
        get { return (Icon)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    public static DependencyProperty IconProperty { get; }
        = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(FluentIconSource), new(Icon.Home, OnCorePropertyChanged));

    public IconSize IconSize
    {
        get { return (IconSize)GetValue(IconSizeProperty); }
        set { SetValue(IconSizeProperty, value); }
    }
    public static DependencyProperty IconSizeProperty { get; }
        = DependencyProperty.Register(nameof(IconSize), typeof(IconSize), typeof(FluentIconSource), new(default(IconSize), OnCorePropertyChanged));

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => FontManager.GetFluent(IconSize, IconVariant);

#if WINDOWS_WINAPPSDK || HAS_UNO
#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
    protected override IconElement CreateIconElementCore()
#else
    public override IconElement CreateIconElement()
#endif
    {
        var icon = new FluentIcon
        {
            Icon = Icon,
            IconSize = IconSize,
            IconVariant = IconVariant,
            FlowDirection = FlowDirection,
            FontSize = FontSize,
            IsTextScaleFactorEnabled = IsTextScaleFactorEnabled,
            MirroredWhenRightToLeft = MirroredWhenRightToLeft
        };
        if (Foreground != null) icon.Foreground = Foreground;
        return icon;
    }
#endif

#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
    protected override DependencyProperty GetIconElementPropertyCore(DependencyProperty dp)
    {
        if (dp == IconProperty)
        {
            return FluentIcon.IconProperty;
        }
        else if (dp == IconSizeProperty)
        {
            return FluentIcon.IconSizeProperty;
        }

        return base.GetIconElementPropertyCore(dp);
    }
#endif
}
