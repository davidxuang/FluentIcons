using System;
using Avalonia;

namespace FluentIcons.Avalonia.Fluent;

public static class Extensions
{
    internal const string Message = "`UseSegoeMetrics` is deprecated. Migrate to `FluentIcon` for cases where `UseSegoeMetrics == false`.";

    [Obsolete(Message)]
    public static AppBuilder UseSegoeMetrics(this AppBuilder builder) => builder;
}
