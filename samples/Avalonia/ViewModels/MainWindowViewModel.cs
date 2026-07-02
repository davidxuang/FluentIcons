using System;
using System.Collections.Generic;

namespace FluentIcons.Samples.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public IEnumerable<Common.Symbol> Symbols { get; } = Enum.GetValues<Common.Symbol>();
}
