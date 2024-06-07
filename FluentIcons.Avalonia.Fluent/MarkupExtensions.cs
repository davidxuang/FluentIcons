using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentIcons.Common;

namespace FluentIcons.Avalonia.Fluent.MarkupExtensions;

public class SymbolIconExtension
{
    public Symbol? Symbol { get; set; }
    public bool? IsFilled { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolIcon ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIcon();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IsFilled.HasValue) icon.IsFilled = IsFilled.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is Visual elem)
        {
            icon.FlowDirection = elem.FlowDirection;
        }

        return icon;
    }
}
public class SymbolIconSourceExtension
{
    public Symbol? Symbol { get; set; }
    public bool? IsFilled { get; set; }
    public bool? UseSegoeMetrics { get; set; }
    public double? FontSize { get; set; }
    public Brush? Foreground { get; set; }

    public SymbolIconSource ProvideValue(IServiceProvider serviceProvider)
    {
        var icon = new SymbolIconSource();

        if (Symbol.HasValue) icon.Symbol = Symbol.Value;
        if (IsFilled.HasValue) icon.IsFilled = IsFilled.Value;
        if (UseSegoeMetrics.HasValue) icon.UseSegoeMetrics = UseSegoeMetrics.Value;
        if (FontSize.HasValue) icon.FontSize = FontSize.Value;
        if (Foreground is not null) icon.Foreground = Foreground;

        var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (service?.TargetObject is Visual elem)
        {
            icon.FlowDirection = elem.FlowDirection;
        }

        return icon;
    }
}
