using Avalonia;
using Avalonia.Styling;

namespace FluentIcons.Avalonia.Fluent;

public static class Extensions
{
    public static AppBuilder UseSegoeMetrics(this AppBuilder builder)
    {
        SymbolIconSource.UseSegoeMetricsDefaultValue = true;
        return builder.AfterSetup(builder =>
        {
            var style = new Style(x => x.OfType<SymbolIcon>());
            style.Add(new Setter(SymbolIcon.UseSegoeMetricsProperty, true));
            builder.Instance?.Styles.Add(style);
        });
    }
}
