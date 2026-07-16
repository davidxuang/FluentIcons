using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace FluentIcons.Gallery.Converters;

internal class CoalesceConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.FirstOrDefault(x => x is not null && x != AvaloniaProperty.UnsetValue) ?? parameter;
    }
}
