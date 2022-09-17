using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentIcons.Common;
using FluentIcons.Common.Internals;

namespace FluentIcons.Avalonia
{
    [TypeConverter(typeof(SymbolIconConverter))]
    public class SymbolIcon : IconElement
    {
        private static readonly Typeface _font
            = new Typeface(new FontFamily("avares://FluentIcons.Avalonia/Fonts#FluentSystemIcons-Resizable"));

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

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TextElement.FontSizeProperty ||
                change.Property == TextElement.ForegroundProperty ||
                change.Property == SymbolProperty ||
                change.Property == IsFilledProperty)
            {
                OnSymbolChanged();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_textLayout == null)
                OnSymbolChanged();

            return _textLayout.Bounds.Size;
        }

        public override void Render(DrawingContext context)
        {
            if (_textLayout == null)
                OnSymbolChanged();

            var canvas = new Rect(Bounds.Size);
            using (context.PushClip(canvas))
            {
                var origin = new Point(canvas.Center.X - _textLayout.Bounds.Size.Width / 2, canvas.Center.Y - _textLayout.Bounds.Size.Height / 2);
                _textLayout.Draw(context, origin);
            }
        }

        private void OnSymbolChanged()
        {
            var glyph = Symbol.ToString(IsFilled);

            _textLayout = new TextLayout(
                glyph,
                _font,
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
