using Avalonia;
using Avalonia.Styling;

namespace FluentIcons.Avalonia;

public static class Extensions
{
    public static AppBuilder UseSegoeMetrics(this AppBuilder builder)
    {
        return builder.AfterSetup(builder =>
        {
            var style = new Style(x => x.OfType<SymbolIcon>());
            style.Add(new Setter(SymbolIcon.UseSegoeMetricsProperty, true));
            builder.Instance?.Styles.Add(style);
        });
    }
}
