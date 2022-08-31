using System.Globalization;
using JassWeb.Shared;

namespace MauiJass.Converters;

public class SeatToNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var player = (Player)value;
        if (player == null)
            return string.Empty;
        else
            return player.Name;


    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? 1 : 0;

    }
}
