using Windows.UI.Xaml;

namespace FluentIcons.Uwp
{
    public static class Extensions
    {
        public static Application UseSegoeMetrics(this Application app)
        {
            SymbolIcon.UseSegoeMetricsDefaultValue = true;
            return app;
        }
    }
}
