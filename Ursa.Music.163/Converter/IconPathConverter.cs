using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Ursa.Music._163.Converter;

public class IconPathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            return PathIcon.LoadFromResource(new Uri((string)value));
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}