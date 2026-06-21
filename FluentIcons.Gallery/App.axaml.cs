using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using FluentIcons.Gallery.ViewModels;
using FluentIcons.Gallery.Views;

namespace FluentIcons.Gallery;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static IClipboard? Clipboard
    {
        get
        {
            if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
            {
                return window.Clipboard;
            }
            else if (Current?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } view })
            {
                if (TopLevel.GetTopLevel(view) is TopLevel topLevel)
                {
                    return topLevel.Clipboard;
                }
            }

            return null;
        }
    }
}