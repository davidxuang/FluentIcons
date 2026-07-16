using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Media;
using FluentIcons.Gallery.ViewModels;

namespace FluentIcons.Gallery.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void FlowDirection_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm && sender is TabStrip tabs)
        {
            vm.FlowDirection = (FlowDirection)tabs.SelectedIndex;
        }
    }

    private void UsesSymbol_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm && sender is TabStrip tabs)
        {
            vm.UsesSymbol = tabs.SelectedIndex == 0;
        }
    }

    private void FluentIcon_Tapped(object? sender, TappedEventArgs e)
    {
        if (sender is TabStripItem tab)
        {
            FlyoutBase.ShowAttachedFlyout(tab);
        }
    }

    private void IconVariant_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm && sender is TabStrip tabs)
        {
            vm.IconVariant = (IconVariant)tabs.SelectedIndex;
        }
    }

    public static FuncMultiValueConverter<object?, double> IconContainerSizeConverter { get; } = new(values => values switch
    {
        [true, _, IconVariant.Light] or [_, IconSize.Resizable, IconVariant.Light] => 48.0,
        [true, _, _] or [_, IconSize.Resizable, _] => 36.0,
        [_, IconSize size, _] => (byte)size + 16.0,
        _ => double.NaN,
    });
}