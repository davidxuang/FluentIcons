using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent
{
    [TypeConverter(typeof(SymbolIconSourceConverter))]
    public class SymbolIconSource : FontIconSource
    {
        public static readonly StyledProperty<Symbol> SymbolProperty = SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<bool> IsFilledProperty = SymbolIcon.IsFilledProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<bool> UseSegoeMetricsProperty = SymbolIcon.UseSegoeMetricsProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<FlowDirection> FlowDirectionProperty = Visual.FlowDirectionProperty.AddOwner<SymbolIconSource>();
        public static new readonly StyledProperty<double> FontSizeProperty = SymbolIcon.FontSizeProperty.AddOwner<SymbolIconSource>();

        private string _glyph;

        public SymbolIconSource()
        {
            UseSegoeMetrics = SymbolIcon.UseSegoeMetricsDefaultValue;
            base.FontSize = FontSize;
            FontFamily = UseSegoeMetrics ? SymbolIcon.Seagull.FontFamily : SymbolIcon.System.FontFamily;
            FontStyle = FontStyle.Normal;
            FontWeight = FontWeight.Regular;
            InvalidateGlyph();
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

        public FlowDirection FlowDirection
        {
            get => GetValue(FlowDirectionProperty);
            set => SetValue(FlowDirectionProperty, value);
        }

        public new double FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            if (change.Property == SymbolProperty
            || change.Property == IsFilledProperty
            || change.Property == UseSegoeMetricsProperty
            || change.Property == FontSizeProperty)
            {
                InvalidateGlyph();
                return;
            }
            else if (change.Property == FontSizeProperty)
            {
                base.FontSize = FontSize;
                return;
            }
            else if (change.Property == FontIconSource.FontSizeProperty)
            {
                if (base.FontSize != FontSize)
                {
                    base.FontSize = FontSize;
                    return;
                }
            }
            else if (change.Property == GlyphProperty)
            {
                if (Glyph != _glyph)
                {
                    Glyph = _glyph;
                    return;
                }
            }
            else if (change.Property == FontFamilyProperty)
            {
                var ff = UseSegoeMetrics ? SymbolIcon.Seagull.FontFamily : SymbolIcon.System.FontFamily;
                if (FontFamily != ff)
                {
                    FontFamily = ff;
                    return;
                }
            }
            else if (change.Property == FontStyleProperty)
            {
                if (FontStyle != FontStyle.Normal)
                {
                    FontStyle = FontStyle.Normal;
                    return;
                }
            }
            else if (change.Property == FontWeightProperty)
            {
                if (FontWeight != FontWeight.Regular)
                {
                    FontWeight = FontWeight.Regular;
                    return;
                }
            }

            base.OnPropertyChanged(change);
        }

        [MemberNotNull(nameof(_glyph))]
        private void InvalidateGlyph()
        {
            Glyph = _glyph = Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft);
        }
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
