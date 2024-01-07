using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.WPF
{
    [TypeConverter(typeof(SymbolIconConverter))]
    public class SymbolIcon : FrameworkElement
    {
        private static readonly Typeface _font = new(
            new FontFamily(new Uri("pack://application:,,,/FluentIcons.WPF;component/"), "./Assets/#FluentSystemIcons-Resizable"),
            FontStyles.Normal,
            FontWeights.Normal,
            FontStretches.Normal);

        private bool _suspendCreate = true;
        private FormattedText? _formattedText;

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
        public static readonly DependencyProperty FontSizeProperty =
            TextBlock.FontSizeProperty.AddOwner(
                typeof(SymbolIcon),
                new FrameworkPropertyMetadata(
                    20d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    OnSizePropertiesChanged));
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
            set { SetValue(IsFilledProperty, value); }
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

        private static void OnSizePropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SymbolIcon)?.InvalidateMeasure();
            (d as SymbolIcon)?.InvalidateText();
        }

        private static void OnSymbolPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SymbolIcon)?.InvalidateText();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_suspendCreate || _formattedText is null)
            {
                _suspendCreate = false;
                InvalidateText();
            }

            return new Size(
                Math.Min(availableSize.Width, FontSize),
                Math.Min(availableSize.Height, FontSize));
        }

        protected override void OnRender(DrawingContext context)
        {
            if (_formattedText is null)
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

    public class SymbolIconConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(Symbol))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string val)
            {
                return new SymbolIcon { Symbol = (Symbol)Enum.Parse(typeof(Symbol), val) };
            }
            else if (value is Symbol symbol)
            {
                return new SymbolIcon { Symbol = symbol };
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
