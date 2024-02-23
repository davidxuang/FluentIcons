using FluentIcons.Common.Internals;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Symbol = FluentIcons.Common.Symbol;

namespace FluentIcons.WinUI
{
    public partial class SymbolIconSource : FontIconSource
    {
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(Symbol), typeof(SymbolIconSource), new PropertyMetadata(Symbol.Home, OnSymbolPropertiesChanged));
        public static readonly DependencyProperty IsFilledProperty =
            DependencyProperty.Register(nameof(IsFilled), typeof(bool), typeof(SymbolIconSource), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#if WINDOWS
        public static readonly DependencyProperty UseSegoeMetricsProperty =
            DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), PropertyMetadata.Create(() => SymbolIcon.UseSegoeMetricsDefaultValue, OnSymbolPropertiesChanged));
#else
        public static readonly DependencyProperty UseSegoeMetricsProperty =
            DependencyProperty.Register(nameof(UseSegoeMetrics), typeof(bool), typeof(SymbolIcon), new PropertyMetadata(false, OnSymbolPropertiesChanged));
#endif
        public static readonly DependencyProperty FlowDirectionProperty = 
            DependencyProperty.Register(nameof(FlowDirection), typeof(FlowDirection), typeof(SymbolIconSource), new PropertyMetadata(FlowDirection.LeftToRight, OnSymbolPropertiesChanged));

        private string _glyph;

        public SymbolIconSource()
        {
#if !WINDOWS
            UseSegoeMetrics = SymbolIcon.UseSegoeMetricsDefaultValue;
#endif
            FontFamily = UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System;
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

        public FlowDirection FlowDirection
        {
            get { return (FlowDirection)GetValue(FlowDirectionProperty); }
            set { SetValue(FlowDirectionProperty, value); }
        }

        private static void OnSymbolPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SymbolIconSource inst)
            {
                inst.FontFamily = inst.UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System;
                inst.Glyph = inst._glyph = inst.Symbol.ToString(inst.IsFilled, inst.FlowDirection == FlowDirection.RightToLeft);
            }
        }

        private static void OnFontFamilyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIcon inst)
            {
                var font = inst.UseSegoeMetrics ? SymbolIcon.Seagull : SymbolIcon.System;
                if (inst.FontFamily != font)
                {
                    inst.FontFamily = font;
                }
            }
        }

        private static void OnFontStyleChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIconSource inst && inst.FontStyle != Windows.UI.Text.FontStyle.Normal)
            {
                inst.FontStyle = Windows.UI.Text.FontStyle.Normal;
            }
        }

        private static void OnFontWeightChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIconSource inst && inst.FontWeight != FontWeights.Normal)
            {
                inst.FontWeight = FontWeights.Normal;
            }
        }

        private static void OnGlyphChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIconSource inst && inst.Glyph != inst._glyph)
            {
                inst.Glyph = inst._glyph;
            }
        }

        private static void OnIsTextScaleFactorEnabledChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIconSource inst && inst.IsTextScaleFactorEnabled != false)
            {
                inst.IsTextScaleFactorEnabled = false;
            }
        }

        private static void OnMirroredWhenRightToLeftChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender is SymbolIconSource inst && inst.MirroredWhenRightToLeft != false)
            {
                inst.MirroredWhenRightToLeft = false;
            }
        }
    }
}
