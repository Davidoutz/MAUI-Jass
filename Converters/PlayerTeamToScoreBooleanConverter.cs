using System.Globalization;

namespace MauiJass.Converters;

public class PlayerTeamToScoreBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var team = (int)value;
        var expected = System.Convert.ToInt32(parameter);
        if (team == expected)
            return true;
        else
            return false;


    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? 1 : 0;

    }
}
