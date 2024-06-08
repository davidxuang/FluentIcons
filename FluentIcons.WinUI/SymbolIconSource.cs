using FluentIcons.Common.Internals;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.WinUI;

public partial class SymbolIconSource : FontIconSource
{
    public static DependencyProperty SymbolProperty { get; } =
        DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIconSource), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
    public static DependencyProperty IsFilledProperty { get; } =
        DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIconSource), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#if WINDOWS
    public static DependencyProperty UseSegoeMetricsProperty { get; } =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => SymbolIcon.UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
    public static DependencyProperty UseSegoeMetricsProperty { get; } =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#endif
    public static DependencyProperty FlowDirectionProperty { get; } =
        DependencyProperty.Register(nameof(FlowDirection), typeof(FlowDirection), typeof(SymbolIconSource), new PropertyMetadata(FlowDirection.LeftToRight, OnSymbolPropertiesChanged));

    private string _glyph;

    public SymbolIconSource()
    {
        FontFamily = UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System;
        FontStyle = Windows.UI.Text.FontStyle.Normal;
        FontWeight = FontWeights.Normal;
        Glyph = _glyph = Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft);
        IsTextScaleFactorEnabled = false;
        MirroredWhenRightToLeft = false;

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

    public FlowDirection FlowDirection
    {
        get { return (FlowDirection)GetValue(FlowDirectionProperty); }
        set { SetValue(FlowDirectionProperty, value); }
    }

    private static void OnSymbolPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is SymbolIconSource inst)
        {
            inst.FontFamily = inst.UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System;
            inst.Glyph = inst._glyph = inst.Symbol.ToString(inst.IsFilled, inst.FlowDirection == FlowDirection.RightToLeft);
        }
    }

    private static void OnFontFamilyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIcon inst)
        {
            inst.FontFamily = inst.UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System;
        }
    }

    private static void OnFontStyleChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIconSource inst)
        {
            inst.FontStyle = Windows.UI.Text.FontStyle.Normal;
        }
    }

    private static void OnFontWeightChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIconSource inst)
        {
            inst.FontWeight = FontWeights.Normal;
        }
    }

    private static void OnGlyphChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIconSource inst)
        {
            inst.Glyph = inst._glyph;
        }
    }

    private static void OnIsTextScaleFactorEnabledChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIconSource inst)
        {
            inst.IsTextScaleFactorEnabled = false;
        }
    }

    private static void OnMirroredWhenRightToLeftChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (sender is SymbolIconSource inst)
        {
            inst.MirroredWhenRightToLeft = false;
        }
    }
}

[MarkupExtensionReturnType(ReturnType = typeof(SymbolIconSource))]
public class SymbolIconSourceExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public bool? IsFilled { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        var icon = new SymbolIconSource();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IsFilled.HasValue) icon.IsFilled = IsFilled.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement elem)
        {
            icon.FlowDirection = elem.FlowDirection;
        }

        return icon;
    }
}
