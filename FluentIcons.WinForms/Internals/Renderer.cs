using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FluentIcons.Common;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

#if NETFRAMEWORK
using CommunityToolkit.HighPerformance;
#endif

namespace FluentIcons.WinForms.Internals;

internal abstract class Renderer : IDisposable
{
    public static readonly Renderer Instance;

    static Renderer()
    {
        // CFF support fallback
        // https://learn.microsoft.com/en-us/windows/win32/gdiplus/-gdiplus-creating-a-private-font-collection-use
        Instance = Environment.OSVersion.Version switch
        {
            { Major: > 6 } or { Major: >= 6, Minor: >= 2 } => new GdipRendering(), // GDI+ for Windows 8+
            _ => new GdiRendering() // GDI
        };
    }

    internal interface IDescriptor
    {
        IDisposable GetResource(float size);
    }

    public abstract IDescriptor Load(string name, Stream stream);
    public abstract void Draw(Graphics graphics, string glyph, IDisposable font, Rectangle bounds, Color color);
    public abstract void Dispose();

    private sealed class GdiRendering : Renderer
    {
        private readonly List<IntPtr> handles = new(IconSizeValues.List.Count + 1);

        private sealed record Descriptor(string Name) : IDescriptor
        {
            public IDisposable GetResource(float size)
            {
                int px = Math.Max(1, (int)Math.Round(size));
                var font = new LOGFONTW()
                {
                    lfHeight = -px,
                    lfWidth = 0,
                    lfEscapement = 0,
                    lfOrientation = 0,
                    lfWeight = 400,
                    lfItalic = 0,
                    lfUnderline = 0,
                    lfStrikeOut = 0,
                    lfCharSet = FONT_CHARSET.DEFAULT_CHARSET,
                    lfOutPrecision = FONT_OUTPUT_PRECISION.OUT_DEFAULT_PRECIS,
                    lfClipPrecision = FONT_CLIP_PRECISION.CLIP_DEFAULT_PRECIS,
                    lfQuality = FONT_QUALITY.ANTIALIASED_QUALITY,
                    lfPitchAndFamily = 0,
                    lfFaceName = Name
                };
                return PInvoke.CreateFontIndirect(in font);
            }
        }

        public override unsafe IDescriptor Load(string name, Stream stream)
        {
            IntPtr buffer = Marshal.AllocCoTaskMem((int)stream.Length);
            try
            {
                var span = new Span<byte>(buffer.ToPointer(), (int)stream.Length);
                stream.Read(span);

                uint count = 0;
                IntPtr handle = PInvoke.AddFontMemResourceEx(unchecked(buffer.ToPointer()), (uint)stream.Length, (void*)0, &count);
                if (handle == IntPtr.Zero)
                    throw new InvalidOperationException("Failed to load font.");
                handles.Add(handle);
                return new Descriptor(name);
            }
            finally
            {
                Marshal.FreeCoTaskMem(buffer);
            }
        }

        public override unsafe void Draw(Graphics graphics, string glyph, IDisposable font, Rectangle bounds, Color color)
        {
            if (font is not DeleteObjectSafeHandle hdf)
                throw new ArgumentException("Invalid font type.", nameof(font));

            var hdc = new HDC(graphics.GetHdc());
            try
            {
                var dc = PInvoke.SaveDC(hdc);
                try
                {
                    PInvoke.SelectObject(hdc, hdf);
                    PInvoke.SetBkMode(hdc, BACKGROUND_MODE.TRANSPARENT);
                    PInvoke.SetTextColor(hdc, new COLORREF(color.R + ((uint)color.G << 8) + ((uint)color.B << 16)));
                    fixed (char* ptr = glyph)
                    {
                        var str = new PCWSTR(ptr);
                        var rect = new RECT(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
                        PInvoke.DrawText(hdc, str, -1, ref rect, DRAW_TEXT_FORMAT.DT_CENTER | DRAW_TEXT_FORMAT.DT_VCENTER | DRAW_TEXT_FORMAT.DT_SINGLELINE);
                    }
                }
                finally
                {
                    PInvoke.RestoreDC(hdc, dc);
                }
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }

        public override void Dispose()
        {
            foreach (var handle in handles)
                PInvoke.RemoveFontMemResourceEx(new HANDLE(handle));
        }
    }

    private sealed class GdipRendering : Renderer
    {
        private readonly PrivateFontCollection collection = new();
        private readonly List<IntPtr> handles = new(IconSizeValues.List.Count + 1);
        private readonly StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Near,
            FormatFlags = StringFormatFlags.NoWrap,
        };

        private sealed record Descriptor(FontFamily Family) : IDescriptor
        {
            public IDisposable GetResource(float size) => new Font(Family, size, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public override unsafe IDescriptor Load(string name, Stream stream)
        {
            IntPtr buffer = Marshal.AllocCoTaskMem((int)stream.Length);

            try
            {
                var span = new Span<byte>(buffer.ToPointer(), (int)stream.Length);
                stream.Read(span);
                collection.AddMemoryFont(buffer, (int)stream.Length);
            }
            catch
            {
                Marshal.FreeCoTaskMem(buffer);
                throw;
            }

            handles.Add(buffer);
            return new Descriptor(collection.Families.Single(f => f.Name == name));
        }

        public override void Draw(Graphics graphics, string glyph, IDisposable font, Rectangle bounds, Color color)
        {
            if (font is not Font f)
                throw new ArgumentException("Invalid font type.", nameof(font));
            using SolidBrush brush = new(color);

            graphics.DrawString(
                glyph,
                f,
                brush,
                bounds.X + bounds.Width / 2.0f,
                bounds.Y + (bounds.Height - f.Size) / 2.0f,
                format);
        }

        public override void Dispose()
        {
            foreach (var item in collection.Families)
                item.Dispose();
            collection.Dispose();
            format.Dispose();
            foreach (var handle in handles)
                Marshal.FreeCoTaskMem(handle);
        }
    }
}
