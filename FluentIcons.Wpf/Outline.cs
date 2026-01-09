using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Wpf.Internals;

namespace FluentIcons.Wpf;

[Experimental("FI001")]
public static class Outline
{
    public static readonly DependencyProperty ForegroundProperty
        = DependencyProperty.RegisterAttached(
            "Foreground",
            typeof(Brush),
            typeof(Outline),
            new(null, OnAttachedPropertyChanged));
    public static Brush? GetForeground(GenericIcon element)
        => (Brush?)element.GetValue(ForegroundProperty);
    public static void SetForeground(GenericIcon element, Brush? value)
        => element.SetValue(ForegroundProperty, value);

    public static readonly DependencyProperty SymbolProperty
        = DependencyProperty.RegisterAttached(
            "Symbol",
            typeof(Symbol?),
            typeof(Outline),
            new(null, OnAttachedPropertyChanged));
    public static Symbol? GetSymbol(SymbolIcon element)
        => (Symbol?)element.GetValue(SymbolProperty);
    public static void SetSymbol(SymbolIcon element, Symbol? value)
        => element.SetValue(SymbolProperty, value);

    public static readonly DependencyProperty IconProperty
        = DependencyProperty.RegisterAttached(
            "Icon",
            typeof(Icon?),
            typeof(Outline),
            new(null, OnAttachedPropertyChanged));
    public static Icon? GetIcon(FluentIcon element)
        => (Icon?)element.GetValue(IconProperty);
    public static void SetIcon(FluentIcon element, Icon? value)
        => element.SetValue(IconProperty, value);

    public static readonly DependencyProperty IconVariantProperty
        = DependencyProperty.RegisterAttached(
            "IconVariant",
            typeof(IconVariant),
            typeof(Outline),
            new(IconVariant.Regular, OnAttachedPropertyChanged));
    public static IconVariant GetIconVariant(GenericIcon element)
        => (IconVariant)element.GetValue(IconVariantProperty);
    public static void SetIconVariant(GenericIcon element, IconVariant value)
        => element.SetValue(IconVariantProperty, value);

    private static readonly DependencyProperty HandInProperty
        = DependencyProperty.RegisterAttached(
            "HandIn",
            typeof(GenericIcon.Core),
            typeof(Outline),
            new PropertyMetadata(null));
    private static GenericIcon.Core? GetHandIn(GenericIcon element)
        => (GenericIcon.Core?)element.GetValue(HandInProperty);
    private static void SetHandIn(GenericIcon element, GenericIcon.Core? value)
        => element.SetValue(HandInProperty, value);

    private static void OnTargetPropertyChanged(object? sender, EventArgs e)
    {
        if (sender is GenericIcon element && GetHandIn(element) is not null)
        {
            Update(element);
        }
    }
    private static void OnAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GenericIcon element)
        {
            if (GetHandIn(element) is null)
            {
                if (element.IsLoaded)
                {
                    OnTargetLoaded(element, default);
                }
                element.Loaded -= OnTargetLoaded;
                element.Loaded += OnTargetLoaded;
            }
            else
            {
                Update(element);
            }
        }
    }

    private static readonly DependencyPropertyDescriptor _fs
        = DependencyPropertyDescriptor.FromProperty(GenericIcon.FontSizeProperty, typeof(GenericIcon));
    private static readonly DependencyPropertyDescriptor _i
        = DependencyPropertyDescriptor.FromProperty(FluentIcon.IconProperty, typeof(FluentIcon));
    private static readonly DependencyPropertyDescriptor _is
        = DependencyPropertyDescriptor.FromProperty(FluentIcon.IconSizeProperty, typeof(FluentIcon));
    private static readonly DependencyPropertyDescriptor _s
        = DependencyPropertyDescriptor.FromProperty(SymbolIcon.SymbolProperty, typeof(SymbolIcon));

    private static void OnTargetLoaded(object sender, RoutedEventArgs? e)
    {
        if (sender is GenericIcon element)
        {
            var handin = GetHandIn(element);
            if (handin is null)
            {
                element.Unloaded -= OnTargetUnloaded;
                element.Unloaded += OnTargetUnloaded;
                handin = new(element.FontSize);
                SetHandIn(element, handin);
                handin.SetBinding(GenericIcon.Core.FlowDirectionProperty, new Binding(nameof(FlowDirection)) { Source = element });
                element.AddHandIn(handin);
            }
            _fs.AddValueChanged(element, OnTargetPropertyChanged);
            if (element is FluentIcon)
            {
                _i.AddValueChanged(element, OnTargetPropertyChanged);
                _is.AddValueChanged(element, OnTargetPropertyChanged);
            }
            else if (element is SymbolIcon)
            {
                _s.AddValueChanged(element, OnTargetPropertyChanged);
            }
            Update(element);
        }
    }

    private static void OnTargetUnloaded(object sender, RoutedEventArgs? e)
    {
        if (sender is GenericIcon element)
        {
            element.Unloaded -= OnTargetUnloaded;
            var handin = GetHandIn(element);
            if (handin is not null)
            {
                BindingOperations.ClearBinding(handin, GenericIcon.Core.FlowDirectionProperty);
                element.RemoveHandIn(handin);
                SetHandIn(element, null);
            }
            _fs.RemoveValueChanged(element, OnTargetPropertyChanged);
            if (element is FluentIcon)
            {
                _i.RemoveValueChanged(element, OnTargetPropertyChanged);
                _is.RemoveValueChanged(element, OnTargetPropertyChanged);
            }
            else if (element is SymbolIcon)
            {
                _s.RemoveValueChanged(element, OnTargetPropertyChanged);
            }
        }
    }

    private static void Update(GenericIcon element)
        => GetHandIn(element)?.Update(
            element switch
            {
                FluentIcon fi => (GetIcon(fi) ?? fi.Icon).ToString(GetIconVariant(fi), fi.FlowDirection == FlowDirection.RightToLeft),
                SymbolIcon si => (GetSymbol(si) ?? si.Symbol).ToString(GetIconVariant(si), si.FlowDirection == FlowDirection.RightToLeft),
                _ => throw new NotSupportedException(),
            },
            element switch
            {
                FluentIcon fi => TypefaceManager.GetFluent(fi.IconSize, GetIconVariant(element)),
                SymbolIcon => TypefaceManager.GetSeagull(),
                _ => throw new NotSupportedException(),
            },
            element.FontSize,
            GetForeground(element));
}
