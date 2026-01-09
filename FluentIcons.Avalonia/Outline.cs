using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Media;
#if FLUENT_AVALONIA
using FluentIcons.Avalonia.Fluent.Internals;
#else
using FluentIcons.Avalonia.Internals;
#endif
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.Resources.Avalonia;

#if FLUENT_AVALONIA
namespace FluentIcons.Avalonia.Fluent;
#else
namespace FluentIcons.Avalonia;
#endif

[Experimental("FI001")]
public abstract class Outline
{
    static Outline()
    {
        ForegroundProperty.Changed.AddClassHandler<GenericIcon>(OnAttachedPropertyChanged);
        SymbolProperty.Changed.AddClassHandler<SymbolIcon>(OnAttachedPropertyChanged);
        IconProperty.Changed.AddClassHandler<FluentIcon>(OnAttachedPropertyChanged);
        IconVariantProperty.Changed.AddClassHandler<GenericIcon>(OnAttachedPropertyChanged);

        GenericIcon.FontSizeProperty.Changed.AddClassHandler<GenericIcon>(OnTargetPropertyChanged);
        FluentIcon.IconProperty.Changed.AddClassHandler<FluentIcon>(OnTargetPropertyChanged);
        FluentIcon.IconSizeProperty.Changed.AddClassHandler<FluentIcon>(OnTargetPropertyChanged);
        SymbolIcon.SymbolProperty.Changed.AddClassHandler<SymbolIcon>(OnTargetPropertyChanged);
    }

    public static readonly AttachedProperty<IBrush?> ForegroundProperty
        = AvaloniaProperty.RegisterAttached<Outline, GenericIcon, IBrush?>("Foreground");
    public static IBrush? GetForeground(GenericIcon element)
        => element.GetValue(ForegroundProperty);
    public static void SetForeground(GenericIcon element, IBrush? value)
        => element.SetValue(ForegroundProperty, value);

    public static readonly AttachedProperty<Symbol?> SymbolProperty
        = AvaloniaProperty.RegisterAttached<Outline, SymbolIcon, Symbol?>("Symbol");
    public static Symbol? GetSymbol(SymbolIcon element)
        => element.GetValue(SymbolProperty);
    public static void SetSymbol(SymbolIcon element, Symbol? value)
        => element.SetValue(SymbolProperty, value);

    public static readonly AttachedProperty<Icon?> IconProperty
        = AvaloniaProperty.RegisterAttached<Outline, FluentIcon, Icon?>("Icon");
    public static Icon? GetIcon(FluentIcon element)
        => element.GetValue(IconProperty);
    public static void SetIcon(FluentIcon element, Icon? value)
        => element.SetValue(IconProperty, value);

    public static readonly AttachedProperty<IconVariant> IconVariantProperty
        = AvaloniaProperty.RegisterAttached<Outline, GenericIcon, IconVariant>("IconVariant", IconVariant.Regular);
    public static IconVariant GetIconVariant(GenericIcon element)
        => element.GetValue(IconVariantProperty);
    public static void SetIconVariant(GenericIcon element, IconVariant value)
        => element.SetValue(IconVariantProperty, value);

    private static readonly AttachedProperty<GenericIcon.Core?> HandInProperty
        = AvaloniaProperty.RegisterAttached<Outline, GenericIcon, GenericIcon.Core?>("HandIn");
    private static GenericIcon.Core? GetHandIn(GenericIcon element)
        => element.GetValue(HandInProperty);
    private static void SetHandIn(GenericIcon element, GenericIcon.Core? value)
        => element.SetValue(HandInProperty, value);

    private static void OnTargetPropertyChanged(GenericIcon element, AvaloniaPropertyChangedEventArgs e)
    {
        var context = GetHandIn(element);
        if (context is not null)
        {
            Update(element);
        }
    }
    private static void OnAttachedPropertyChanged(GenericIcon element, AvaloniaPropertyChangedEventArgs e)
    {
        var context = GetHandIn(element);
        if (context is null)
        {
            if (element.IsLoaded)
            {
                OnTargetLoaded(element, new RoutedEventArgs());
            }
            element.Loaded -= OnTargetLoaded;
            element.Loaded += OnTargetLoaded;
        }
        else
        {
            Update(element);
        }
    }

    private static void OnTargetLoaded(object? sender, RoutedEventArgs e)
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
                handin.Bind(GenericIcon.Core.FlowDirectionProperty, element.GetBindingObservable(GenericIcon.FlowDirectionProperty));
                element.AddHandIn(handin);
            }
            Update(element);
        }
    }

    private static void OnTargetUnloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is GenericIcon element)
        {
            element.Unloaded -= OnTargetUnloaded;
            var handin = GetHandIn(element);
            if (handin is not null)
            {
                handin.ClearValue(GenericIcon.Core.FlowDirectionProperty);
                handin.Clear();
                element.RemoveHandIn(handin);
                SetHandIn(element, null);
            }
            SetHandIn(element, null);
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
