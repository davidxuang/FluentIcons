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

        RegisterPropertyChangedCallback(FlowDirectionProperty, OnCorePropertyChanged);
        RegisterPropertyChangedCallback(FontFamilyProperty, OnCorePropertyChanged);
        RegisterPropertyChangedCallback(GlyphProperty, OnCorePropertyChanged);

        RegisterPropertyChangedCallback(FontStyleProperty,
            static (o, _) => (o as GenericIcon)?.FontStyle = Windows.UI.Text.FontStyle.Normal);
        RegisterPropertyChangedCallback(FontWeightProperty,
            static (o, _) => (o as GenericIcon)?.FontWeight = FontWeights.Normal);
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
        = DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(GenericIcon), new(default(IconVariant), OnCorePropertyChanged));

    protected abstract string IconText { get; }
    protected abstract FontFamily IconFont { get; }

    protected static void OnCorePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as GenericIcon)?.InvalidateText();
    }
    protected static void OnCorePropertyChanged(DependencyObject d, DependencyProperty? dp)
    {
        (d as GenericIcon)?.InvalidateText();
    }

    protected void InvalidateText()
    {
        FontFamily = IconFont;
        Glyph = IconText;
    }

#if HAS_UNO
    private static Action<IconElement, UIElement>? GetAddIconChild()
    {
        var type = typeof(IconElement);
        System.Diagnostics.Debug.Assert(typeof(GenericIcon).IsAssignableTo(type));
        var methodInfo = type.GetMethod("AddIconChild", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return methodInfo is not null
            ? (Action<IconElement, UIElement>)Delegate.CreateDelegate(
                typeof(Action<IconElement, UIElement>),
                null,
                methodInfo)
            : null;
    }
    private static readonly Action<IconElement, UIElement>? _addIconChild = GetAddIconChild();

    internal static bool CanAddHandIn => _addIconChild is not null;
    internal void AddHandIn(UIElement element)
        => _addIconChild?.Invoke(this, element);
#endif
}
