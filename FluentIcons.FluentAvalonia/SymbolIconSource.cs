using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.FluentAvalonia
{
    public class SymbolIconSource : IconSource
    {
        private static readonly FontFamily _font
            = new FontFamily("avares://FluentIcons.FluentAvalonia/Fonts#FluentSystemIcons-Resizable");

        public static readonly StyledProperty<double> FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner<SymbolIconSource>();

        public static readonly StyledProperty<Symbol> SymbolProperty =
            SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<bool> IsFilledProperty =
            SymbolIcon.IsFilledProperty.AddOwner<SymbolIconSource>();

        public double FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
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
    }
}
