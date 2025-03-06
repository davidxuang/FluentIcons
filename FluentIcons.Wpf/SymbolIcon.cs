using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Wpf.Internals;

namespace FluentIcons.Wpf;

[TypeConverter(typeof(GenericIconConverter<Symbol, SymbolIcon>))]
public class SymbolIcon : GenericIcon, IValue<Symbol>
{
    private static readonly Typeface _typeface = new(
        new FontFamily(new Uri("pack://application:,,,/FluentIcons.Wpf;component/"), "./Assets/#Seagull Fluent Icons"),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);

    public static readonly DependencyProperty SymbolProperty 
        = DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(Symbol.Home, OnIconPropertiesChanged));
    [Obsolete(Extensions.Message)]
    public static readonly DependencyProperty UseSegoeMetricsProperty 
        = DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSizePropertiesChanged));

    public Symbol Symbol
    {
        get { return (Symbol)GetValue(SymbolProperty); }
        set { SetValue(SymbolProperty, value); }
    }

    Symbol IValue<Symbol>.Value
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
    protected override Typeface IconFont => _typeface;
}

[MarkupExtensionReturnType(typeof(SymbolIcon))]
public class SymbolIconExtension : MarkupExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    [Obsolete(Extensions.Message)]
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
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
