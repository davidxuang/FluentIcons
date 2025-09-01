using Avalonia;
using Avalonia.Browser;
using FluentIcons.Gallery;
using Fonts.Avalonia.JetBrainsMono;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("browser")]

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
        .WithInterFont()
        .WithJetBrainsMonoFont()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
