using System.Diagnostics.CodeAnalysis;
using FluentIcons.Common.Internals;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Uwp;

public partial class SymbolIcon : FontIcon
{
    internal static readonly FontFamily System = new("ms-appx:///FluentIcons.Uwp/Assets/FluentSystemIcons.ttf#Fluent System Icons");
    internal static readonly FontFamily Seagull = new("ms-appx:///FluentIcons.Uwp/Assets/SeagullFluentIcons.ttf#Seagull Fluent Icons");
#if !NET || WINDOWS
    internal static bool UseSegoeMetricsDefaultValue = false;
#endif

    public static DependencyProperty SymbolProperty { get; } =
        DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
    public static DependencyProperty IsFilledProperty { get; } =
        DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#if !NET || WINDOWS
    public static DependencyProperty UseSegoeMetricsProperty { get; } =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
    public static DependencyProperty UseSegoeMetricsProperty { get; } =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#endif

    private string _glyph;

    public SymbolIcon()
    {
        FontStyle = FontStyle.Normal;
        FontWeight = FontWeights.Normal;
        IsTextScaleFactorEnabled = false;
        MirroredWhenRightToLeft = false;
        InvalidateText();

        RegisterPropertyChangedCallback(FlowDirectionProperty, OnSymbolPropertiesChanged);
        RegisterPropertyChangedCallback(FontFamilyProperty, OnFontFamilyChanged);
        RegisterPropertyChangedCallback(FontStyleProperty, OnFontStyleChanged);
        RegisterPropertyChangedCallback(FontWeightProperty, OnFontWeightChanged);
        RegisterPropertyChangedCallback(GlyphProperty, OnGlyphChanged);
        RegisterPropertyChangedCallback(IsTextScaleFactorEnabledProperty, OnIsTextScaleFactorEnabledChanged);
        RegisterPropertyChangedCallback(MirroredWhenRightToLeftProperty, OnMirroredWhenRightToLeftChanged);
    }

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }

    public bool IsFilled
    {
        get { return (bool)GetValue(IsFilledProperty); }
        set { SetValue(IsFilledProperty, value); }
    }

    public bool UseSegoeMetrics
    {
        get { return (bool)GetValue(UseSegoeMetricsProperty); }
        set { SetValue(UseSegoeMetricsProperty, value); }
    }

    private static void OnSymbolPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        (sender as SymbolIcon)?.InvalidateText();
    }
    private static void OnSymbolPropertiesChanged(DependencyObject sender, DependencyProperty args)
    {
        (sender as SymbolIcon)?.InvalidateText();
    }

    [MemberNotNull(nameof(_glyph))]
    private void InvalidateText()
    {
        FontFamily = UseSegoeMetrics ? Seagull : System;
        Glyph = _glyph = Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft);
    }

    private static void OnFontFamilyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIcon inst)
        {
            inst.FontFamily = inst.UseSegoeMetrics ? Seagull : System;
        }
    }

    private static void OnFontStyleChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIcon inst)
        {
            inst.FontStyle = FontStyle.Normal;
        }
    }

    private static void OnFontWeightChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIcon inst)
        {
            inst.FontWeight = FontWeights.Normal;
        }
    }

    private static void OnGlyphChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIcon inst)
        {
            inst.Glyph = inst._glyph;
        }
    }

    private static void OnIsTextScaleFactorEnabledChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIcon inst)
        {
            inst.IsTextScaleFactorEnabled = false;
        }
    }

    private static void OnMirroredWhenRightToLeftChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIcon inst)
        {
            inst.MirroredWhenRightToLeft = false;
        }
    }
}

[MarkupExtensionReturnType(ReturnType = typeof(SymbolIcon))]
public class SymbolIconExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public bool? IsFilled { get; set; }
    public bool? UseSegoeMetrics { get; set; }
#if UAP10_0_17763_0
    public FlowDirection? FlowDirection { get; set; }
#endif
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

#if UAP10_0_17763_0
    protected override object ProvideValue()
#else
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
#endif
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IsFilled.HasValue) icon.IsFilled = IsFilled.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

#if UAP10_0_17763_0
        if (FlowDirection.HasValue) icon.FlowDirection = FlowDirection.Value;
#else
        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement elem)
        {
            icon.FlowDirection = elem.FlowDirection;
        }
#endif

        return icon;
    }
}
