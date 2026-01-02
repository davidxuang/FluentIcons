#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI.Internals;
#else
namespace FluentIcons.Uwp.Internals;
#endif

internal static class FontManager
{
    private const string AssetsAssembly =
#if WINDOWS_WINAPPSDK
        "FluentIcons.WinUI";
#elif WINDOWS_UWP
        "FluentIcons.Uwp";
#else
        "FluentIcons.Resources.Uno";
#endif

    private static readonly Dictionary<IconSize, FontFamily> _fluent = IconSizeValues.Enumerable
        .Where(size => (byte)size > 0)
        .ToDictionary(k => k, k => new FontFamily($"ms-appx:///{AssetsAssembly}/Assets/FluentSystemIcons-{k}.otf#Fluent System Icons {k}"));

    internal static FontFamily GetFluent(IconSize size, IconVariant variant)
        => size switch
        {
            IconSize.Resizable when variant != IconVariant.Light => _fluent[IconSize.Size20],
            IconSize.Resizable => _fluent[IconSize.Size32],
            _ => _fluent[size]
        };

    private static readonly FontFamily _seagull = new($"ms-appx:///{AssetsAssembly}/Assets/SeagullFluentIcons.otf#Seagull Fluent Icons");
    internal static FontFamily GetSeagull() => _seagull;
}
