using System;
using System.ComponentModel;
using System.Windows.Forms;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.WinForms.Internals;

namespace FluentIcons.WinForms;

[DefaultProperty(nameof(Symbol))]
public sealed partial class SymbolIcon : GenericIcon
{
    private IDisposable _font;

    public SymbolIcon()
    {
        _font = FontManager.GetSeagull().GetResource(ScaledFontSize);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _font.Dispose();
    }

    [Category("Appearance")]
    [DefaultValue(Symbol.Home)]
    public Symbol Symbol
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            Invalidate();
        }
    } = Symbol.Home;

    protected override void OnScaledFontSizeChanged(EventArgs e)
    {
        _font.Dispose();
        _font = FontManager.GetSeagull().GetResource(ScaledFontSize);
        base.OnScaledFontSizeChanged(e);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected override string IconText =>
        Symbol.ToString(IconVariant, RightToLeft == RightToLeft.Yes);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    private protected override IDisposable IconFont => _font;
}
