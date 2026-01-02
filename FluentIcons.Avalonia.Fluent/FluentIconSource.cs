using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Avalonia.Fluent.Internals;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Resources.Avalonia;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(GenericIconSourceConverter<Icon, FluentIconSource>))]
public class FluentIconSource : GenericIconSource, IValue<Icon>
{
    public static TypeConverter Converter { get; } = new GenericIconSourceConverter<Icon, FluentIconSource>();

    static FluentIconSource()
    {
        IconProperty.Changed.AddClassHandler<FluentIconSource>(OnCorePropertyChanged);
        IconSizeProperty.Changed.AddClassHandler<FluentIconSource>(OnCorePropertyChanged);
    }

    public FluentIconSource()
    {
        InvalidateText();
    }

    public Icon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public static readonly StyledProperty<Icon> IconProperty
        = FluentIcon.IconProperty.AddOwner<FluentIconSource>();

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
        = FluentIcon.IconSizeProperty.AddOwner<FluentIconSource>();

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => TypefaceManager.GetFluent(IconSize, IconVariant);
}
