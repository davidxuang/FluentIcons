using System.Diagnostics.CodeAnalysis;
using FluentIcons.Common.Internals;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.WinUI;

public partial class SymbolIcon : FontIcon
{
    internal static readonly FontFamily System = new("ms-appx:///FluentIcons.WinUI/Assets/FluentSystemIcons.ttf#Fluent System Icons");
    internal static readonly FontFamily Seagull = new("ms-appx:///FluentIcons.WinUI/Assets/SeagullFluentIcons.ttf#Seagull Fluent Icons");
    internal static bool UseSegoeMetricsDefaultValue = false;

    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
    public static readonly DependencyProperty IsFilledProperty =
        DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#if !NET || WINDOWS
    public static readonly DependencyProperty UseSegoeMetricsProperty =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
    public static readonly DependencyProperty UseSegoeMetricsProperty =
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#endif

    private string _glyph;

    public SymbolIcon()
    {
#if NET && !WINDOWS
        UseSegoeMetrics = UseSegoeMetricsDefaultValue;
#endif
        FontStyle = Windows.UI.Text.FontStyle.Normal;
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
            inst.FontStyle = Windows.UI.Text.FontStyle.Normal;
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
    public Symbol Symbol { get; set; } = Symbol.Home;
    public bool IsFilled { get; set; }
    public bool UseSegoeMetrics { get; set; } = SymbolIcon.UseSegoeMetricsDefaultValue;
    public double FontSize { get; set; } = 20d;
    public Brush? Foreground { get; set; }

    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon
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
}
