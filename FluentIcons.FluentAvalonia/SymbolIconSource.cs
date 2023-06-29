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
    [TypeConverter(typeof(SymbolIconSourceConverter))]
    public class SymbolIconSource : FontIconSource
    {
        private static readonly FontFamily _font = new("avares://FluentIcons.Avalonia/Fonts#FluentSystemIcons-Resizable");

        public static readonly StyledProperty<Symbol> SymbolProperty = SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<bool> IsFilledProperty = SymbolIcon.IsFilledProperty.AddOwner<SymbolIconSource>();

        public SymbolIconSource()
        {
            FontFamily = _font;
            InvalidateData();
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

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SymbolProperty || change.Property == IsFilledProperty)
            {
                InvalidateData();
            }
        }

        private void InvalidateData()
        {
            base.Glyph = Symbol.ToString(IsFilled);
        }

        [Obsolete("Do not use.")]
        public new static readonly StyledProperty<FontWeight> FontWeightProperty =
            AvaloniaProperty.Register<SymbolIconSource, FontWeight>(nameof(FontWeight));
        [Obsolete("Do not use.")]
        public new static readonly StyledProperty<FontStyle> FontStyleProperty =
            AvaloniaProperty.Register<SymbolIconSource, FontStyle>(nameof(FontSize));
        [Obsolete("Do not use.")]
        public new static readonly StyledProperty<string> GlyphProperty =
            AvaloniaProperty.Register<SymbolIconSource, string>(nameof(Glyph));

        [Obsolete("Do not use.")] public new FontWeight FontWeight { get => base.FontWeight; set { } }
        [Obsolete("Do not use.")] public new FontStyle FontStyle { get => base.FontStyle; set { } }
        [Obsolete("Do not use.")] public new string Glyph { get => base.Glyph; set { } }
    }

    public class SymbolIconSourceConverter : TypeConverter
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
                return new SymbolIconSource { Symbol = (Symbol)Enum.Parse(typeof(Symbol), val) };
            }
            else if (value is Symbol symbol)
            {
                return new SymbolIconSource { Symbol = symbol };
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
