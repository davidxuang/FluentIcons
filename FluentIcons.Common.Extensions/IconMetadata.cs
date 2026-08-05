using System.Collections.Generic;
using System.Linq;

namespace FluentIcons.Common.Extensions;

public static partial class IconMetadata
{
    public static IEnumerable<string> Metaphors => _metaphors.Keys;
    public static IEnumerable<Icon> GetIconsByMetaphor(string metaphor) => ((IReadOnlyDictionary<string, IReadOnlyList<Icon>>)_metaphors)[metaphor];
}
