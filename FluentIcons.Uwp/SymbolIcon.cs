using System;
using System.Diagnostics.CodeAnalysis;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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
    public static DependencyProperty IconVariantProperty { get; } =
        DependencyProperty.Register(nameof(IconVariant), typeof(IconVariant), typeof(SymbolIcon), new PropertyMetadata(default(IconVariant), OnSymbolPropertiesChanged));
    public static DependencyProperty UseSegoeMetricsProperty { get; } =
#if !NET || WINDOWS
        DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
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

#if UAP10_0_17763_0
    internal SymbolIcon(bool bindFlowDirection) : this()
    {
        if (!bindFlowDirection)
            return;

        static void handler(object sender, RoutedEventArgs args)
        {
            if (sender is SymbolIcon icon)
            {
                icon.Loaded -= handler;
                icon.SetBinding(
                    FlowDirectionProperty,
                    new Binding { Source = icon.Parent, Path = new PropertyPath(nameof(FlowDirection)) });
            }
        };

        Loaded += handler;
    }
#endif

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

    [Obsolete("Deprecated in favour of IconVariant")]
    public bool IsFilled
    {
        get => IconVariant == IconVariant.Filled;
        set => IconVariant = value ? IconVariant.Filled : IconVariant.Regular;
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
        Glyph = _glyph = Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
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
    public IconVariant? IconVariant { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

#if UAP10_0_17763_0
    protected override object ProvideValue()
#else
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
#endif
    {
        var icon = new SymbolIcon(
#if UAP10_0_17763_0
            true
#endif
            );

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

#if !UAP10_0_17763_0
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
