using FluentIcons.Common;

namespace UnoApp1;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    public string Text => Icon.ToString();
    public Icon Icon => Icon.Add;
}
