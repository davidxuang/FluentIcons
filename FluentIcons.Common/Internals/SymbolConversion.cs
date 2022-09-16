using System.Runtime.CompilerServices;
using System;

[assembly: InternalsVisibleTo("FluentIcons.Avalonia")]
[assembly: InternalsVisibleTo("FluentIcons.FluentAvalonia")]
[assembly: InternalsVisibleTo("FluentIcons.WPF")]

namespace FluentIcons.Common.Internals
{
    internal static class SymbolConversion
    {
        public static FilledSymbol ToFilledSymbol(this Symbol symbol)
            => (FilledSymbol)Enum.Parse(typeof(FilledSymbol), Enum.GetName(typeof(Symbol), symbol));
    }
}
