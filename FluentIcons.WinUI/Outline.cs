#if WINDOWS_WINAPPSDK || WINDOWS_UWP
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
#if WINDOWS_WINAPPSDK
using Microsoft.Graphics.DirectX;
#else
using Windows.Graphics.DirectX;
#endif
#endif

#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
namespace FluentIcons.WinUI;
#else
namespace FluentIcons.Uwp;
#endif

[Experimental("FI001")]
public static partial class Outline
{
    public static DependencyProperty ForegroundProperty { get; }
        = DependencyProperty.RegisterAttached(
            "Foreground",
            typeof(Brush),
            typeof(Outline),
            new PropertyMetadata(null, OnAttachedPropertyChanged));
    public static Brush? GetForeground(GenericIcon element)
        => (Brush?)element.GetValue(ForegroundProperty);
    public static void SetForeground(GenericIcon element, Brush? value)
        => element.SetValue(ForegroundProperty, value);

    public static DependencyProperty SymbolProperty { get; }
        = DependencyProperty.RegisterAttached(
            "Symbol",
            typeof(Symbol?),
            typeof(Outline),
            new PropertyMetadata(null, OnAttachedPropertyChanged));
    public static Symbol? GetSymbol(SymbolIcon element)
        => (Symbol?)element.GetValue(SymbolProperty);
    public static void SetSymbol(SymbolIcon element, Symbol? value)
        => element.SetValue(SymbolProperty, value);

    public static DependencyProperty IconProperty { get; }
        = DependencyProperty.RegisterAttached(
            "Icon",
            typeof(Icon?),
            typeof(Outline),
            new PropertyMetadata(null, OnAttachedPropertyChanged));
    public static Icon? GetIcon(FluentIcon element)
        => (Icon?)element.GetValue(IconProperty);
    public static void SetIcon(FluentIcon element, Icon? value)
        => element.SetValue(IconProperty, value);

    public static DependencyProperty IconVariantProperty { get; }
        = DependencyProperty.RegisterAttached(
            "IconVariant",
            typeof(IconVariant),
            typeof(Outline),
            new PropertyMetadata(IconVariant.Regular, OnAttachedPropertyChanged));
    public static IconVariant GetIconVariant(GenericIcon element)
        => (IconVariant)element.GetValue(IconVariantProperty);
    public static void SetIconVariant(GenericIcon element, IconVariant value)
        => element.SetValue(IconVariantProperty, value);

    private sealed partial class Context { }

    private static DependencyProperty ContextProperty { get; }
        = DependencyProperty.RegisterAttached(
            "Context",
            typeof(Context),
            typeof(Outline),
            new PropertyMetadata(null));
    private static Context? GetContext(GenericIcon obj)
        => (Context)obj.GetValue(ContextProperty);
    private static void SetContext(GenericIcon obj, Context? value)
        => obj.SetValue(ContextProperty, value);

    private static void OnTargetPropertyChanged(DependencyObject d, DependencyProperty dp)
    {
        if (d is GenericIcon element && GetContext(element) is not null)
        {
            Update(element);
        }
    }
    private static void OnAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GenericIcon element)
        {
            var context = GetContext(element);
            if (context is null)
            {
                if (element.IsLoaded)
                {
                    OnTargetLoaded(element, default!);
                }
                element.Loaded -= OnTargetLoaded;
                element.Loaded += OnTargetLoaded;
            }
            else
            {
                Update(element);
            }
        }
    }

#if WINDOWS_WINAPPSDK || WINDOWS_UWP
    public static bool IsSupported => true;

    private static float GetScaleFactor(UIElement element)
#if WINDOWS_WINAPPSDK
        => (float)(element.XamlRoot?.RasterizationScale ?? 1.0f);
#else
        => (float)Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
