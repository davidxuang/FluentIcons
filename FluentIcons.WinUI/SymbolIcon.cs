using FluentIcons.Common.Internals;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.WinUI
{
    public partial class SymbolIcon : FontIcon
    {
        internal static readonly FontFamily System = new("ms-appx:///FluentIcons.WinUI/Assets/FluentSystemIcons.ttf#Fluent System Icons");
        internal static readonly FontFamily Seagull = new("ms-appx:///FluentIcons.WinUI/Assets/SeagullFluentIcons.ttf#Seagull Fluent Icons");
        internal static bool UseSegoeMetricsDefaultValue = false;

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#if WINDOWS
        public static readonly DependencyProperty UseSegoeMetricsProperty =
            DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
        public static readonly DependencyProperty UseSegoeMetricsProperty =
            DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#endif

        private string _glyph;

        public SymbolIcon()
        {
#if !WINDOWS
            UseSegoeMetrics = UseSegoeMetricsDefaultValue;
#endif
            FontFamily = UseSegoeMetrics ? Seagull : System;
            FontStyle = Windows.UI.Text.FontStyle.Normal;
            FontWeight = FontWeights.Normal;
            Glyph = _glyph = Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft);
            IsTextScaleFactorEnabled = false;
            MirroredWhenRightToLeft = false;

            RegisterPropertyChangedCallback(FontFamilyProperty, OnFontFamilyChanged);
            RegisterPropertyChangedCallback(FontStyleProperty, OnFontStyleChanged);
            RegisterPropertyChangedCallback(FontWeightProperty, OnFontWeightChanged);
            RegisterPropertyChangedCallback(GlyphProperty, OnGlyphChanged);
            RegisterPropertyChangedCallback(IsTextScaleFactorEnabledProperty, OnIsTextScaleFactorEnabledChanged);
            RegisterPropertyChangedCallback(MirroredWhenRightToLeftProperty, OnMirroredWhenRightToLeftChanged);
        }

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

        public bool UseSegoeMetrics
        {
            get { return (bool)GetValue(UseSegoeMetricsProperty); }
            set { SetValue(UseSegoeMetricsProperty, value); }
        }

        private static void OnSymbolPropertiesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is SymbolIcon inst)
            {
                inst.FontFamily = inst.UseSegoeMetrics ? Seagull : System;
                inst.Glyph = inst._glyph = inst.Symbol.ToString(inst.IsFilled, inst.FlowDirection == FlowDirection.RightToLeft);
            }
        }

        private static void OnFontFamilyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                var font = inst.UseSegoeMetrics ? Seagull : System;
                if (inst.FontFamily != font)
                {
                    inst.FontFamily = font;
                }
            }
        }

        private static void OnFontStyleChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                inst.FontStyle = Windows.UI.Text.FontStyle.Normal;
            }
        }

        private static void OnFontWeightChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                inst.FontWeight = FontWeights.Normal;
            }
        }

        private static void OnGlyphChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                inst.Glyph = inst._glyph;
            }
        }

        private static void OnIsTextScaleFactorEnabledChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                inst.IsTextScaleFactorEnabled = false;
            }
        }

        private static void OnMirroredWhenRightToLeftChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                inst.MirroredWhenRightToLeft = false;
            }
        }
    }
}
