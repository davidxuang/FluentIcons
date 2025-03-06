using System;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace FluentIcons.WinUI;

public static class Extensions
{
    internal const string Message = "`UseSegoeMetrics` is deprecated. Migrate to `FluentIcon` for cases where `UseSegoeMetrics == false`.";

    [Obsolete(Message)]
    public static Application UseSegoeMetrics(this Application app) => app;
    [Obsolete(Message)]
    public static IHostBuilder UseSegoeMetrics(this IHostBuilder builder) => builder;
}
