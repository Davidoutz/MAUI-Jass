using System.Globalization;
using JassWeb.Shared;

namespace MauiJass.Converters;

public class IsSeatFreeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var players = (Dictionary<int, Player>)value;
        int param = System.Convert.ToInt32(parameter);
        try
        {
            return !players.Keys.Contains(param);
        }
        catch
        {
            return false;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

        return (bool)value ? 1 : 0;
    }
}
