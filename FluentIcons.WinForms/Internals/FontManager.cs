using System;
using System.Collections.Generic;
using FluentIcons.Common;

namespace FluentIcons.WinForms.Internals;

internal static class FontManager
{
    private static readonly Dictionary<IconSize, Renderer.IDescriptor> _fluent = new(IconSizeValues.List.Count);
    private static readonly Renderer.IDescriptor _seagull;

    internal static Renderer.IDescriptor GetFluent(IconSize size, IconVariant variant)
        => size switch
        {
            IconSize.Resizable when variant != IconVariant.Light => _fluent[IconSize.Size20],
            IconSize.Resizable => _fluent[IconSize.Size32],
            _ => _fluent[size]
        };

    internal static Renderer.IDescriptor GetSeagull() => _seagull;

    static FontManager()
    {
        foreach (var size in IconSizeValues.List)
        {
            if ((byte)size > 0)
            {
                using var stream = typeof(FontManager).Assembly
                    .GetManifestResourceStream($"FluentIcons.WinForms.Assets.FluentSystemIcons-{size}.otf")
                    ?? throw new InvalidOperationException($"Resource 'FluentSystemIcons-{size}.otf' not found.");
                _fluent[size] = Renderer.Instance.Load($"Fluent System Icons {size}", stream);
            }
        }
        using var seagullStream = typeof(FontManager).Assembly
            .GetManifestResourceStream("FluentIcons.WinForms.Assets.SeagullFluentIcons.otf")
            ?? throw new InvalidOperationException("Resource 'SeagullFluentIcons.otf' not found.");
        _seagull = Renderer.Instance.Load("Seagull Fluent Icons", seagullStream);
    }
}
