using System;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace FluentIcons.Maui;

public partial class FluentIcon : GenericIcon
{
    internal static string GetFontFamily(IconSize size, IconVariant variant) => size switch
    {
        IconSize.Resizable when variant != IconVariant.Light => "FluentSystemIconsSize20",
        IconSize.Resizable => "FluentSystemIconsSize32",
        _ => $"FluentSystemIcons{size}",
    };

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
        = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(FluentIcon), propertyChanged: OnIconPropertiesChanged);

    public IconSize IconSize
    {
        get => (IconSize)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    public static readonly BindableProperty IconSizeProperty
        = BindableProperty.Create(nameof(IconSize), typeof(IconSize), typeof(FluentIcon), default(IconSize));

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == Microsoft.Maui.FlowDirection.RightToLeft);
    protected override string IconFont => GetFontFamily(IconSize, IconVariant);
}

public class FluentIconExtension : IMarkupExtension<FluentIcon>
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public double? FontSize { get; set; }
    public Color? ForegroundColor { get; set; }

    public FluentIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new FluentIcon();

        if (Icon.HasValue) icon.Icon = Icon.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) icon.IconSize = IconSize.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (ForegroundColor is not null) icon.ForegroundColor = ForegroundColor;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is VisualElement source)
        {
            icon.SetBinding(VisualElement.FlowDirectionProperty, new Binding(nameof(FlowDirection), source: source));
        }

        return icon;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
