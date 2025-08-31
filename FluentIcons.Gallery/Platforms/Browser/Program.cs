using Avalonia;
using Avalonia.Browser;
using FluentIcons.Gallery;
using System.Runtime.Versioning;
using System.Threading.Tasks;

[assembly:SupportedOSPlatform("browser")]

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
        .WithInterFont()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
