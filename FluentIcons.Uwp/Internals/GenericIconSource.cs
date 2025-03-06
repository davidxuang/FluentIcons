using FluentIcons.Common;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FluentIcons.Uwp.Internals;

public abstract partial class GenericIconSource : FontIconSource
{
    public static DependencyProperty IconVariantProperty
        => DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(GenericIconSource), new PropertyMetadata(default(IconVariant), OnIconPropertiesChanged));
    public static DependencyProperty FlowDirectionProperty
        => DependencyProperty.Register(nameof(FlowDirection), typeof(FlowDirection), typeof(GenericIconSource), new PropertyMetadata(FlowDirection.LeftToRight, OnIconPropertiesChanged));

    public GenericIconSource()
    {
        FontFamily = IconFont;
        FontStyle = FontStyle.Normal;
        FontWeight = FontWeights.Normal;
        Glyph = IconText;
        IsTextScaleFactorEnabled = false;
        MirroredWhenRightToLeft = false;

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

    public FlowDirection FlowDirection
    {
        get { return (FlowDirection)GetValue(FlowDirectionProperty); }
        set { SetValue(FlowDirectionProperty, value); }
    }

    protected abstract string IconText { get; }
    protected abstract FontFamily IconFont { get; }

    protected static void OnIconPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is GenericIconSource inst)
        {
            inst.FontFamily = inst.IconFont;
            inst.Glyph = inst.IconText;
        }
    }

    private static void OnFontFamilyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIconSource inst)
        {
            inst.FontFamily = inst.IconFont;
        }
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

    private static void OnGlyphChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIconSource inst)
        {
            inst.Glyph = inst.IconText;
        }
    }

    private static void OnIsTextScaleFactorEnabledChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIconSource inst)
        {
            inst.IsTextScaleFactorEnabled = false;
        }
    }

    private static void OnMirroredWhenRightToLeftChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is GenericIconSource inst)
        {
            inst.MirroredWhenRightToLeft = false;
        }
    }
}
