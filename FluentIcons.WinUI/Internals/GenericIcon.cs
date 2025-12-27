#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI.Internals;
#else
namespace FluentIcons.Uwp.Internals;
#endif

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract partial class GenericIcon : FontIcon
{
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

#if WINDOWS_UWP
    internal GenericIcon(bool bindFlowDirection) : this()
    {
        if (!bindFlowDirection)
            return;

        static void handler(object sender, RoutedEventArgs args)
        {
            if (sender is GenericIcon icon)
            {
                icon.Loaded -= handler;
                if (icon.Parent is FrameworkElement parent)
                {
                    icon.SetBinding(
                        FlowDirectionProperty,
                        new Binding { Source = parent, Path = new PropertyPath(nameof(FlowDirection)) });
                }
            }
        };

        Loaded += handler;
    }
#endif

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
