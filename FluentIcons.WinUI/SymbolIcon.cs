#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI;
#else
namespace FluentIcons.Uwp;
#endif

public partial class SymbolIcon : GenericIcon
{
    internal static readonly FontFamily SFontFamily = new($"ms-appx:///{AssetsAssembly}/Assets/SeagullFluentIcons.ttf#Seagull Fluent Icons");

    public SymbolIcon()
    {
        InvalidateText();
    }

#if WINDOWS_UWP
    internal SymbolIcon(bool bindFlowDirection) : base(bindFlowDirection)
    {
        InvalidateText();
    }
#endif

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }
    public static DependencyProperty SymbolProperty { get; }
        = DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new(Symbol.Home, OnIconPropertiesChanged));

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => SFontFamily;
}
