using System.Windows;

namespace FluentIcons.Samples.Wpf;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public IEnumerable<Common.Symbol> Symbols { get; } = Enum.GetValues<Common.Symbol>();
}