#endif

    private sealed partial class Context
    {
        public CompositionGraphicsDevice? GraphicsDevice { get; private set; }
        public CompositionDrawingSurface? DrawingSurface { get; private set; }
        public SpriteVisual? SpriteVisual { get; private set; }

        private readonly long _fd, _fs, _i, _is;

        public Context(GenericIcon element, float logical)
        {
            Initialize(element, logical, logical * GetScaleFactor(element));
            if (_fd == 0) _fd = element.RegisterPropertyChangedCallback(GenericIcon.FlowDirectionProperty, OnTargetPropertyChanged);
            if (_fs == 0) _fs = element.RegisterPropertyChangedCallback(GenericIcon.FontSizeProperty, OnTargetPropertyChanged);
            //// https://github.com/microsoft/microsoft-ui-xaml/issues/6608
            //element.RegisterPropertyChangedCallback(GenericIcon.IsTextScaleFactorEnabledProperty, OnTargetPropertyChanged);
            if (element is FluentIcon fi)
            {
                if (_i == 0) _i = fi.RegisterPropertyChangedCallback(FluentIcon.IconProperty, OnTargetPropertyChanged);
                if (_is == 0) _is = fi.RegisterPropertyChangedCallback(FluentIcon.IconSizeProperty, OnTargetPropertyChanged);
            }
            else if (element is SymbolIcon si)
            {
                if (_i == 0) _i = si.RegisterPropertyChangedCallback(SymbolIcon.SymbolProperty, OnTargetPropertyChanged);
            }
        }

        public void Initialize(GenericIcon element, float logical, float physical)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            var compositor = visual.Compositor;
            var canvas = CanvasDevice.GetSharedDevice();
            GraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvas);
            SpriteVisual = compositor.CreateSpriteVisual();
            ResetCore(logical, physical);
            ElementCompositionPreview.SetElementChildVisual(element, SpriteVisual);
        }

        public void Reset(float logical, float physical)
        {
            var brush = SpriteVisual?.Brush;
            SpriteVisual?.Brush = null;
            brush?.Dispose();
            DrawingSurface?.Dispose();
            ResetCore(logical, physical);
        }
        private void ResetCore(float logical, float physical)
        {
            DrawingSurface = GraphicsDevice!.CreateDrawingSurface(
                new Size(physical, physical),
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                DirectXAlphaMode.Premultiplied);
            SpriteVisual!.Size = new Vector2(logical, logical);
            SpriteVisual.Brush = GraphicsDevice.Compositor.CreateSurfaceBrush(DrawingSurface);
        }

        public void Clear(GenericIcon element)
        {
            if (_fd != 0) element.UnregisterPropertyChangedCallback(GenericIcon.FlowDirectionProperty, _fd);
            if (_fs != 0) element.UnregisterPropertyChangedCallback(GenericIcon.FontSizeProperty, _fs);
            if (element is FluentIcon fi)
            {
                if (_i != 0) fi.UnregisterPropertyChangedCallback(FluentIcon.IconProperty, _i);
                if (_is != 0) fi.UnregisterPropertyChangedCallback(FluentIcon.IconSizeProperty, _is);
            }
            else if (element is SymbolIcon si)
            {
                if (_i != 0) si.UnregisterPropertyChangedCallback(SymbolIcon.SymbolProperty, _i);
            }

            ElementCompositionPreview.SetElementChildVisual(element, null);
            SpriteVisual?.Clip?.Dispose();
            SpriteVisual?.Dispose();
            DrawingSurface?.Dispose();
            GraphicsDevice?.Dispose();
            SpriteVisual = null;
            DrawingSurface = null;
            GraphicsDevice = null;
        }
    }

    private static void OnTargetLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is GenericIcon element)
        {
            var context = GetContext(element);
            if (context is null)
            {
                element.SizeChanged -= OnTargetSizeChanged;
                element.Unloaded -= OnTargetUnloaded;
                element.SizeChanged += OnTargetSizeChanged;
                element.Unloaded += OnTargetUnloaded;
                SetContext(element, new Context(element, (float)element.FontSize));
            }
            Update(element);
        }
    }

    private static void OnTargetUnloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is GenericIcon element)
        {
            element.SizeChanged -= OnTargetSizeChanged;
            element.Unloaded -= OnTargetUnloaded;
            GetContext(element)?.Clear(element);
            SetContext(element, null);
        }
    }

    private static void OnTargetSizeChanged(object? sender, SizeChangedEventArgs e)
        => OnTargetSizeChanged(sender, e.NewSize);
    private static void OnTargetSizeChanged(object? sender, Size newSize)
    {
        if (sender is GenericIcon element)
        {
            var size = (float)element.FontSize;
            var sprite = GetContext(element)?.SpriteVisual;
            if (sprite is not null)
            {
                sprite.Offset = new Vector3(
                    Math.Max(0, ((float)element.ActualWidth - size) / 2),
                    ((float)element.ActualHeight - size) / 2,
                    0);
                if (newSize.Width < size || newSize.Height < size)
                {
                    var clip = sprite.Clip is InsetClip ic ? ic : sprite.Compositor.CreateInsetClip();
                    clip.RightInset = Math.Max(0, size - (float)newSize.Width);
                    clip.TopInset = clip.BottomInset = Math.Max(0, size - (float)newSize.Height) / 2;
                    if (sprite.Clip != clip)
                    {
                        sprite.Clip?.Dispose();
                        sprite.Clip = clip;
                    }
                }
                else
                {
                    sprite.Clip?.Dispose();
                    sprite.Clip = null;
                }
            }
        }
    }

    private static void Update(GenericIcon element)
    {
        var iv = GetIconVariant(element);
        if (element is FluentIcon fi)
        {
            Draw(element,
                FontManager.GetFluent(fi.IconSize, iv).Source,
                (GetIcon(fi) ?? fi.Icon).ToString(
                    GetIconVariant(fi),
                    fi.FlowDirection == FlowDirection.RightToLeft));
        }
        else if (element is SymbolIcon si)
        {
            Draw(element,
                FontManager.GetSeagull().Source,
                (GetSymbol(si) ?? si.Symbol).ToString(
                    GetIconVariant(si),
                    si.FlowDirection == FlowDirection.RightToLeft));
        }
        OnTargetSizeChanged(element, new Size(element.ActualWidth, element.ActualHeight));
    }

    private static void Draw(
        GenericIcon element,
        string fontFamily,
        string text)
    {
        var context = GetContext(element);
        var logical = (float)element.FontSize;
        var scale = GetScaleFactor(element);
        var physical = logical * scale;
        if (context is null)
        {
            System.Diagnostics.Debug.Assert(false); // invalid state
            return;
        }
        else if (context.GraphicsDevice is null)
        {
            context.Initialize(element, logical, physical);
        }
        else if (Math.Abs(context.DrawingSurface!.Size.Height - physical) > 1e-3)
        {
            context.Reset(logical, physical);
        }

        CanvasDrawingSession? session = null;
        CanvasTextFormat? format = null;
        var disposing = true;
        try
        {
            session = CanvasComposition.CreateDrawingSession(
                context.DrawingSurface,
                new(0, 0, physical, physical),
                96 * scale);
            session.Clear(Windows.UI.Color.FromArgb(0, 0, 0, 0));

            var foreground = GetForeground(element);
            if (foreground is null) return;

            // The host will flip the hand-in when RTL, so we need to un-flip it here.
            if (element.FlowDirection  == FlowDirection.RightToLeft)
            {
                session.Transform = new Matrix3x2(-1, 0, 0, 1, logical, 0);
            }

#if WINDOWS_WINAPPSDK
            fontFamily = AppHelper.ResolveFast(fontFamily);
#endif
            format = new CanvasTextFormat
            {
                FontFamily = fontFamily,
                FontSize = logical,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center,
            };
            if (CanvasBrushConverter.TryGetColor(
                session,
                foreground,
                new Size(logical, logical),
                out var color))
            {
                session.DrawText(
                    text,
                    new Vector2(logical / 2, logical / 2),
                    color,
                    format);
            }
            else
            {
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                CanvasBrushConverter.GetBrushAsync(session, foreground, new Size(logical, logical))
                    .ContinueWith(t =>
                    {
                        try
                        {
                            if (t.IsCompletedSuccessfully)
                            {
                                using var brush = t.Result;
                                session.DrawText(
                                    text,
                                    new Vector2(logical / 2, logical / 2),
                                    brush,
                                    format);
                            }
                            else if (t.IsFaulted && t.Exception is not null)
                            {
                                System.Diagnostics.Debug.WriteLine(t.Exception);
                            }
                        }
                        catch(Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                        }
                        finally
                        {
                            session.Dispose();
                            format.Dispose();
                        }
                    }, scheduler);
                disposing = false;
            }
        }
        catch
        {
            GetContext(element)?.Clear(element);
        }
        finally
        {
            if (disposing)
            {
                session?.Dispose();
                format?.Dispose();
            }
        }
    }
