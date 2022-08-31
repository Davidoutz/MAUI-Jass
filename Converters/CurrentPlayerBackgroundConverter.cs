using System.Globalization;

namespace MauiJass.Converters;

public class CurrentPlayerBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isSelected = (bool)value;

        if (isSelected)
            return Colors.Beige;
        else
            return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

        return (bool)value ? 1 : 0;
    }
}