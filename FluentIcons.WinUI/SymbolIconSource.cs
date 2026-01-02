#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI;
#else
namespace FluentIcons.Uwp;
#endif

public partial class SymbolIconSource : GenericIconSource
{
    public SymbolIconSource()
    {
        InvalidateText();
    }

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }
    public static DependencyProperty SymbolProperty { get; }
        = DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIconSource), new(Symbol.Home, OnCorePropertyChanged));

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => FontManager.GetSeagull();

#if WINDOWS_WINAPPSDK || HAS_UNO
#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
    protected override IconElement CreateIconElementCore()
#else
    public override IconElement CreateIconElement()
#endif
    {
        var icon = new SymbolIcon
        {
            Symbol = Symbol,
            IconVariant = IconVariant,
            FlowDirection = FlowDirection,
            FontSize = FontSize,
            IsTextScaleFactorEnabled = IsTextScaleFactorEnabled,
            MirroredWhenRightToLeft = MirroredWhenRightToLeft
        };
        if (Foreground != null) icon.Foreground = Foreground;
        return icon;
    }
#endif

#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
    protected override DependencyProperty GetIconElementPropertyCore(DependencyProperty dp)
    {
        if (dp == SymbolProperty)
        {
            return SymbolIcon.SymbolProperty;
        }

        return base.GetIconElementPropertyCore(dp);
    }
#endif
}
