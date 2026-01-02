using FluentIcons.Common;

namespace FluentIcons.Maui.Internals;

internal static class FontManager
{
    internal static string GetFluent(IconSize size, IconVariant variant)
        => size switch
        {
            IconSize.Resizable when variant != IconVariant.Light => "FluentSystemIconsSize20",
            IconSize.Resizable => "FluentSystemIconsSize32",
            _ => $"FluentSystemIcons{size}",
        };

    internal static string GetSeagull() => "SeagullFluentIcons";
}
