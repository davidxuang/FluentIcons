using System;
using Avalonia;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.FluentAvalonia
{
    public class SymbolIconSource : PathIconSource
    {
        private static readonly IGlyphTypeface _typeface = new Typeface("avares://FluentIcons.FluentAvalonia/Fonts#FluentSystemIcons-Resizable").GlyphTypeface;

        public static readonly StyledProperty<Symbol> SymbolProperty = SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<bool> IsFilledProperty = SymbolIcon.IsFilledProperty.AddOwner<SymbolIconSource>();

        public SymbolIconSource()
        {
            base.Stretch = Stretch.None;
            InvalidateData();
        }

        public Symbol Symbol
        {
            get => GetValue(SymbolProperty);
            set => SetValue(SymbolProperty, value);
        }

        public bool IsFilled
        {
            get => GetValue(IsFilledProperty);
            set => SetValue(IsFilledProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SymbolProperty || change.Property == IsFilledProperty)
            {
                InvalidateData();
            }
        }

        private void InvalidateData()
        {
            var codepoint = Symbol.ToChar(IsFilled);
            var glyphRun = new GlyphRun(_typeface, 20, new[] { codepoint }, new[] { _typeface.GetGlyph(codepoint) });
            base.Data = glyphRun.BuildGeometry();
        }

        [Obsolete("Do not use.")]
        public new static readonly StyledProperty<Geometry> DataProperty =
            AvaloniaProperty.Register<SymbolIconSource, Geometry>(nameof(Data));

        [Obsolete("Do not use.")] public new Geometry Data { get => base.Data; set { } }
    }
}
