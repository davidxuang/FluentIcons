using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentIcons.Common;

namespace FluentIcons.Avalonia.MarkupExtensions;

public sealed class FluentIconExtension
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public FluentIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new FluentIcon();

        if (Icon.HasValue) icon.Icon = Icon.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) icon.IconSize = IconSize.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is Visual source)
        {
            icon.Bind(Visual.FlowDirectionProperty, source.GetBindingObservable(Visual.FlowDirectionProperty));
        }

        return icon;
    }
}

public sealed class SymbolIconExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    [Obsolete(Extensions.Message)]
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is Visual source)
        {
            icon.Bind(Visual.FlowDirectionProperty, source.GetBindingObservable(Visual.FlowDirectionProperty));
        }

        return icon;
    }
}

public sealed class FluentImageExtension
{
    public Icon? Icon { get; set; }
    public IconVariant? IconVariant { get; set; }
    public IconSize? IconSize { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public FluentImage ProvideValue(IServiceProvider serviceProvider)
    {
        var image = new FluentImage();

        if (Icon.HasValue) image.Icon = Icon.Value;
        if (IconVariant.HasValue) image.IconVariant = IconVariant.Value;
        if (IconSize.HasValue) image.IconSize = IconSize.Value;
        if (FontSize.HasValue) image.FontSize = FontSize.Value;
        if (Foreground is not null) image.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is Visual source)
        {
            image.Bind(Visual.FlowDirectionProperty, source.GetBindingObservable(Visual.FlowDirectionProperty));
        }

        return image;
    }
}

public sealed class SymbolImageExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    [Obsolete(Extensions.Message)]
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolImage ProvideValue(IServiceProvider serviceProvider)
    {
        var image = new SymbolImage();

        if (Symbol.HasValue) image.Symbol = Symbol.Value;
        if (IconVariant.HasValue) image.IconVariant = IconVariant.Value;
        if (FontSize.HasValue) image.FontSize = FontSize.Value;
        if (Foreground is not null) image.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is Visual source)
        {
            image.Bind(Visual.FlowDirectionProperty, source.GetBindingObservable(Visual.FlowDirectionProperty));
        }

        return image;
    }
}
