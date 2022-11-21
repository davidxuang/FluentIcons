using System;
using System.ComponentModel;
using System.Globalization;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.FluentAvalonia
{
    [TypeConverter(typeof(SymbolIconConverter))]
    public partial class SymbolIcon : FAIconElement
    {
        private static readonly Typeface _font
            = new Typeface(new FontFamily("avares://FluentIcons.FluentAvalonia/Fonts#FluentSystemIcons-Resizable"));

        private bool _suspendCreate = true;
        private TextLayout _textLayout;

        public static readonly StyledProperty<double> FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner<FontIcon>();

        public static readonly StyledProperty<Symbol> SymbolProperty =
            AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol));
        public static readonly StyledProperty<bool> IsFilledProperty =
            AvaloniaProperty.Register<SymbolIcon, bool>(nameof(IsFilled));

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

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            if (change.Property == TextElement.ForegroundProperty ||
                change.Property == TextElement.FontSizeProperty ||
                change.Property == SymbolProperty ||
                change.Property == IsFilledProperty)
            {
                InvalidateText();
            }

            base.OnPropertyChanged(change);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_suspendCreate || _textLayout == null)
            {
                _suspendCreate = false;
                InvalidateText();
            }

            return new Size(FontSize, FontSize);
        }

        public override void Render(DrawingContext context)
        {
            if (_textLayout == null)
                return;

            var canvas = new Rect(Bounds.Size);
            using (context.PushClip(canvas))
            {
                var origin = new Point(canvas.Center.X - _textLayout.Bounds.Width / 2, 0);
                _textLayout.Draw(context, origin);
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
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(Symbol) || sourceType == typeof(SymbolIconSource))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string val)
            {
                return new SymbolIcon { Symbol = (Symbol)Enum.Parse(typeof(Symbol), val) };
            }
            else if (value is Symbol symbol)
            {
                return new SymbolIcon { Symbol = symbol };
            }
            else if (value is SymbolIconSource source)
            {
                var icon = new SymbolIcon
                {
                    [!TextElement.FontSizeProperty]   = source[!TextElement.FontSizeProperty],
                    [!SymbolIcon.SymbolProperty]      = source[!SymbolIcon.SymbolProperty],
                    [!SymbolIcon.IsFilledProperty]    = source[!SymbolIcon.IsFilledProperty],
                };

                if (source.IsSet(IconSource.ForegroundProperty))
                {
                    icon.Bind(TextElement.ForegroundProperty, source.GetBindingObservable(IconSource.ForegroundProperty),
                        priority: BindingPriority.LocalValue);
                }
                else
                {
                    icon.Bind(TextElement.ForegroundProperty, source.GetBindingObservable(IconSource.ForegroundProperty).Skip(1),
                        priority: BindingPriority.LocalValue);
                }
                return source;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
