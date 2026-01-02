using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentIcons.Common;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace FluentIcons.Maui.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract partial class GenericImageSource : FontImageSource
{
    public IconVariant IconVariant
    {
        get => (IconVariant)GetValue(IconVariantProperty);
        set => SetValue(IconVariantProperty, value);
    }
    public static readonly BindableProperty IconVariantProperty
        = BindableProperty.Create(nameof(IconVariant), typeof(IconVariant), typeof(GenericImageSource), propertyChanged: OnCorePropertyChanged);

    [TypeConverter(typeof(FlowDirectionConverter))]
    public FlowDirection FlowDirection
    {
        get => (FlowDirection)GetValue(FlowDirectionProperty);
        set => SetValue(FlowDirectionProperty, value);
    }
    public static readonly BindableProperty FlowDirectionProperty
        = BindableProperty.Create(nameof(FlowDirection), typeof(FlowDirection), typeof(GenericImageSource), default(FlowDirection), propertyChanged: OnCorePropertyChanged);

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

    protected void InvalidateText()
    {
        FontFamily = IconFont;
        Glyph = IconText;
    }

    public static void OnCorePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        => (bindable as GenericImageSource)?.InvalidateText();
}
