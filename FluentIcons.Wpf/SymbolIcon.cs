using System;
using System.ComponentModel;
using System.Windows;
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

    public static TypeConverter Converter { get; } = new GenericIconConverter<Symbol, SymbolIcon>();

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
    public static readonly DependencyProperty SymbolProperty
        = DependencyProperty.Register(
            nameof(Symbol),
            typeof(Symbol),
            typeof(SymbolIcon),
            new(Symbol.Home, OnCorePropertyChanged));

    protected override string IconText => Symbol.ToString(IconVariant, FlowDirection == FlowDirection.RightToLeft);
    protected override Typeface IconFont => _typeface;
}
