using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.FluentAvalonia
{
    [TypeConverter(typeof(SymbolIconConverter))]
    public class SymbolIcon : FontIcon
    {
        private static readonly FontFamily _font
            = new FontFamily("avares://FluentIcons.FluentAvalonia/Fonts#FluentSystemIcons-Resizable");

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

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            FontFamily = _font;
            OnSymbolChanged();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SymbolProperty ||
                change.Property == IsFilledProperty)
            {
                OnSymbolChanged();
            }
        }

        private void OnSymbolChanged()
        {
            Glyph = char.ConvertFromUtf32(IsFilled ? (int)Symbol.ToFilledSymbol() : (int)Symbol).ToString();
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
