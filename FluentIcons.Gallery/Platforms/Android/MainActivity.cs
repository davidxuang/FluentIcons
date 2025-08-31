using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace FluentIcons.Gallery;

[Activity(Label = "FluentIcons.Gallery", Theme = "@style/Theme.AppCompat.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity;
