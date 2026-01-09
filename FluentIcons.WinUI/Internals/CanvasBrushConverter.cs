#if WINDOWS_WINAPPSDK || WINDOWS_UWP
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.Graphics.DirectX;

#if WINDOWS_WINAPPSDK
using Microsoft.Windows.ApplicationModel.Resources;
#else
using Windows.ApplicationModel.Resources.Core;
#endif

#if WINDOWS_WINAPPSDK
namespace FluentIcons.WinUI.Internals;
#else
namespace FluentIcons.Uwp.Internals;
#endif

internal static class CanvasBrushConverter
{
    public static async Task<ICanvasBrush?> GetBrushAsync(
        ICanvasResourceCreator device, Brush source, Size size)
    {
        if (source is LinearGradientBrush linear)
        {
            return new CanvasLinearGradientBrush(device,
                Convert(linear.GradientStops),
                Convert(linear.SpreadMethod),
                default)
            {
                StartPoint = Convert(linear.StartPoint, size, linear.MappingMode),
                EndPoint = Convert(linear.EndPoint, size, linear.MappingMode),
                Opacity = (float)linear.Opacity,
                Transform = Convert(linear.RelativeTransform, linear.Transform, size),
            };
        }
#if WINDOWS_WINAPPSDK
        else if (source is RadialGradientBrush radial)
        {
            var center = Convert(radial.Center, size, radial.MappingMode);
            return new CanvasRadialGradientBrush(device,
                Convert(radial.GradientStops),
                Convert(radial.SpreadMethod),
                default)
            {
                Center = center,
                RadiusX = Convert(radial.RadiusX, size.Width, radial.MappingMode),
                RadiusY = Convert(radial.RadiusY, size.Height, radial.MappingMode),
                OriginOffset = Convert(radial.GradientOrigin, size, radial.MappingMode) - center,
                Opacity = (float)radial.Opacity,
                Transform = Convert(radial.RelativeTransform, radial.Transform, size),
            };
        }
#endif
        else if (source is ImageBrush image)
        {
            if (await ConvertAsync(device, image.ImageSource) is ICanvasImage img)
            {
                var bounds = img.GetBounds(device);
                return new CanvasImageBrush(device, img)
                {
                    ExtendX = CanvasEdgeBehavior.Clamp,
                    ExtendY = CanvasEdgeBehavior.Clamp,
                    Opacity = (float)image.Opacity,
                    Interpolation = CanvasImageInterpolation.HighQualityCubic,
                    Transform = Convert(image.RelativeTransform, image.Transform, size)
                              * Convert(new Size(bounds.Width, bounds.Height), size, image.Stretch, image.AlignmentX, image.AlignmentY),
                };
            }
            else
            {
                return null;
            }
        }
        else if (TryGetColor(device, source, size, out var color))
        {
            return new CanvasSolidColorBrush(device, color);
        }
        else
        {
            return null;
        }
    }

    public static bool TryGetColor(
        ICanvasResourceCreator device, Brush source, Size size,
        out Windows.UI.Color color)
    {
        color = default;
        if (source is SolidColorBrush solid)
        {
            color = solid.Color;
            if (solid.Opacity < 1.0)
            {
                color.A = (byte)(color.A * solid.Opacity);
            }
            return true;
        }
        else if (source is LinearGradientBrush)
        {
            return false;
        }
#if WINDOWS_WINAPPSDK
        else if (source is RadialGradientBrush)
        {
            return false;
        }
#endif
        else if (source is ImageBrush)
        {
            return false;
        }
        else if (source is XamlCompositionBrushBase composition)
        {
            color = composition.FallbackColor;
            if (composition.Opacity < 1.0)
            {
                color.A = (byte)(color.A * composition.Opacity);
            }
            return true;
        }
        else
        {
            return true;
        }
    }

    private static CanvasGradientStop[] Convert(IEnumerable<GradientStop>? stops)
        => stops?.Select(s => new CanvasGradientStop
        {
            Color = s.Color,
            Position = (float)s.Offset,
        })?.ToArray() ?? [];

    private static CanvasEdgeBehavior Convert(GradientSpreadMethod method)
        => method switch
        {
            GradientSpreadMethod.Reflect => CanvasEdgeBehavior.Mirror,
            GradientSpreadMethod.Repeat => CanvasEdgeBehavior.Wrap,
            _ => CanvasEdgeBehavior.Clamp,
        };

    private static float Convert(double value, double size, BrushMappingMode mapping)
        => mapping == BrushMappingMode.RelativeToBoundingBox
            ? (float)(value * size)
            : (float)value;

    private static Vector2 Convert(Point point, Size size, BrushMappingMode mapping)
        => mapping == BrushMappingMode.RelativeToBoundingBox
            ? new((float)(point.X * size.Width), (float)(point.Y * size.Height))
            : new((float)point.X, (float)point.Y);

