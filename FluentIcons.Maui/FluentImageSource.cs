using System;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

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

public class FluentImageSourceExtension : IMarkupExtension<FluentImageSource>
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public double? Size { get; set; }
    public Color? Color { get; set; }

    public FluentImageSource ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new FluentImageSource();

        if (Icon.HasValue) icon.Icon = Icon.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) icon.IconSize = IconSize.Value;
        if (Size.HasValue) icon.Size = Size.Value;
        if (Color is not null) icon.Color = Color;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is VisualElement source)
        {
            icon.FlowDirection = source.FlowDirection;
        }

        return icon;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
