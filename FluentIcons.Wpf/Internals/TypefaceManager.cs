using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FluentIcons.Common;

namespace FluentIcons.Wpf.Internals;

internal static class TypefaceManager
{
    private static readonly Dictionary<IconSize, Typeface> _fluent = IconSizeValues.Enumerable
        .Where(size => (byte)size > 0)
        .ToDictionary(k => k, k => new Typeface(
            new FontFamily(new Uri("pack://application:,,,/FluentIcons.Wpf;component/"), $"./Assets/#Fluent System Icons {k}"),
            FontStyles.Normal,
            FontWeights.Normal,
            FontStretches.Normal));
    internal static Typeface GetFluent(IconSize size, IconVariant variant)
        => size switch
        {
            IconSize.Resizable when variant != IconVariant.Light => _fluent[IconSize.Size20],
            IconSize.Resizable => _fluent[IconSize.Size32],
            _ => _fluent[size]
        };

    private static readonly Typeface _seagull = new(
        new FontFamily(new Uri("pack://application:,,,/FluentIcons.Wpf;component/"), "./Assets/#Seagull Fluent Icons"),
        FontStyles.Normal,
        FontWeights.Normal,
        FontStretches.Normal);
    internal static Typeface GetSeagull() => _seagull;
}
