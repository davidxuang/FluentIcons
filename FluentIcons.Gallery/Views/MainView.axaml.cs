using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using FluentIcons.Gallery.ViewModels;

namespace FluentIcons.Gallery.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void UsesSymbol_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm && sender is TabStrip tabs)
        {
            vm.UsesSymbol = tabs.SelectedIndex != 0;
        }
    }

    private void IconVariant_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm && sender is TabStrip tabs)
        {
            vm.IconVariant = (IconVariant)tabs.SelectedIndex;
        }
    }
}