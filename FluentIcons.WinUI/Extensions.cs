using Microsoft.Extensions.Hosting;

namespace FluentIcons.WinUI
{
    public static class Extensions
    {
        public static IHostBuilder UseSegoeMetrics(this IHostBuilder builder)
        {
            SymbolIcon.UseSegoeMetricsDefaultValue = true;
            return builder;
        }
    }
}
