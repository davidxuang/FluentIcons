using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;

[assembly: InternalsVisibleTo("FluentIcons.Avalonia")]
[assembly: InternalsVisibleTo("FluentIcons.WPF")]

namespace FluentIcons.Common.Internals
{
    internal static class SymbolConversion
    {
        private static readonly Dictionary<Symbol, FilledSymbol> _mappings = new Dictionary<Symbol, FilledSymbol>()
        {
            { Symbol.PresenceBlocked,       FilledSymbol.FlagPride },
            { Symbol.ColorBackgroundAccent, FilledSymbol.HighlightAccent },
            { Symbol.ColorFillAccent,       FilledSymbol.InkingToolAccent },
            { Symbol.ColorLineAccent,       FilledSymbol.TagLockAccent },
            { Symbol.PresenceOffline,       FilledSymbol.PanelLeftFocusRight },
            { Symbol.PresenceOof,           FilledSymbol.PresenceAway },
            { Symbol.PresenceUnknown,       FilledSymbol.PresenceBusy },
        };

        public static FilledSymbol ToFilledSymbol(this Symbol symbol)
        { 
            if (_mappings.ContainsKey(symbol)) return _mappings[symbol];
            return (FilledSymbol)Enum.Parse(typeof(FilledSymbol), Enum.GetName(typeof(Symbol), symbol));
        }
    }
}
