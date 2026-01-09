#if WINDOWS_WINAPPSDK
using Microsoft.Windows.ApplicationModel.Resources;
#elif WINDOWS_UWP
using Windows.ApplicationModel.Resources.Core;
#endif

#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI.Internals;
#else
namespace FluentIcons.Uwp.Internals;
#endif

internal static class AppHelper
{
#if WINDOWS_WINAPPSDK
    private static bool CheckAppPackaged()
    {
        try
        {
            return Windows.ApplicationModel.Package.Current is not null;
        }
        catch
        {
            return false;
        }
    }
    internal static bool IsPackaged { get; } = CheckAppPackaged();

    private static string BaseDirectoryUri { get; } = new Uri(AppContext.BaseDirectory).AbsoluteUri;
    /// <summary>Handle resource URIs for Win2D in unpackaged WinUI 3</summary>
    /// <see href="https://github.com/microsoft/Win2D/issues/941"/>
    internal static string ResolveFast(string str) =>
        IsPackaged ? str : str.Replace("ms-appx:///", BaseDirectoryUri);
#endif

#if WINDOWS_WINAPPSDK || WINDOWS_UWP
    private static readonly ResourceManager _resourceManager
#if WINDOWS_WINAPPSDK
        = new();
#else
        = ResourceManager.Current;
#endif

    internal static Uri Resolve(Uri uri)
    {
        try
        {
            var host = uri.GetComponents(UriComponents.Host, UriFormat.Unescaped);
            var path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).TrimStart('/');
            if (uri.Scheme == "ms-appx" || uri.Scheme == "ms-appx-web")
            {
                var map = _resourceManager.MainResourceMap.GetSubtree("Files");
                var resource = map.GetValue(path);
                return new(resource.ValueAsString);
            }
            else if (uri.Scheme == "ms-resource")
            {
                if (string.IsNullOrEmpty(host))
                {
                    var resource = _resourceManager.MainResourceMap.GetValue(path);
                    return new(resource.ValueAsString);
                }
                else
                {
#if WINDOWS_WINAPPSDK
                    var resource = _resourceManager.MainResourceMap.GetValue($"{host}/{path}");
                    return new(resource.ValueAsString);
#else
                    var map = _resourceManager.AllResourceMaps[uri.Host];
                    var resource = map.GetValue(path);
                    return new(resource.ValueAsString);
#endif
                }
            }
            else if (uri.Scheme == "ms-appdata")
            {
#if WINDOWS_WINAPPSDK
                if (!IsPackaged) return uri;
#endif
                // skip host check

                var segments = path.Split('/', 2);
                if (segments.Length < 2) return uri;

                var root = segments[0].ToLowerInvariant() switch
                {
                    "local" => Windows.Storage.ApplicationData.Current.LocalFolder,
                    "roaming" => Windows.Storage.ApplicationData.Current.RoamingFolder,
                    "temp" => Windows.Storage.ApplicationData.Current.TemporaryFolder,
                    _ => null
                };
                if (root?.Path is null) return uri;

                return new(Path.Combine(root.Path, segments[1]));
            }
        }
        catch { }

        return uri;
    }
#endif
}
