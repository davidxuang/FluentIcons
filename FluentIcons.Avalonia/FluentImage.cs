using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Avalonia.Internals;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Resources.Avalonia;

namespace FluentIcons.Avalonia;

[TypeConverter(typeof(GenericImageConverter<Icon, FluentImage>))]
public class FluentImage : GenericImage, IValue<Icon>
{
    public static TypeConverter Converter { get; } = new GenericImageConverter<Icon, FluentImage>();

    static FluentImage()
    {
        IconProperty.Changed.AddClassHandler<FluentImage>(OnCorePropertyChanged);
        IconSizeProperty.Changed.AddClassHandler<FluentImage>(OnCorePropertyChanged);
    }

    public Icon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public static readonly StyledProperty<Icon> IconProperty
        = FluentIcon.IconProperty.AddOwner<FluentImage>();

    Icon IValue<Icon>.Value
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IconSize IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    public static readonly StyledProperty<IconSize> IconSizeProperty
        = FluentIcon.IconSizeProperty.AddOwner<FluentImage>();

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => TypefaceManager.GetFluent(IconSize, IconVariant);
}
