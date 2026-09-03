using System;
using System.ComponentModel;
using System.Windows.Forms;
using FluentIcons.Common;
using FluentIcons.Common.Internals;
using FluentIcons.WinForms.Internals;
using Icon = FluentIcons.Common.Icon;

namespace FluentIcons.WinForms;

[DefaultProperty(nameof(Icon))]
public partial class FluentIcon : GenericIcon
{
    private IDisposable _font;

    public FluentIcon()
    {
        _font = TypefaceManager.GetFluent(IconSize, IconVariant).GetResource(ScaledFontSize);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _font.Dispose();
    }

    [Category("Appearance")]
    [DefaultValue(Icon.Home)]
    public Icon Icon
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            Invalidate();
        }
    } = Icon.Home;

    [Category("Appearance")]
    [DefaultValue(IconSize.Resizable)]
    public IconSize IconSize
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            OnIconSizeChanged(EventArgs.Empty);
        }
    } = IconSize.Resizable;

    private void OnIconSizeChanged(EventArgs e)
    {
        _font.Dispose();
        _font = TypefaceManager.GetFluent(IconSize, IconVariant).GetResource(ScaledFontSize);
        Invalidate();
    }

    protected override void OnIconVariantChanged(EventArgs e)
    {
        _font.Dispose();
        _font = TypefaceManager.GetFluent(IconSize, IconVariant).GetResource(ScaledFontSize);
        base.OnIconVariantChanged(e);
    }

    protected override void OnScaledFontSizeChanged(EventArgs e)
    {
        _font.Dispose();
        _font = TypefaceManager.GetFluent(IconSize, IconVariant).GetResource(ScaledFontSize);
        base.OnScaledFontSizeChanged(e);
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected override string IconText =>
        Icon.ToString(IconVariant, RightToLeft == RightToLeft.Yes);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    private protected override IDisposable IconFont => _font;
}
