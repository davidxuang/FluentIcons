using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent
{
    [TypeConverter(typeof(SymbolIconConverter))]
    public partial class SymbolIcon : FAIconElement
    {
        internal static readonly Typeface System = new("avares://FluentIcons.Avalonia.Fluent/Assets#Fluent System Icons");
        internal static readonly Typeface Seagull = new("avares://FluentIcons.Avalonia.Fluent/Assets#Seagull Fluent Icons");
        internal static bool UseSegoeMetricsDefaultValue = false;

        public static readonly StyledProperty<Symbol> SymbolProperty =
            AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol), Symbol.Home);
        public static readonly StyledProperty<bool> IsFilledProperty =
            AvaloniaProperty.Register<SymbolIcon, bool>(nameof(IsFilled));
        public static readonly StyledProperty<bool> UseSegoeMetricsProperty =
            AvaloniaProperty.Register<SymbolIcon, bool>(nameof(UseSegoeMetrics));
        public static readonly StyledProperty<double> FontSizeProperty =
            AvaloniaProperty.Register<SymbolIcon, double>(nameof(FontSize), 20d, false);

        private TextLayout? _textLayout;

        public SymbolIcon()
        {
            UseSegoeMetrics = UseSegoeMetricsDefaultValue;
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

        public bool UseSegoeMetrics
        {
            get => GetValue(UseSegoeMetricsProperty);
            set => SetValue(UseSegoeMetricsProperty, value);
        }

        public double FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            InvalidateText();
            base.OnLoaded(e);
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            _textLayout?.Dispose();
            _textLayout = null;
            base.OnUnloaded(e);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            if (change.Property == FontSizeProperty)
            {
                InvalidateMeasure();
                InvalidateText();
            }
            else if (change.Property == ForegroundProperty ||
                change.Property == SymbolProperty ||
                change.Property == IsFilledProperty ||
                change.Property == UseSegoeMetricsProperty)
            {
                InvalidateText();
            }

            base.OnPropertyChanged(change);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double size = FontSize;
            return new Size(
                Width.Or(
                    HorizontalAlignment == HorizontalAlignment.Stretch
                        ? availableSize.Width.Or(size)
                        : Math.Min(availableSize.Width, size)),
                Height.Or(
                    VerticalAlignment == VerticalAlignment.Stretch
                        ? availableSize.Height.Or(size)
                        : Math.Min(availableSize.Height, size)));
        }

        public override void Render(DrawingContext context)
        {
            if (_textLayout is null)
                return;

            double size = FontSize;
            Rect bounds = Bounds;
            using (context.PushClip(new Rect(bounds.Size)))
            {
                var origin = new Point(
                    HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => 0,
                        HorizontalAlignment.Right => bounds.Width - size,
                        _ => (bounds.Width - size) / 2,
                    },
                    VerticalAlignment switch
                    {
                        VerticalAlignment.Top => 0,
                        VerticalAlignment.Bottom => bounds.Height - size,
                        _ => (bounds.Height - size) / 2,
                    });
                _textLayout.Draw(context, origin);
            }
        }

        private void InvalidateText()
        {
            if (!IsLoaded)
                return;

            _textLayout?.Dispose();
            _textLayout = new TextLayout(
                Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft),
                UseSegoeMetrics ? Seagull : System,
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
