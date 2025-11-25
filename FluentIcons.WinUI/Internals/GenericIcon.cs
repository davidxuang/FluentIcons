using FluentIcons.Common;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FluentIcons.WinUI.Internals;

public abstract partial class GenericIcon : FontIcon
{
    protected const string AssetsAssembly =
#if !HAS_UNO
        "FluentIcons.WinUI";
#else
        "FluentIcons.Resources.Uno";
#endif

    public GenericIcon()
    {
        FontStyle = Windows.UI.Text.FontStyle.Normal;
        FontWeight = FontWeights.Normal;

        RegisterPropertyChangedCallback(FlowDirectionProperty, OnIconPropertiesChanged);
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
        = DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(GenericIcon), new(default(IconVariant), OnIconPropertiesChanged));

    protected abstract string IconText { get; }
    protected abstract FontFamily IconFont { get; }

    protected void InvalidateText()
    {
        FontFamily = IconFont;
        Glyph = IconText;
    }

    protected static void OnIconPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        (sender as GenericIcon)?.InvalidateText();
    }
    protected static void OnIconPropertiesChanged(DependencyObject sender, DependencyProperty args)
    {
        (sender as GenericIcon)?.InvalidateText();
    }

    private static void OnFontStyleChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIcon inst)
        {
            inst.FontStyle = Windows.UI.Text.FontStyle.Normal;
        }
    }

    private static void OnFontWeightChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIcon inst)
        {
            inst.FontWeight = FontWeights.Normal;
        }
    }
}
