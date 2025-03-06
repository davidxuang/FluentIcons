using System;
using Windows.UI.Xaml;

namespace FluentIcons.Uwp;

public static class Extensions
{
    internal const string Message = "`UseSegoeMetrics` is deprecated. Migrate to `FluentIcon` for cases where `UseSegoeMetrics == false`.";

    [Obsolete(Message)]
    public static Application UseSegoeMetrics(this Application app) => app;
}
