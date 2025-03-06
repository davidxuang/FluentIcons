using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia.Fluent;

[TypeConverter(typeof(Internals.GenericIconSourceConverter<Icon, FluentIconSource>))]
public class FluentIconSource : Internals.GenericIconSource, IValue<Icon>
{
    public static readonly StyledProperty<Icon> IconProperty
        = FluentIcon.IconProperty.AddOwner<FluentIconSource>();
    public static readonly StyledProperty<IconSize> IconSizeProperty
        = FluentIcon.IconSizeProperty.AddOwner<FluentIconSource>();

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
        if (change.Property == IconProperty ||
            change.Property == IconVariantProperty ||
            change.Property == IconSizeProperty ||
            change.Property == FontSizeProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }
        else
        {
            base.OnPropertyChanged(change);
        }
    }
}
