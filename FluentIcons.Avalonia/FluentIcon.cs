using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using FluentIcons.Avalonia.Internals;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia;

[TypeConverter(typeof(GenericIconConverter<Icon, FluentIcon>))]
public class FluentIcon : GenericIcon, IValue<Icon>
{
    private static readonly Dictionary<IconSize, Typeface> _typefaces = IconSizeValues.Enumerable
        .Where(size => (byte)size > 0)
        .ToDictionary(k => k, k => new Typeface($"avares://FluentIcons.Avalonia/Assets#Fluent System Icons {k}"));

    internal static Typeface GetTypeface(IconSize size, IconVariant variant) => size switch
    {
        IconSize.Resizable when variant != IconVariant.Light => _typefaces[IconSize.Size20],
        IconSize.Resizable => _typefaces[IconSize.Size32],
        _ => _typefaces[size]
    };

    public static TypeConverter Converter { get; } = new GenericIconConverter<Icon, FluentIcon>();

    public static readonly StyledProperty<Icon> IconProperty
        = AvaloniaProperty.Register<FluentIcon, Icon>(nameof(Icon), Icon.Home);
    public static readonly StyledProperty<IconSize> IconSizeProperty
        = AvaloniaProperty.Register<FluentIcon, IconSize>(nameof(IconSize), default);

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
    protected override Typeface IconFont => GetTypeface(IconSize, IconVariant);

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
