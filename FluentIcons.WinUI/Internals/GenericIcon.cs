using FluentIcons.Common;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FluentIcons.WinUI.Internals;

public abstract partial class GenericIcon : FontIcon
{
    protected const string AssetsNamespace =
#if !HAS_UNO
        "FluentIcons.WinUI";
#else
        "FluentIcons.Uno";
#endif

    public static DependencyProperty IconVariantProperty { get; }
        = DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(GenericIcon), new(default(IconVariant), OnIconPropertiesChanged));

    public GenericIcon()
    {
        FontStyle = Windows.UI.Text.FontStyle.Normal;
        FontWeight = FontWeights.Normal;
        IsTextScaleFactorEnabled = false;
        MirroredWhenRightToLeft = false;
        InvalidateText();

        RegisterPropertyChangedCallback(FlowDirectionProperty, OnIconPropertiesChanged);
        RegisterPropertyChangedCallback(FontFamilyProperty, OnFontFamilyChanged);
        RegisterPropertyChangedCallback(FontStyleProperty, OnFontStyleChanged);
        RegisterPropertyChangedCallback(FontWeightProperty, OnFontWeightChanged);
        RegisterPropertyChangedCallback(GlyphProperty, OnGlyphChanged);
        RegisterPropertyChangedCallback(IsTextScaleFactorEnabledProperty, OnIsTextScaleFactorEnabledChanged);
        RegisterPropertyChangedCallback(MirroredWhenRightToLeftProperty, OnMirroredWhenRightToLeftChanged);
    }

    public IconVariant IconVariant
    {
        get { return (IconVariant)GetValue(IconVariantProperty); }
        set { SetValue(IconVariantProperty, value); }
    }

    protected abstract string IconText { get; }
    protected abstract FontFamily IconFont { get; }

    protected static void OnIconPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        (sender as GenericIcon)?.InvalidateText();
    }
    protected static void OnIconPropertiesChanged(DependencyObject sender, DependencyProperty args)
    {
        (sender as GenericIcon)?.InvalidateText();
    }

    private void InvalidateText()
    {
        FontFamily = IconFont;
        Glyph = IconText;
    }

    private static void OnFontFamilyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIcon inst)
        {
            inst.FontFamily = inst.IconFont;
        }
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

    private static void OnGlyphChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIcon inst)
        {
            inst.Glyph = inst.IconText;
        }
    }

    private static void OnIsTextScaleFactorEnabledChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIcon inst)
        {
            inst.IsTextScaleFactorEnabled = false;
        }
    }

    private static void OnMirroredWhenRightToLeftChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIcon inst)
        {
            inst.MirroredWhenRightToLeft = false;
        }
    }
}
