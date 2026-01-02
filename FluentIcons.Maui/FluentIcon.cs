using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui.Controls;

namespace FluentIcons.Maui;

public partial class FluentIcon : GenericIcon
{
    public FluentIcon()
    {
        InvalidateText();
    }

    public Icon Icon
    {
        get => (Icon)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public static readonly BindableProperty IconProperty
        = BindableProperty.Create(
            nameof(Icon),
            typeof(Icon),
            typeof(FluentIcon),
            Icon.Home,
            propertyChanged: OnCorePropertiesChanged);

    public IconSize IconSize
    {
        get => (IconSize)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    public static readonly BindableProperty IconSizeProperty
        = BindableProperty.Create(
            nameof(IconSize),
            typeof(IconSize),
            typeof(FluentIcon),
            default(IconSize),
            propertyChanged: OnCorePropertiesChanged);

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == Microsoft.Maui.FlowDirection.RightToLeft);
    protected override string IconFont => FontManager.GetFluent(IconSize, IconVariant);
}
