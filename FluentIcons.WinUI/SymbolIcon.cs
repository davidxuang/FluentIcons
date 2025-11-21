using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.WinUI.Internals;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.WinUI;

public partial class SymbolIcon : GenericIcon
{
    internal static readonly FontFamily SFontFamily = new($"ms-appx:///{AssetsNamespace}/Assets/SeagullFluentIcons.ttf#Seagull Fluent Icons");

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
        = DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new(Symbol.Home, OnIconPropertiesChanged));

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => SFontFamily;
}

[MarkupExtensionReturnType(ReturnType = typeof(SymbolIcon))]
public partial class SymbolIconExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.SetBinding(
                FrameworkElement.FlowDirectionProperty,
                new Binding { Source = source, Path = new PropertyPath(nameof(source.FlowDirection)) });
        }

        return icon;
    }
}
