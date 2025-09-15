﻿using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Avalonia.Internals;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia;

[TypeConverter(typeof(GenericImageConverter<Icon, FluentImage>))]
public class FluentImage : GenericImage, IValue<Icon>
{
    public static TypeConverter Converter { get; } = new GenericImageConverter<Icon, FluentImage>();

    public static readonly StyledProperty<Icon> IconProperty
        = FluentIcon.IconProperty.AddOwner<FluentImage>();
    public static readonly StyledProperty<IconSize> IconSizeProperty
        = FluentIcon.IconSizeProperty.AddOwner<FluentImage>();

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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == FontSizeProperty ||
            change.Property == ForegroundProperty ||
            change.Property == IconProperty ||
            change.Property == IconVariantProperty ||
            change.Property == IconSizeProperty ||
            change.Property == FlowDirectionProperty)
        {
            InvalidateText();
        }

        base.OnPropertyChanged(change);
    }

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => FluentIcon.GetTypeface(IconSize, IconVariant);
}
