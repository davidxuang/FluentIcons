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
        private static readonly FontFamily _filledFont = 
            new FontFamily(new Uri("pack://application:,,,/FluentIcons.WPF;component/"), "./#FluentSystemIcons-Filled");
        private static readonly FontFamily _regularFont =
            new FontFamily(new Uri("pack://application:,,,/FluentIcons.WPF;component/"), "./#FluentSystemIcons-Regular");

        static SymbolIcon()
        {
            FontSizeProperty.OverrideMetadata(typeof(SymbolIcon), new FrameworkPropertyMetadata(20d));
        }

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(OnSymbolPropertyChanged));
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnIsFilledPropertyChanged));

        public SymbolIcon()
        {
            SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.Grayscale);
            SetValue(FontFamilyProperty, IsFilled ? _filledFont : _regularFont);
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

        private static void OnSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SymbolIcon inst)
            {
                d.SetValue(TextProperty, char.ConvertFromUtf32(inst.IsFilled ? (int)inst.Symbol.ToFilledSymbol() : (int)inst.Symbol).ToString());
            }
        }

        private static void OnIsFilledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SymbolIcon inst)
            {
                d.SetValue(FontFamilyProperty, inst.IsFilled ? _filledFont : _regularFont);
            }
        }
    }
}
