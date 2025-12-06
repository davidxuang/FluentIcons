using System;
using Avalonia.Controls;

namespace FluentIcons.Avalonia;

public static class Extensions
{
    internal const string Message = "`UseSegoeMetrics` is deprecated. Migrate to `FluentIcon` for cases where `UseSegoeMetrics == false`.";

    [Obsolete(Message)]
    public static T UseSegoeMetrics<T>(this T builder) where T : AppBuilderBase<T>, new()
        => builder;
}
