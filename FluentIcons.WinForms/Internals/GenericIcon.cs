using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FluentIcons.Common;

namespace FluentIcons.WinForms.Internals;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract partial class GenericIcon : Control
{
    private float _scale = 1f;

    protected GenericIcon()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.Transparent;
    }

    [Category("Layout")]
    [DefaultValue(20f)]
    public float FontSize
    {
        get;
        set
        {
            if (field == value || float.IsNaN(value) || float.IsInfinity(value) || value <= 0)
                return;
            field = value;
            OnScaledFontSizeChanged(EventArgs.Empty);
        }
    } = 20f;

    protected float ScaledFontSize => FontSize * _scale;

    protected virtual void OnScaledFontSizeChanged(EventArgs e)
    {
        Invalidate();
    }

    [Category("Appearance")]
    [DefaultValue(IconVariant.Regular)]
    public IconVariant IconVariant
    {
        get;
        set
        {
            if (field == value)
                return;
            field = value;
            OnIconVariantChanged(EventArgs.Empty);
        }
    }

    protected virtual void OnIconVariantChanged(EventArgs e)
    {
        Invalidate();
    }

    protected override void OnRightToLeftChanged(EventArgs e)
    {
        base.OnRightToLeftChanged(e);
        Invalidate();
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        Invalidate();
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        int size = (int)Math.Ceiling(FontSize * _scale);

        return new(
            size + Padding.Horizontal,
            size + Padding.Vertical);
    }

#if NET47_OR_GREATER || NETCOREAPP3_0_OR_GREATER
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateDpiScale();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        UpdateDpiScale();
    }

    private void UpdateDpiScale()
    {
        if (GetParentAutoScaleMode() != AutoScaleMode.Dpi)
            return;

        float scale = DeviceDpi / 96f;
        if (_scale == scale)
            return;

        _scale = scale;
        OnScaledFontSizeChanged(EventArgs.Empty);
        Parent?.PerformLayout(this, nameof(PreferredSize));
        Invalidate();
    }

    private AutoScaleMode GetParentAutoScaleMode()
    {
        for (Control? control = Parent; control is not null; control = control.Parent)
        {
            if (control is ContainerControl container && container.AutoScaleMode != AutoScaleMode.Inherit)
                return container.AutoScaleMode;
        }

        return AutoScaleMode.None;
    }
#endif

    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
#if NET47_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        if (GetParentAutoScaleMode() != AutoScaleMode.Dpi)
#endif
        {
            _scale *= factor.Height;
            OnScaledFontSizeChanged(EventArgs.Empty);
        }

        base.ScaleControl(factor, specified);
        Invalidate();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected abstract string IconText { get; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    private protected abstract IDisposable IconFont { get; }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Rectangle bounds = ClientRectangle;

        bounds = new Rectangle(
            bounds.X + Padding.Left,
            bounds.Y + Padding.Top,
            Math.Max(0, bounds.Width - Padding.Horizontal),
            Math.Max(0, bounds.Height - Padding.Vertical));

        if (bounds.Width == 0 || bounds.Height == 0)
            return;

        Renderer.Instance.Draw(
            e.Graphics,
            IconText,
            IconFont,
            bounds,
            ForeColor);
    }
}
