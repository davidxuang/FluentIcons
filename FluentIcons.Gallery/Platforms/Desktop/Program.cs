using Avalonia;
using Fonts.Avalonia.JetBrainsMono;

namespace FluentIcons.Gallery.Platforms.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .WithJetBrainsMonoFont()
            .LogToTrace();
}
