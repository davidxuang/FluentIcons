using System;
using System.Windows;

namespace FluentIcons.Wpf;

public static class Extensions
{
    internal const string Message = "`UseSegoeMetrics` is deprecated. Migrate to `FluentIcon` for cases where `UseSegoeMetrics == false`.";

    [Obsolete(Message)]
    public static Application UseSegoeMetrics(this Application app) => app;
}
