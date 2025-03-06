using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui.Controls;

namespace FluentIcons.Maui;

public partial class FluentImageSource : GenericImageSource
{
    public static readonly BindableProperty IconProperty
        = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(FluentImageSource), Icon.AccessTime, propertyChanged: OnIconPropertiesChanged);
    public static readonly BindableProperty IconSizeProperty
        = BindableProperty.Create(nameof(IconSize), typeof(IconSize), typeof(FluentImageSource), default(IconSize));

    public Icon Icon
    {
        get => (Icon)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IconSize IconSize
    {
        get => (IconSize)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == Microsoft.Maui.FlowDirection.RightToLeft);
    protected override string IconFont => FluentIcon.GetFontFamily(IconSize, IconVariant);
}
