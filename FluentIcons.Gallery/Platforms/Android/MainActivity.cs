using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Fonts.Avalonia.JetBrainsMono;

namespace FluentIcons.Gallery;

[Activity(Label = "FluentIcons.Gallery", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .WithJetBrainsMonoFont();
    }
}
