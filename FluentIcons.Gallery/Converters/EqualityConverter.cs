using System.Globalization;
using Avalonia.Data.Converters;

namespace FluentIcons.Gallery.Converters;

internal class EqualityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.Skip(1).All(x => Equals(x, values[0]));
    }
}
