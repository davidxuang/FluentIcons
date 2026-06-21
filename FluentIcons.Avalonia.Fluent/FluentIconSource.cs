using System.ComponentModel;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
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
        FAIconHelpers.RegisterCustomIconSourceFactory(
            typeof(FluentIconSource),
            static (s) =>
            {
                if (s is FluentIconSource fis)
                {
                    var fi = new FluentIcon()
                    {
                        [!FluentIcon.IconProperty] = fis[!IconProperty],
                        [!FluentIcon.IconVariantProperty] = fis[!IconVariantProperty],
                        [!FluentIcon.IconSizeProperty] = fis[!IconSizeProperty],
                        [!FluentIcon.FontSizeProperty] = fis[!FontSizeProperty],
                    };

                    var observable = fis.GetBindingObservable(ForegroundProperty);
                    if (fis.IsSet(ForegroundProperty))
                    {
                        fi.Bind(FluentIcon.ForegroundProperty, observable, BindingPriority.LocalValue);
                    }
                    else
                    {
                        fi.Bind(FluentIcon.ForegroundProperty, observable.SkipOne(), BindingPriority.LocalValue);
                    }
                    return fi;
                }
                return null;
            });
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
