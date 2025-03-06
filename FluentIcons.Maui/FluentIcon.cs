using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui.Controls;

namespace FluentIcons.Maui;

public partial class FluentIcon : GenericIcon
{
    internal static string GetFontFamily(IconSize size, IconVariant variant) => size switch
    {
        IconSize.Resizable when variant != IconVariant.Light => "FluentSystemIconsSize20",
        IconSize.Resizable => "FluentSystemIconsSize32",
        _ => $"FluentSystemIcons{size}",
    };

    public static readonly BindableProperty IconProperty
        = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(FluentIcon), propertyChanged: OnIconPropertiesChanged);
    public static readonly BindableProperty IconSizeProperty
        = BindableProperty.Create(nameof(IconSize), typeof(IconSize), typeof(FluentIcon), default(IconSize));

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
    protected override string IconFont => GetFontFamily(IconSize, IconVariant);
}
