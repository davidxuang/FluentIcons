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
        private static readonly Typeface _font = new(new FontFamily("avares://FluentIcons.Avalonia/Fonts#FluentSystemIcons-Resizable"));

        private bool _suspendCreate = true;
        private TextLayout? _textLayout;

        public static readonly StyledProperty<Symbol> SymbolProperty =
            AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol), Symbol.Home);
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

            if (change.Property == TextBlock.FontSizeProperty)
            {
                InvalidateMeasure();
                InvalidateText();
            }
            else if (change.Property == TextBlock.ForegroundProperty ||
                change.Property == SymbolProperty ||
                change.Property == IsFilledProperty)
            {
                InvalidateText();
            }

            base.OnPropertyChanged(change);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_suspendCreate || _textLayout is null)
            {
                _suspendCreate = false;
                InvalidateText();
            }

            return new Size(FontSize, FontSize);
        }

        public override void Render(DrawingContext context)
        {
            if (_textLayout is null)
                return;

            var canvas = new Rect(Bounds.Size);
            using (context.PushClip(canvas))
            using (context.PushPreTransform(Matrix.CreateTranslation(
                canvas.Center.X - _textLayout.Size.Width / 2,
                canvas.Center.Y - _textLayout.Size.Height / 2)))
            {
                _textLayout.Draw(context);
            }
        }

        private void InvalidateText()
        {
            if (_suspendCreate)
                return;

            _textLayout = new TextLayout(
                Symbol.ToString(IsFilled),
                _font,
                FontSize,
                Foreground,
                TextAlignment.Center);

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
