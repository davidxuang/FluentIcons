using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentIcons.Common;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace FluentIcons.Maui.Internals;

public abstract partial class GenericImageSource : FontImageSource
{
    public static readonly BindableProperty IconVariantProperty
        = BindableProperty.Create(nameof(IconVariant), typeof(IconVariant), typeof(GenericImageSource), propertyChanged: OnIconPropertiesChanged);
    public static readonly BindableProperty FlowDirectionProperty
        = BindableProperty.Create(nameof(FlowDirection), typeof(FlowDirection), typeof(GenericImageSource), default(FlowDirection), propertyChanged: OnIconPropertiesChanged);

    public GenericImageSource()
    {
        InvalidateText();
    }

    public IconVariant IconVariant
    {
        get => (IconVariant)GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }

    [TypeConverter(typeof(FlowDirectionConverter))]
    public FlowDirection FlowDirection
    {
        get => (FlowDirection)GetValue(FlowDirectionProperty);
        set => SetValue(FlowDirectionProperty, value);
    }

    protected abstract string IconText { get; }
    protected abstract string IconFont { get; }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        switch (propertyName)
        {
            case nameof(FontFamily):
                FontFamily = IconFont;
                break;
            case nameof(Glyph):
                Glyph = IconText;
                break;
            default:
                base.OnPropertyChanged(propertyName);
                break;
        }
    }

    public static void OnIconPropertiesChanged(BindableObject bindable, object oldValue, object newValue)
        => (bindable as GenericImageSource)?.InvalidateText();

    private void InvalidateText()
    {
        FontFamily = IconFont;
        Glyph = IconText;
    }
}
