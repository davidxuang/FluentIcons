using System;
using Avalonia;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.FluentAvalonia
{
    public class SymbolIconSource : FontIconSource
    {
        private static readonly FontFamily _font
            = new FontFamily("avares://FluentIcons.FluentAvalonia/Fonts#FluentSystemIcons-Resizable");

        public static readonly StyledProperty<Symbol> SymbolProperty =
            SymbolIcon.SymbolProperty.AddOwner<FontIconSource>();
        public static readonly StyledProperty<bool> IsFilledProperty =
            SymbolIcon.IsFilledProperty.AddOwner<FontIconSource>();

        static SymbolIconSource()
        {
            FontSizeProperty.OverrideDefaultValue<SymbolIconSource>(20d);
        }

        public SymbolIconSource()
        {
            base.FontFamily = _font;
            OnSymbolChanged();
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

        [Obsolete]
        public new FontFamily FontFamily
        {
            get => base.FontFamily;
            set { }
        }

        [Obsolete]
        public new string Glyph
        {
            get => base.Glyph;
            set { }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            if (change.Property == SymbolIcon.SymbolProperty ||
                change.Property == SymbolIcon.IsFilledProperty)
            {
                OnSymbolChanged();
            }

            base.OnPropertyChanged(change);
        }

        private void OnSymbolChanged()
        {
            base.Glyph = Symbol.ToString(IsFilled);
        }
    }
}