#else
    public static bool IsSupported => GenericIcon.CanAddHandIn;

    private sealed partial class Context
    {
        public TextBlock TextBlock { get; set; }

        private long _i, _is;

        public Context(GenericIcon element, Brush? brush)
        {
            TextBlock = new()
            {
                Style = null,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                FontStyle = Windows.UI.Text.FontStyle.Normal,
                FontWeight = FontWeights.Normal,
                Foreground = brush,
            };
            ConnectTo(element);
            element.AddHandIn(TextBlock);
        }

        public void ConnectTo(GenericIcon element)
        {
            TextBlock.SetBinding(
                TextBlock.FlowDirectionProperty,
                new Binding { Source = element, Path = new(nameof(GenericIcon.FlowDirection)) });
            TextBlock.SetBinding(
                TextBlock.FontSizeProperty,
                new Binding { Source = element, Path = new(nameof(GenericIcon.FontSize)) });
            //// https://github.com/microsoft/microsoft-ui-xaml/issues/6608
            //TextBlock.SetBinding(
            //    TextBlock.IsTextScaleFactorEnabledProperty,
            //    new Binding { Source = i, Path = new(nameof(GenericIcon.IsTextScaleFactorEnabled)) });

            if (element is FluentIcon fi)
            {
                if (_i == 0) _i = fi.RegisterPropertyChangedCallback(FluentIcon.IconProperty, OnTargetPropertyChanged);
                if (_is == 0) _is = fi.RegisterPropertyChangedCallback(FluentIcon.IconSizeProperty, OnTargetPropertyChanged);
            }
            else if (element is SymbolIcon si)
            {
                TextBlock!.FontFamily = FontManager.GetSeagull();
                if (_i == 0) _i = si.RegisterPropertyChangedCallback(SymbolIcon.SymbolProperty, OnTargetPropertyChanged);
            }
        }

        public void DisconnectFrom(GenericIcon element)
        {
            TextBlock.ClearValue(TextBlock.FlowDirectionProperty);
            TextBlock.ClearValue(TextBlock.FontSizeProperty);
            //TextBlock.ClearValue(TextBlock.IsTextScaleFactorEnabledProperty);

            if (element is FluentIcon fi)
            {
                if (_i != 0) fi.UnregisterPropertyChangedCallback(FluentIcon.IconProperty, _i);
                if (_is != 0) fi.UnregisterPropertyChangedCallback(FluentIcon.IconSizeProperty, _is);
            }
            else if (element is SymbolIcon si)
            {
                if (_i != 0) si.UnregisterPropertyChangedCallback(SymbolIcon.SymbolProperty, _i);
            }
        }
    }

    private static void OnTargetLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is GenericIcon element)
        {
            var context = GetContext(element);
            if (context is null)
            {
                element.Unloaded -= OnTargetUnloaded;
                element.Unloaded += OnTargetUnloaded;
                SetContext(element, new Context(element, GetForeground(element)));
            }
            else
            {
                context.ConnectTo(element);
            }
            Update(element);
        }
    }

    private static void OnTargetUnloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is GenericIcon element)
        {
            element.Unloaded -= OnTargetUnloaded;
            GetContext(element)?.DisconnectFrom(element);
        }
    }

    private static void Update(GenericIcon element)
    {
        var block = GetContext(element)!.TextBlock;
        var iv = GetIconVariant(element);
        if (element is FluentIcon fi)
        {
            block.FontFamily = FontManager.GetFluent(fi.IconSize, iv);
            block.Text = (GetIcon(fi) ?? fi.Icon).ToString(GetIconVariant(fi), fi.FlowDirection == FlowDirection.RightToLeft);
        }
        else if (element is SymbolIcon si)
        {
            block.Text = (GetSymbol(si) ?? si.Symbol).ToString(GetIconVariant(si), si.FlowDirection == FlowDirection.RightToLeft);
        }
        block.Foreground = GetForeground(element);
    }
#endif
}
