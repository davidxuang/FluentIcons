using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentIcons.Common;

namespace FluentIcons.Avalonia.MarkupExtensions;

public class SymbolIconExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IconVariant.HasValue) icon.IconVariant = IconVariant.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
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

public class SymbolImageExtension
{
    public Symbol? Symbol { get; set; }
    public IconVariant? IconVariant { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolImage ProvideValue(IServiceProvider serviceProvider)
    {
        var image = new SymbolImage();

        if (Symbol.HasValue) image.Symbol = Symbol.Value;
        if (IconVariant.HasValue) image.IconVariant = IconVariant.Value;
        if (UseSegoeMetrics.HasValue) image.UseSegoeMetrics = UseSegoeMetrics.Value;
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
