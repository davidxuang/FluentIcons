using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentIcons.Common.Internals;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Avalonia.Fluent
{
    [TypeConverter(typeof(SymbolIconSourceConverter))]
    public class SymbolIconSource : PathIconSource
    {
        private static readonly IGlyphTypeface _system = SymbolIcon.System.GlyphTypeface;
        private static readonly IGlyphTypeface _seagull = SymbolIcon.Seagull.GlyphTypeface;

        public static readonly StyledProperty<Symbol> SymbolProperty = SymbolIcon.SymbolProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<bool> IsFilledProperty = SymbolIcon.IsFilledProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<bool> UseSegoeMetricsProperty = SymbolIcon.UseSegoeMetricsProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<FlowDirection> FlowDirectionProperty = Visual.FlowDirectionProperty.AddOwner<SymbolIconSource>();
        public static readonly StyledProperty<double> FontSizeProperty = SymbolIcon.FontSizeProperty.AddOwner<SymbolIconSource>();

        private Geometry _data;

#pragma warning disable CS8618
        public SymbolIconSource()
        {
            Stretch = Stretch.None;
            InvalidateData();
        }
#pragma warning restore CS8618

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

        public double FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SymbolProperty || change.Property == IsFilledProperty || change.Property == FontSizeProperty)
            {
                InvalidateData();
            }
            else if (change.Property == DataProperty || _data != change.NewValue as Geometry)
            {
                Data = _data;
            }
            else if (change.Property == StretchProperty || Stretch.None != (Stretch)change.NewValue)
            {
                Stretch = Stretch.None;
            }
        }

        private void InvalidateData()
        {
            var codepoint = Symbol.ToChar(IsFilled, FlowDirection == FlowDirection.RightToLeft);
            var typeface = UseSegoeMetrics ? _seagull : _system;
            using var glyphRun = new GlyphRun(typeface, FontSize, new[] { codepoint }, new[] { typeface.GetGlyph(codepoint) });
            Data = _data = glyphRun.BuildGeometry();
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
