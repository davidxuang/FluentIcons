#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI;
#else
namespace FluentIcons.Uwp;
#endif

public partial class SymbolIcon : GenericIcon
{
    public SymbolIcon()
    {
        InvalidateText();
    }

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }
    public static DependencyProperty SymbolProperty { get; }
        = DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new(Symbol.Home, OnCorePropertyChanged));

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => FontManager.GetSeagull();
}
