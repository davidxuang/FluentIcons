using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia
{
    [TypeConverter(typeof(SymbolIconConverter))]
    public class SymbolIcon : IconElement
    {
        private static readonly Typeface _filledFont
            = new Typeface(new FontFamily("avares://FluentIcons.Avalonia/Fonts#FluentSystemIcons-Filled"));
        private static readonly Typeface _regularFont
            = new Typeface(new FontFamily("avares://FluentIcons.Avalonia/Fonts#FluentSystemIcons-Regular"));

        private TextLayout _textLayout;

        static SymbolIcon()
        {
            FontSizeProperty.OverrideDefaultValue<SymbolIcon>(20d);
        }

        public static readonly StyledProperty<Symbol> SymbolProperty =
            AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol));
        public static readonly StyledProperty<bool> IsFilledProperty =
            AvaloniaProperty.Register<SymbolIcon, bool>(nameof(IsFilled));

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

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TextBlock.FontSizeProperty ||
                change.Property == SymbolProperty)
            {
                OnInvalidateText();
                InvalidateMeasure();                
            }
            else if (change.Property == TextBlock.ForegroundProperty)
            {
                OnInvalidateText();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_textLayout == null)
                OnInvalidateText();

            return _textLayout.Size;
        }

        public override void Render(DrawingContext context)
        {
            if (_textLayout == null)
                OnInvalidateText();

            var canvas = new Rect(Bounds.Size);
            using (context.PushClip(canvas))
            using (context.PushPreTransform(Matrix.CreateTranslation(
                canvas.Center.X - _textLayout.Size.Width / 2,
                canvas.Center.Y - _textLayout.Size.Height / 2)))
            {
                _textLayout.Draw(context);
            }
        }

        private void OnInvalidateText()
        {
            var glyph = char.ConvertFromUtf32(IsFilled ? (int)Symbol.ToFilledSymbol() : (int)Symbol).ToString();

            _textLayout = new TextLayout(
                glyph,
                IsFilled ? _filledFont : _regularFont,
                FontSize,
                Foreground,
                TextAlignment.Center);
        }
    }

    public class SymbolIconConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new SymbolIcon
            {
                Symbol = (Symbol)Enum.Parse(typeof(Symbol), value as string),
            };
        }
    }
}
