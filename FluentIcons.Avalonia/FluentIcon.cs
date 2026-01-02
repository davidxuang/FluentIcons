using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
#if FLUENT_AVALONIA
using FluentIcons.Avalonia.Fluent.Internals;
#else
using FluentIcons.Avalonia.Internals;
#endif
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Resources.Avalonia;

#if FLUENT_AVALONIA
namespace FluentIcons.Avalonia.Fluent;
#else
namespace FluentIcons.Avalonia;
#endif

[TypeConverter(typeof(GenericIconConverter<Icon, FluentIcon>))]
public class FluentIcon : GenericIcon, IValue<Icon>
{
    public static TypeConverter Converter { get; } = new GenericIconConverter<Icon, FluentIcon>();

    static FluentIcon()
    {
        IconProperty.Changed.AddClassHandler<FluentIcon>(OnCorePropertyChanged);
        IconSizeProperty.Changed.AddClassHandler<FluentIcon>(OnCorePropertyChanged);
    }

    public Icon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public static readonly StyledProperty<Icon> IconProperty
        = AvaloniaProperty.Register<FluentIcon, Icon>(nameof(Icon), Icon.Home);

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
        = AvaloniaProperty.Register<FluentIcon, IconSize>(nameof(IconSize), default);

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => TypefaceManager.GetFluent(IconSize, IconVariant);
}
