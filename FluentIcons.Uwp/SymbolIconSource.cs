using FluentIcons.Common.Internals;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Uwp;

public partial class SymbolIconSource : FontIconSource
{
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIconSource), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
    public static readonly DependencyProperty IsFilledProperty =
        DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIconSource), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#if WINDOWS
    public static readonly DependencyProperty UseSegoeMetricsProperty =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => SymbolIcon.UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
    public static readonly DependencyProperty UseSegoeMetricsProperty =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#endif
    public static readonly DependencyProperty FlowDirectionProperty =
        DependencyProperty.Register(nameof(FlowDirection), typeof(FlowDirection), typeof(SymbolIconSource), new PropertyMetadata(FlowDirection.LeftToRight, OnSymbolPropertiesChanged));

    private string _glyph;

    public SymbolIconSource()
    {
#if !WINDOWS
        UseSegoeMetrics = SymbolIcon.UseSegoeMetricsDefaultValue;
#endif
        FontFamily = UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System;
        FontStyle = FontStyle.Normal;
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
            inst.FontStyle = FontStyle.Normal;
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
    public Symbol Symbol { get; set; } = Symbol.Home;
    public bool IsFilled { get; set; }
    public bool UseSegoeMetrics { get; set; } = SymbolIcon.UseSegoeMetricsDefaultValue;
#if UAP10_0_17763_0
    public FlowDirection FlowDirection { get; set; }
#endif
    public double FontSize { get; set; } = 20d;
    public Brush? Foreground { get; set; }

#if UAP10_0_17763_0
    protected override object ProvideValue()
    {
        var icon = new SymbolIconSource
        {
            Symbol = Symbol,
            IsFilled = IsFilled,
            UseSegoeMetrics = UseSegoeMetrics,
            FlowDirection = FlowDirection,
            FontSize = FontSize,
        };

        if (Foreground is not null)
        {
            icon.Foreground = Foreground;
        }

        return icon;
    }
#else
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        var icon = new SymbolIconSource
        {
            Symbol = Symbol,
            IsFilled = IsFilled,
            UseSegoeMetrics = UseSegoeMetrics,
            FontSize = FontSize,
        };

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is FrameworkElement elem)
        {
            icon.FlowDirection = elem.FlowDirection;
        }

        if (Foreground is not null)
        {
            icon.Foreground = Foreground;
        }

        return icon;
    }
#endif
}
