using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(Internals.GenericIconConverter<Icon, FluentIcon>))]
public class FluentIcon : Internals.GenericIcon, IValue<Icon>
{
    public static readonly StyledProperty<Icon> IconProperty
        = AvaloniaProperty.Register<FluentIcon, Icon>(nameof(Icon), Icon.Home);
    public static readonly StyledProperty<IconSize> IconSizeProperty
        = AvaloniaProperty.Register<FluentIcon, IconSize>(nameof(IconSize), IconSize.Resizable);

    public Icon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

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

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => Avalonia.FluentIcon.GetTypeface(IconSize, IconVariant);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == FontSizeProperty)
        {
            InvalidateMeasure();
            InvalidateText();
        }
        else if (change.Property == ForegroundProperty ||
            change.Property == IconProperty ||
            change.Property == IconVariantProperty ||
            change.Property == IconSizeProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }

        base.OnPropertyChanged(change);
    }
}
