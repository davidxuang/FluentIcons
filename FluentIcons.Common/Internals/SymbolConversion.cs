using System.Runtime.CompilerServices;
using System;

[assembly: InternalsVisibleTo("FluentIcons.Avalonia")]
[assembly: InternalsVisibleTo("FluentIcons.FluentAvalonia")]
[assembly: InternalsVisibleTo("FluentIcons.WPF")]

namespace FluentIcons.Common.Internals
{
    internal static class SymbolConversion
    {
        private static FilledSymbol ToFilledSymbol(this Symbol symbol)
            => (FilledSymbol)Enum.Parse(typeof(FilledSymbol), Enum.GetName(typeof(Symbol), symbol));

        internal static string ToString(this Symbol symbol, bool isFilled)
            => char.ConvertFromUtf32(isFilled ? (int)symbol.ToFilledSymbol() : (int)symbol).ToString();
    }
}
