using System;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Uwp.Internals;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Uwp;

public partial class SymbolIconSource : GenericIconSource
{
    public static DependencyProperty SymbolProperty { get; } 
        = DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIconSource), new(Symbol.Home, OnIconPropertiesChanged));

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override FontFamily IconFont => SymbolIcon.SFontFamily;
}

[MarkupExtensionReturnType(ReturnType = typeof(SymbolIconSource))]
public partial class SymbolIconSourceExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
#if !HAS_UNO
    public FlowDirection? FlowDirection { get; set; }
#endif
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

#if !HAS_UNO
    protected override object ProvideValue()
#else
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
#endif
    {
        var icon = new SymbolIconSource();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

#if !HAS_UNO
        if (FlowDirection.HasValue) icon.FlowDirection = FlowDirection.Value;
#else
        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.FlowDirection = source.FlowDirection;
            source.RegisterPropertyChangedCallback(FrameworkElement.FlowDirectionProperty, (obj, args) 
            => {
                if (obj is FrameworkElement f) icon.FlowDirection = f.FlowDirection;
            });
        }
#endif

        return icon;
    }
}
