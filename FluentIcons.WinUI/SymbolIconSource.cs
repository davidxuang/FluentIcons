using System;
using FluentIcons.Common;
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
    public static DependencyProperty IconVariantProperty { get; } =
        DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(SymbolIcon), new PropertyMetadata(default(IconVariant), OnSymbolPropertiesChanged));
    public static DependencyProperty UseSegoeMetricsProperty { get; } =
#if WINDOWS
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => SymbolIcon.UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
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
        Glyph = _glyph = Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
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

    public IconVariant IconVariant
    {
        get { return (IconVariant)GetValue(IconVariantProperty); }
        set { SetValue(IconVariantProperty, value); }
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

    [Obsolete("Deprecated in favour of IconVariant")]
    public bool IsFilled
    {
        get => IconVariant == IconVariant.Filled;
        set => IconVariant = value ? IconVariant.Filled : IconVariant.Regular;
    }

    private static void OnSymbolPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is SymbolIconSource inst)
        {
            inst.FontFamily = inst.UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System;
            inst.Glyph = inst._glyph = inst.Symbol.ToString(inst.IconVariant, inst.FlowDirection == FlowDirection.RightToLeft);
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
    public IconVariant? IconVariant { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        var icon = new SymbolIconSource();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement source)
        {
            icon.FlowDirection = source.FlowDirection;
            source.RegisterPropertyChangedCallback(FrameworkElement.FlowDirectionProperty, (obj, args) =>
            {
                if (obj is FrameworkElement elem) icon.FlowDirection = elem.FlowDirection;
            });
        }

        return icon;
    }
}
