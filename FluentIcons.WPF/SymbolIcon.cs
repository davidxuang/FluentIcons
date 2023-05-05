using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.WPF
{
    public class SymbolIcon : FrameworkElement
    {
        private static readonly Typeface _font =
            new Typeface(
                new FontFamily(new Uri("pack://application:,,,/FluentIcons.WPF;component/"), "./#FluentSystemIcons-Resizable"),
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);

        private bool _suspendCreate = true;
        private FormattedText _formattedText;

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(OnSymbolPropertiesChanged));
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
        public static readonly DependencyProperty FontSizeProperty =
            TextBlock.FontSizeProperty.AddOwner(typeof(SymbolIcon), new FrameworkPropertyMetadata(20d, OnSymbolPropertiesChanged));
        public static readonly DependencyProperty ForegroundProperty =
            TextBlock.ForegroundProperty.AddOwner(typeof(SymbolIcon), new FrameworkPropertyMetadata(OnSymbolPropertiesChanged));

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

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        private static void OnSymbolPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SymbolIcon)?.InvalidateText();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_suspendCreate || _formattedText == null)
            {
                _suspendCreate = false;
                InvalidateText();
            }

            return new Size(FontSize, FontSize);
        }

        protected override void OnRender(DrawingContext context)
        {
            if (_formattedText == null)
                return;

            var canvas = RenderTransform.TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));
            context.PushClip(new RectangleGeometry(canvas));
            var origin = new Point(canvas.Right - canvas.Width / 2 - _formattedText.Width / 2, canvas.Bottom - canvas.Height / 2 - _formattedText.Height / 2);
            context.DrawText(_formattedText, origin);
            context.Pop();
        }

        private void InvalidateText()
        {
            if (_suspendCreate)
                return;

            _formattedText = new FormattedText(
                Symbol.ToString(IsFilled),
                CultureInfo.CurrentCulture,
                CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                _font,
                FontSize,
                Foreground,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            InvalidateVisual();
        }
    }
}
