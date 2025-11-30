using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Wpf.Internals;

namespace FluentIcons.Wpf;

[TypeConverter(typeof(GenericIconConverter<Icon, FluentIcon>))]

public class FluentIcon : GenericIcon, IValue<Icon>
{
    private static readonly Dictionary<IconSize, Typeface> _typefaces = IconSizeValues.Enumerable
        .Where(size => (byte)size > 0)
        .ToDictionary(k => k, k => new Typeface(
            new FontFamily(new Uri("pack://application:,,,/FluentIcons.Wpf;component/"), $"./Assets/#Fluent System Icons {k}"),
            FontStyles.Normal,
            FontWeights.Normal,
            FontStretches.Normal));

    public static TypeConverter Converter { get; } = new GenericIconConverter<Icon, FluentIcon>();

    public Icon Icon
    {
        get { return (Icon)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(FluentIcon), new(Icon.Home, OnIconPropertiesChanged));

    Icon IValue<Icon>.Value
    {
        get { return (Icon)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    public static readonly DependencyProperty IconSizeProperty
        = DependencyProperty.Register(nameof(IconSize), typeof(IconSize), typeof(FluentIcon), new(default(IconSize), OnIconPropertiesChanged));

    public IconSize IconSize
    {
        get { return (IconSize)GetValue(IconSizeProperty); }
        set { SetValue(IconSizeProperty, value); }
    }

    protected override string IconText => Icon.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => IconSize switch
    {
        IconSize.Resizable when IconVariant != IconVariant.Light => _typefaces[IconSize.Size20],
        IconSize.Resizable => _typefaces[IconSize.Size32],
        _ => _typefaces[IconSize]
    };
}
