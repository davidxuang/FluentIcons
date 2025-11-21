using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Uwp.Internals;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace FluentIcons.Uwp;

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
        = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(FluentIconSource), new(Icon.Home, OnIconPropertiesChanged));

    public IconSize IconSize
    {
        get { return (IconSize)GetValue(IconSizeProperty); }
        set { SetValue(IconSizeProperty, value); }
    }
    public static DependencyProperty IconSizeProperty { get; }
        = DependencyProperty.Register(nameof(IconSize), typeof(IconSize), typeof(FluentIconSource), new(default(IconSize), OnIconPropertiesChanged));

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => FluentIcon.GetFontFamily(IconSize, IconVariant);
}

[MarkupExtensionReturnType(ReturnType = typeof(FluentIconSource))]
public partial class FluentIconSourceExtension : MarkupExtension
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
#if !HAS_UNO
    public FlowDirection? FlowDirection { get; set; }
#endif
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

#if !HAS_UNO
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

#if !HAS_UNO
        if (FlowDirection.HasValue) icon.FlowDirection = FlowDirection.Value;
#else
        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.FlowDirection = source.FlowDirection;
            source.RegisterPropertyChangedCallback(FrameworkElement.FlowDirectionProperty, (obj, args)
            => {
                if (obj is FrameworkElement f) icon.FlowDirection = f.FlowDirection;
            });
        }
#endif

        return icon;
    }
}
