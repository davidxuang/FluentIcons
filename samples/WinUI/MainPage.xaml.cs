namespace FluentIcons.Samples.WinUI;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    public IEnumerable<Common.Symbol> Symbols { get; } = Enum.GetValues<Common.Symbol>();
}
