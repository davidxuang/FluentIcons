using FluentIcons.Common;
using FluentIcons.WinForms;
using GenericIcon = FluentIcons.WinForms.Internals.GenericIcon;

namespace FluentIcons.Samples.WinForms;

internal sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "FluentIcons WinForms Sample";
        ClientSize = new(800, 450);
        AutoScaleDimensions = new(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new(16),
            ColumnCount = 1,
            RowCount = 5,
        };
        content.ColumnStyles.Add(new(SizeType.Percent, 100));
        content.RowStyles.Add(new(SizeType.AutoSize));
        content.RowStyles.Add(new(SizeType.AutoSize));
        content.RowStyles.Add(new(SizeType.AutoSize));
        content.RowStyles.Add(new(SizeType.AutoSize));
        content.RowStyles.Add(new(SizeType.Percent, 100));

        content.Controls.Add(CreateTitle("Elements"), 0, 0);
        content.Controls.Add(CreateVariants(() => new FluentIcon { Icon = Common.Icon.Alert }), 0, 1);
        content.Controls.Add(CreateVariants(() => new SymbolIcon { Symbol = Symbol.Chat }), 0, 2);
        content.Controls.Add(CreateTitle("Icon catalog"), 0, 3);
        content.Controls.Add(CreateCatalog(), 0, 4);
        Controls.Add(content);
    }

    private static Label CreateTitle(string text) => new()
    {
        AutoSize = true,
        Dock = DockStyle.Fill,
        Text = text,
        Font = new Font(DefaultFont.FontFamily, 14, FontStyle.Bold),
        Margin = new(0, 8, 0, 4),
    };

    private static FlowLayoutPanel CreateVariants<T>(Func<T> create)
        where T : GenericIcon
    {
        var panel = CreateIconPanel();

        foreach (IconVariant variant in Enum.GetValues<IconVariant>())
        {
            T icon = create();
            icon.IconVariant = variant;
            icon.AutoSize = true;
            icon.Margin = new(4);
            panel.Controls.Add(icon);
        }

        return panel;
    }

    private static FlowLayoutPanel CreateCatalog()
    {
        var panel = CreateIconPanel();
        panel.AutoSize = false;
        panel.AutoScroll = true;

        foreach (Symbol symbol in Enum.GetValues<Symbol>().OrderBy(symbol => symbol.ToString()))
        {
            panel.Controls.Add(new SymbolIcon
            {
                Symbol = symbol,
                AutoSize = true,
                Margin = new(4),
            });
        }

        return panel;
    }

    private static FlowLayoutPanel CreateIconPanel() => new()
    {
        AutoSize = true,
        Dock = DockStyle.Fill,
        WrapContents = true,
        Margin = new(0),
    };
}
