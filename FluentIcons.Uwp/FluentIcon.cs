using System.Collections.Generic;
using System.Linq;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Uwp.Internals;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace FluentIcons.Uwp;

public partial class FluentIcon : GenericIcon
{
    private static readonly Dictionary<IconSize, FontFamily> _fontfamilies = IconSizeValues.Enumerable
        .Where(size => (byte)size > 0)
        .ToDictionary(k => k, k => new FontFamily($"ms-appx:///{AssetsNamespace}/Assets/FluentSystemIcons-{k}.ttf#Fluent System Icons {k}"));

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

#if !HAS_UNO
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

[MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
public partial class FluentIconExtension : MarkupExtension
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

#if !HAS_UNO
    protected override object ProvideValue()
#else
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
#endif
    {
        var icon = new FluentIcon(
#if !HAS_UNO
            true
#endif
            );

        if (Icon.HasValue) icon.Icon = Icon.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) icon.IconSize = IconSize.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

#if HAS_UNO
        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.SetBinding(
                FrameworkElement.FlowDirectionProperty,
                new Binding { Source = source, Path = new PropertyPath(nameof(source.FlowDirection)) });
        }
#endif

        return icon;
    }
}