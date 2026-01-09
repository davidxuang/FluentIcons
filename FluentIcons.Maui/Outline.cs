using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Maui.Internals;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace FluentIcons.Maui;

[Experimental("FI001")]
public static class Outline
{
    public static readonly BindableProperty ForegroundColorProperty
        = BindableProperty.CreateAttached(
            "ForegroundColor",
            typeof(Color),
            typeof(Outline),
            null,
            propertyChanged: OnCorePropertiesChanged);
    public static Color? GetForegroundColor(GenericIcon element)
        => (Color?)element.GetValue(ForegroundColorProperty);
    public static void SetForegroundColor(GenericIcon element, Color? value)
        => element.SetValue(ForegroundColorProperty, value);

    public static readonly BindableProperty SymbolProperty
        = BindableProperty.CreateAttached(
            "Symbol",
            typeof(Symbol?),
            typeof(Outline),
            null,
            propertyChanged: OnCorePropertiesChanged);
    public static Symbol? GetSymbol(SymbolIcon element)
        => (Symbol?)element.GetValue(SymbolProperty);
    public static void SetSymbol(SymbolIcon element, Symbol? value)
        => element.SetValue(SymbolProperty, value);

    public static readonly BindableProperty IconProperty
        = BindableProperty.CreateAttached(
            "Icon",
            typeof(Icon?),
            typeof(Outline),
            null,
            propertyChanged: OnCorePropertiesChanged);
    public static Icon? GetIcon(FluentIcon element)
        => (Icon?)element.GetValue(IconProperty);
    public static void SetIcon(FluentIcon element, Icon? value)
        => element.SetValue(IconProperty, value);

    public static readonly BindableProperty IconVariantProperty
        = BindableProperty.CreateAttached(
            "IconVariant",
            typeof(IconVariant),
            typeof(Outline),
            IconVariant.Regular,
            propertyChanged: OnCorePropertiesChanged);
    public static IconVariant GetIconVariant(GenericIcon element)
        => (IconVariant)element.GetValue(IconVariantProperty);
    public static void SetIconVariant(GenericIcon element, IconVariant value)
        => element.SetValue(IconVariantProperty, value);

    private static readonly BindableProperty HandInProperty
        = BindableProperty.CreateAttached(
            "HandIn",
            typeof(GenericIcon.Core),
            typeof(Outline),
            null);
    private static GenericIcon.Core? GetHandIn(GenericIcon element)
        => (GenericIcon.Core?)element.GetValue(HandInProperty);
    private static void SetHandIn(GenericIcon element, GenericIcon.Core? value)
        => element.SetValue(HandInProperty, value);

    private static void OnTargetPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is GenericIcon element && GetHandIn(element) is not null)
        {
            if (e.PropertyName == nameof(GenericIcon.FontSize) ||
                e.PropertyName == nameof(FluentIcon.Icon) ||
                e.PropertyName == nameof(FluentIcon.IconSize) ||
                e.PropertyName == nameof(SymbolIcon.Symbol))
            {
                Update(element);
            }
        }
    }
    private static void OnCorePropertiesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GenericIcon element)
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

    private static void OnTargetLoaded(object? sender, EventArgs? e)
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
                handin.SetBinding(GenericIcon.Core.FlowDirectionProperty, new Binding(nameof(FlowDirection), source: element));
                element.AddHandIn(handin);
            }
            element.PropertyChanged -= OnTargetPropertyChanged;
            element.PropertyChanged += OnTargetPropertyChanged;
            Update(element);
        }
    }

    private static void OnTargetUnloaded(object? sender, EventArgs? e)
    {
        if (sender is GenericIcon element)
        {
            element.Unloaded -= OnTargetUnloaded;
            var handin = GetHandIn(element);
            if (handin is not null)
            {
                handin.RemoveBinding(GenericIcon.Core.FlowDirectionProperty);
                element.RemoveHandIn(handin);
                SetHandIn(element, null);
            }
            element.PropertyChanged -= OnTargetPropertyChanged;
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
                FluentIcon fi => Internals.FontManager.GetFluent(fi.IconSize, GetIconVariant(element)),
                SymbolIcon => Internals.FontManager.GetSeagull(),
                _ => throw new NotSupportedException(),
            },
            element.FontSize,
            GetForegroundColor(element));
}
