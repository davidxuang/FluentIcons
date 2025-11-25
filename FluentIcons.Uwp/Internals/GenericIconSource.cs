using FluentIcons.Common;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FluentIcons.Uwp.Internals;

public abstract partial class GenericIconSource : FontIconSource
{
    public GenericIconSource()
    {
        FontFamily = IconFont;
        FontStyle = FontStyle.Normal;
        FontWeight = FontWeights.Normal;
        Glyph = IconText;

        RegisterPropertyChangedCallback(FontFamilyProperty, OnIconPropertiesChanged);
        RegisterPropertyChangedCallback(FontStyleProperty, OnFontStyleChanged);
        RegisterPropertyChangedCallback(FontWeightProperty, OnFontWeightChanged);
        RegisterPropertyChangedCallback(GlyphProperty, OnIconPropertiesChanged);
    }

    public IconVariant IconVariant
    {
        get { return (IconVariant)GetValue(IconVariantProperty); }
        set { SetValue(IconVariantProperty, value); }
    }
    public static DependencyProperty IconVariantProperty { get; }
        = DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(GenericIconSource), new(default(IconVariant), OnIconPropertiesChanged));

    public FlowDirection FlowDirection
    {
        get { return (FlowDirection)GetValue(FlowDirectionProperty); }
        set { SetValue(FlowDirectionProperty, value); }
    }
    public static DependencyProperty FlowDirectionProperty { get; }
        = DependencyProperty.Register(nameof(FlowDirection), typeof(FlowDirection), typeof(GenericIconSource), new(default(FlowDirection), OnIconPropertiesChanged));

    protected abstract string IconText { get; }
    protected abstract FontFamily IconFont { get; }

    protected void InvalidateText()
    {
        FontFamily = IconFont;
        Glyph = IconText;
    }

    protected static void OnIconPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        (sender as GenericIconSource)?.InvalidateText();
    }
    protected static void OnIconPropertiesChanged(DependencyObject sender, DependencyProperty args)
    {
        (sender as GenericIconSource)?.InvalidateText();
    }

    private static void OnFontStyleChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIconSource inst)
        {
            inst.FontStyle = FontStyle.Normal;
        }
    }

    private static void OnFontWeightChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIconSource inst)
        {
            inst.FontWeight = FontWeights.Normal;
        }
    }
}
