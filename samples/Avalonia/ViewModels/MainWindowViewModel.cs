using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentIcons.Samples.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public IEnumerable<Common.Symbol> Symbols { get; } = Enum.GetValues<Common.Symbol>().OrderBy(s => s.ToString());
}
