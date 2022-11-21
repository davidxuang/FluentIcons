using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.WPF
{
    public class SymbolIcon : TextBlock
    {
        private static readonly FontFamily _font =
            new FontFamily(new Uri("pack://application:,,,/FluentIcons.WPF;component/"), "./#FluentSystemIcons-Resizable");

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(OnSymbolChanged));
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolChanged));

        public Symbol Symbol
        {
            get { return (Symbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        public bool IsFilled
        {
            get { return (bool)GetValue(IsFilledProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SymbolIcon inst)
            {
                d.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.Grayscale);
                d.SetValue(FontFamilyProperty, _font);
                d.SetValue(TextProperty, inst.Symbol.ToString(inst.IsFilled));
                d.SetValue(TextAlignmentProperty, TextAlignment.Center);
            }
        }
    }
}
