using Avalonia;
using Avalonia.iOS;
using Fonts.Avalonia.JetBrainsMono;
using Foundation;
using UIKit;

namespace FluentIcons.Gallery.Platforms.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the
// User Interface of the application, as well as listening (and optionally responding) to
// application events from iOS.
[Register("AppDelegate")]
public partial class AppDelegate : AvaloniaAppDelegate<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        => base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .WithJetBrainsMonoFont();

    // This is the main entry point of the application.
    internal static void Main(string[] args)
    {
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
