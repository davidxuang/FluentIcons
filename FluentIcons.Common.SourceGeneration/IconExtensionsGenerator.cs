using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentIcons.Common.Internals;
using Microsoft.CodeAnalysis;

namespace FluentIcons.Common.SourceGeneration;

[Generator]
public sealed class IconExtensionsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var files = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".otf"));

        context.RegisterSourceOutput(files, (spc, file) =>
        {
            var raw = new byte[0x10000 / 8];
            {
                using var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
                using var reader = new BinaryReader(stream);
                var defined = GetBestCmap(reader)
                    .Select(item => item.Item1)
                    .Where(cp => cp >= 0xf0000u && cp < 0x100000u)
                    .Select(cp => (int)(cp - 0xf0000u));
                var bits = new BitSetView(raw);
                foreach (var i in defined)
                {
                    bits[i] = true;
                }
            }
            int n = raw.Length - 1;
            while (n >= 0 && raw[n] == 0)
            {
                n--;
            }

            var name = Path.GetFileNameWithoutExtension(file.Path);
            var size = name.StartsWith("Seagull")
                ? "Resizable"
                : name.Split('-').Last();
            var suffix = size == "Resizable" ? "0" : $"{size.Substring(4)}";

            var builder = new StringBuilder();
            builder.Append($$"""
                using System;
                using System.IO;
                using FluentIcons.Common.Internals;

                namespace FluentIcons.Common;

                public static partial class IconExtensions
                {
                    private static readonly BitSetView _bits{{suffix}} = new BitSetView([
                """);

            builder.AppendLine();
            for (int i = 0; i <= n; i++)
            {
                builder.Append($"0x{raw[i]:X2},");
            }
            builder.AppendLine();

            builder.Append("""
                    ]);
                }
                """);

            spc.AddSource($"IconExtensions.{size}.g.cs", builder.ToString());
        });
    }

    private IEnumerable<(uint, uint)> GetBestCmap(BinaryReader reader)
    {
        reader.BaseStream.Seek(4, SeekOrigin.Begin);
        var numTables = reader.ReadUInt16BigEndian();
        reader.BaseStream.Seek(12, SeekOrigin.Begin);

        uint offsetCmap = 0;
        for (int i = 0; i < numTables; i++)
        {
            var tag = Encoding.ASCII.GetString(reader.ReadBytes(4));
            reader.BaseStream.Seek(4, SeekOrigin.Current); // checksum
            var offset = reader.ReadUInt32BigEndian();
            if (tag == "cmap")
            {
                offsetCmap = offset;
                break;
            }
            reader.BaseStream.Seek(4, SeekOrigin.Current); // length
        }
        if (offsetCmap == 0) yield break;

        reader.BaseStream.Seek(offsetCmap, SeekOrigin.Begin);
        var version = reader.ReadUInt16BigEndian();
        var numSubtables = reader.ReadUInt16BigEndian();

        var offsetCmapSubtables = Enumerable.Range(0, numSubtables)
            .Select(i =>
            {
                var platform = reader.ReadUInt16BigEndian();
                var encoding = reader.ReadUInt16BigEndian();
                var offset = reader.ReadUInt32BigEndian();
                return (platform, encoding, offset);
            })
            .OrderBy(item => item switch
            {
                (3, 10, _) => -2,
                (0, 4, _) => -1,
                _ => 0,
            })
            .Select(item => item.offset);

        foreach (var offset in offsetCmapSubtables)
        {
            reader.BaseStream.Seek(offsetCmap + offset, SeekOrigin.Begin);
            var format = reader.ReadUInt16BigEndian();
            if (format != 12) continue;

            reader.BaseStream.Seek(2, SeekOrigin.Current); // reserved
            reader.BaseStream.Seek(4, SeekOrigin.Current); // length
            reader.BaseStream.Seek(4, SeekOrigin.Current); // language
            var numGroups = reader.ReadUInt32BigEndian();

            for (uint i = 0; i < numGroups; i++)
            {
                var startCharCode = reader.ReadUInt32BigEndian();
                var endCharCode = reader.ReadUInt32BigEndian();
                var startGlyphId = reader.ReadUInt32BigEndian();
                for (uint j = startCharCode, k = startGlyphId; j <= endCharCode; j++, k++)
                {
                    yield return (j, k);
                }
            }
            yield break;
        }
    }
}

internal static class BinaryReaderExtensions
{
    public static ushort ReadUInt16BigEndian(this BinaryReader reader)
    {
        var bytes = (stackalloc byte[2] { reader.ReadByte(), reader.ReadByte() });
        return BinaryPrimitives.ReadUInt16BigEndian(bytes);
    }

    public static uint ReadUInt32BigEndian(this BinaryReader reader)
    {
        var bytes = (stackalloc byte[4] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() });
        return BinaryPrimitives.ReadUInt32BigEndian(bytes);
    }
}
