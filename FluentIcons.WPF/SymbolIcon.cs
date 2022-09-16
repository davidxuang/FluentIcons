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

        static SymbolIcon()
        {
            FontSizeProperty.OverrideMetadata(typeof(SymbolIcon), new FrameworkPropertyMetadata(20d));
        }

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(OnSymbolChanged));
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolChanged));

        public SymbolIcon()
        {
            SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.Grayscale);
            SetValue(FontFamilyProperty, _font);
            SetValue(TextProperty, char.ConvertFromUtf32(IsFilled ? (int)Symbol.ToFilledSymbol() : (int)Symbol).ToString());
            SetValue(TextAlignmentProperty, TextAlignment.Center);
        }

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
                d.SetValue(TextProperty, char.ConvertFromUtf32(inst.IsFilled ? (int)inst.Symbol.ToFilledSymbol() : (int)inst.Symbol).ToString());
            }
        }
    }
}