    private static Matrix3x2 Convert(Transform? relative, Transform? absolute, Size size)
    {
        var matrix = Matrix3x2.Identity;
        if (relative is not null)
        {
            var m = Convert(relative);
            var w = (float)size.Width;
            var h = (float)size.Height;
            if (!m.IsIdentity && w > 0 && h > 0)
            {
                matrix = Matrix3x2.CreateScale(1 / w, 1 / h) * m * Matrix3x2.CreateScale(w, h);
            }
        }
        if (absolute is not null)
        {
            var m = Convert(absolute);
            if (!m.IsIdentity) {
                matrix *= m;
            }
        }
        return matrix;
    }

    private static Matrix3x2 Convert(Transform transform)
    {
        if (transform is RotateTransform rotate)
        {
            return Matrix3x2.CreateRotation(
                (float)(rotate.Angle * Math.PI / 180.0),
                new((float)rotate.CenterX, (float)rotate.CenterY));
        }
        else if (transform is ScaleTransform scale)
        {
            return Matrix3x2.CreateScale(
                (float)scale.ScaleX,
                (float)scale.ScaleY,
                new((float)scale.CenterX, (float)scale.CenterY));
        }
        else if (transform is SkewTransform skew)
        {
            return Matrix3x2.CreateSkew(
                (float)(skew.AngleX * Math.PI / 180.0),
                (float)(skew.AngleY * Math.PI / 180.0),
                new((float)skew.CenterX, (float)skew.CenterY));
        }
        else if (transform is TranslateTransform translate)
        {
            return Matrix3x2.CreateTranslation(
                (float)translate.X,
                (float)translate.Y);
        }
        else if (transform is MatrixTransform matrix)
        {
            var m = matrix.Matrix;
            return new Matrix3x2(
                (float)m.M11, (float)m.M12,
                (float)m.M21, (float)m.M22,
                (float)m.OffsetX, (float)m.OffsetY);
        }
        else if (transform is CompositeTransform composite)
        {
            var result = Matrix3x2.Identity;
            result *= Matrix3x2.CreateTranslation(-(float)composite.CenterX, -(float)composite.CenterY);
            result *= Matrix3x2.CreateScale((float)composite.ScaleX, (float)composite.ScaleY);
            result *= Matrix3x2.CreateSkew(
                (float)(composite.SkewX * Math.PI / 180.0),
                (float)(composite.SkewY * Math.PI / 180.0));
            result *= Matrix3x2.CreateRotation(
                (float)(composite.Rotation * Math.PI / 180.0));
            result *= Matrix3x2.CreateTranslation(
                (float)(composite.TranslateX + composite.CenterX),
                (float)(composite.TranslateY + composite.CenterY));
            return result;
        }
        else if (transform is TransformGroup group)
        {
            var result = Matrix3x2.Identity;
            foreach (var child in group.Children)
            {
                result *= Convert(child);
            }
            return result;
        }
        else
        {
            return Matrix3x2.Identity;
        }
    }

    private static async Task<CanvasBitmap?> ConvertAsync(ICanvasResourceCreator device, ImageSource source)
    {
        if (source is BitmapImage bitmap && bitmap.UriSource is not null)
        {
            return await ConvertBitmapAsync(device, bitmap.UriSource);
        }
        else if (source is SvgImageSource svg && svg.UriSource is not null)
        {
            return await ConvertBitmapAsync(device, svg.UriSource);
        }
        else if (source is WriteableBitmap writeable)
        {
            return CanvasBitmap.CreateFromBytes(device,
                writeable.PixelBuffer,
                writeable.PixelWidth,
                writeable.PixelHeight,
                DirectXPixelFormat.B8G8R8A8UIntNormalized);
        }
        else
        {
            return null;
        }
    }

    private static async Task<CanvasBitmap?> ConvertBitmapAsync(ICanvasResourceCreator device, Uri uri)
    {
        uri = AppHelper.Resolve(uri);
        if (uri.IsFile)
        {
            return await CanvasBitmap.LoadAsync(device, uri.LocalPath);
        }
        return await CanvasBitmap.LoadAsync(device, uri);
    }

    private static Matrix3x2 Convert(Size source, Size target, Stretch stretch, AlignmentX alignX, AlignmentY alignY)
    {
        var x = source.Width > 0 ? (float)(target.Width / source.Width) : 1f;
        var y = source.Height > 0 ? (float)(target.Height / source.Height) : 1f;
        var scaleX = stretch switch
        {
            Stretch.Fill => x,
            Stretch.Uniform => Math.Min(x, y),
            Stretch.UniformToFill => Math.Max(x, y),
            _ => 1f,
        };
        var scaleY = stretch == Stretch.Fill ? y : scaleX;
        var translateX = alignX switch
        {
            AlignmentX.Center => (float)(target.Width - source.Width * scaleX) / 2f,
            AlignmentX.Right => (float)(target.Width - source.Width * scaleX),
            _ => 0f,
        };
        var translateY = alignY switch
        {
            AlignmentY.Center => (float)(target.Height - source.Height * scaleY) / 2f,
            AlignmentY.Bottom => (float)(target.Height - source.Height * scaleY),
            _ => 0f,
        };

        return Matrix3x2.CreateScale(scaleX, scaleY) * Matrix3x2.CreateTranslation(translateX, translateY);
    }
}
#endif
