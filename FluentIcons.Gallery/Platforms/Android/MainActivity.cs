using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Fonts.Avalonia.JetBrainsMono;

namespace FluentIcons.Gallery.Platforms.Android;

[Activity(Label = "FluentIcons.Gallery", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        => base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .WithJetBrainsMonoFont();
}
