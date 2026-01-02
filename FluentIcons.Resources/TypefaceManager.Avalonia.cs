using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using FluentIcons.Common;

namespace FluentIcons.Resources.Avalonia;

internal static class TypefaceManager
{
    private static readonly Dictionary<IconSize, Typeface> _fluent = IconSizeValues.Enumerable
        .Where(size => (byte)size > 0)
        .ToDictionary(k => k, k => new Typeface($"avares://FluentIcons.Resources.Avalonia/Assets#Fluent System Icons {k}"));
    internal static Typeface GetFluent(IconSize size, IconVariant variant)
        => size switch
        {
            IconSize.Resizable when variant != IconVariant.Light => _fluent[IconSize.Size20],
            IconSize.Resizable => _fluent[IconSize.Size32],
            _ => _fluent[size]
        };

    private static readonly Typeface _seagull = new("avares://FluentIcons.Resources.Avalonia/Assets#Seagull Fluent Icons");
    internal static Typeface GetSeagull() => _seagull;
}
