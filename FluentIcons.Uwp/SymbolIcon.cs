using System;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Uwp.Internals;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Uwp;

public partial class SymbolIcon : GenericIcon
{
    internal static readonly FontFamily SFontFamily = new($"ms-appx:///{AssetsNamespace}/Assets/SeagullFluentIcons.ttf#Seagull Fluent Icons");

    public static DependencyProperty SymbolProperty
        => DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new(Symbol.Home, OnIconPropertiesChanged));
    [Obsolete(Extensions.Message)]
    public static DependencyProperty UseSegoeMetricsProperty
        => DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new(false, OnIconPropertiesChanged));

#if !HAS_UNO
    internal SymbolIcon(bool bindFlowDirection) : base(bindFlowDirection) {}
#endif

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }

    [Obsolete(Extensions.Message)]
    public bool UseSegoeMetrics
    {
        get { return (bool)GetValue(UseSegoeMetricsProperty); }
        set { SetValue(UseSegoeMetricsProperty, value); }
    }

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => SFontFamily;
}

[MarkupExtensionReturnType(ReturnType = typeof(SymbolIcon))]
public partial class SymbolIconExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    [Obsolete(Extensions.Message)]
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

#if !HAS_UNO
    protected override object ProvideValue()
#else
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
#endif
    {
        var icon = new SymbolIcon(
#if !HAS_UNO
            true
#endif
            );

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

#if HAS_UNO
        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.SetBinding(
                FrameworkElement.FlowDirectionProperty,
                new Binding { Source = source, Path = new PropertyPath(nameof(source.FlowDirection)) });
        }
#endif

        return icon;
    }
}
