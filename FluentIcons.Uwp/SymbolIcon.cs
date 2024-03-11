using System.Diagnostics.CodeAnalysis;
using FluentIcons.Common.Internals;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.Uwp
{
    public partial class SymbolIcon : FontIcon
    {
        internal static readonly FontFamily System = new("ms-appx:///FluentIcons.Uwp/Assets/FluentSystemIcons.ttf#Fluent System Icons");
        internal static readonly FontFamily Seagull = new("ms-appx:///FluentIcons.Uwp/Assets/SeagullFluentIcons.ttf#Seagull Fluent Icons");
        internal static bool UseSegoeMetricsDefaultValue = false;

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIcon), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#if !NET || WINDOWS
        public static readonly DependencyProperty UseSegoeMetricsProperty =
            DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
        public static readonly DependencyProperty UseSegoeMetricsProperty =
            DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#endif

        private string _glyph;

        public SymbolIcon()
        {
#if NET && !WINDOWS
            UseSegoeMetrics = UseSegoeMetricsDefaultValue;
#endif
            FontStyle = FontStyle.Normal;
            FontWeight = FontWeights.Normal;
            IsTextScaleFactorEnabled = false;
            MirroredWhenRightToLeft = false;
            InvalidateText();

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
            (sender as SymbolIcon)?.InvalidateText();
        }

        [MemberNotNull(nameof(_glyph))]
        private void InvalidateText()
        {
            FontFamily = UseSegoeMetrics ? Seagull : System;
            Glyph = _glyph = Symbol.ToString(IsFilled, FlowDirection == FlowDirection.RightToLeft);
        }

        private static void OnFontFamilyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                inst.FontFamily = inst.UseSegoeMetrics ? Seagull : System;
            }
        }

        private static void OnFontStyleChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                inst.FontStyle = FontStyle.Normal;
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
