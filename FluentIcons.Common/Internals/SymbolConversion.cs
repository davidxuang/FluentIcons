using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FluentIcons.Common.Internals
{
    internal static class SymbolConversion
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char ToChar(this Symbol symbol, bool isFilled, bool isRtl)
            => char.ConvertFromUtf32((int)symbol + Convert.ToInt32(isFilled) + (Convert.ToInt32(isRtl) << 1)).Single();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ToString(this Symbol symbol, bool isFilled, bool isRtl)
            => char.ConvertFromUtf32((int)symbol + Convert.ToInt32(isFilled) + (Convert.ToInt32(isRtl) << 1));
    }
}
