#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI;
#else
namespace FluentIcons.Uwp;
#endif

public static class Extensions
{
    internal const string Message = "`UseSegoeMetrics` is deprecated. Migrate to `FluentIcon` for cases where `UseSegoeMetrics == false`.";

    [Obsolete(Message)]
    public static Application UseSegoeMetrics(this Application app) => app;
#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
    [Obsolete(Message)]
    public static IHostBuilder UseSegoeMetrics(this IHostBuilder builder) => builder;
#endif
}
