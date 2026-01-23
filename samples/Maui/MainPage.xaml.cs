namespace FluentIcons.Samples.Maui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public IEnumerable<Common.Symbol> Symbols { get; } = Enum.GetValues<Common.Symbol>();
}
