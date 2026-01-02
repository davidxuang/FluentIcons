#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI.Internals;
#else
namespace FluentIcons.Uwp.Internals;
#endif

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract partial class GenericIconSource : FontIconSource
{
    public GenericIconSource()
    {
        Glyph = IconText;
        FontFamily = IconFont;
        FontStyle = Windows.UI.Text.FontStyle.Normal;
        FontWeight = FontWeights.Normal;

        RegisterPropertyChangedCallback(FontFamilyProperty, OnCorePropertyChanged);
        RegisterPropertyChangedCallback(GlyphProperty, OnCorePropertyChanged);

        RegisterPropertyChangedCallback(FontStyleProperty,
            static (o, _) => (o as GenericIconSource)?.FontStyle = Windows.UI.Text.FontStyle.Normal);
        RegisterPropertyChangedCallback(FontWeightProperty,
            static (o, _) => (o as GenericIconSource)?.FontWeight = FontWeights.Normal);
    }

    public IconVariant IconVariant
    {
        get { return (IconVariant)GetValue(IconVariantProperty); }
        set { SetValue(IconVariantProperty, value); }
    }
    public static DependencyProperty IconVariantProperty { get; }
        = DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(GenericIconSource), new(default(IconVariant), OnCorePropertyChanged));

    public FlowDirection FlowDirection
    {
        get { return (FlowDirection)GetValue(FlowDirectionProperty); }
        set { SetValue(FlowDirectionProperty, value); }
    }
    public static DependencyProperty FlowDirectionProperty { get; }
        = DependencyProperty.Register(nameof(FlowDirection), typeof(FlowDirection), typeof(GenericIconSource), new(default(FlowDirection), OnCorePropertyChanged));

    protected abstract string IconText { get; }
    protected abstract FontFamily IconFont { get; }

    protected static void OnCorePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as GenericIconSource)?.InvalidateText();
    }
    protected static void OnCorePropertyChanged(DependencyObject d, DependencyProperty? dp)
    {
        (d as GenericIconSource)?.InvalidateText();
    }

    protected void InvalidateText()
    {
        FontFamily = IconFont;
        Glyph = IconText;
    }

#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
    protected override DependencyProperty GetIconElementPropertyCore(DependencyProperty dp)
    {
        if (dp == IconVariantProperty)
        {
            return GenericIcon.IconVariantProperty;
        }
        else if (dp == FlowDirectionProperty)
        {
            return GenericIcon.FlowDirectionProperty;
        }

        return base.GetIconElementPropertyCore(dp);
    }
#endif
}
