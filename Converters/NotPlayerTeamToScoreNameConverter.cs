using System.Globalization;

namespace MauiJass.Converters;

public class NotPlayerTeamToScoreNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var team = (int)value;
        var expected = System.Convert.ToInt32(parameter);
        if (team == expected)
            return "Vous";
        else
            return "Eux";


    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? 1 : 0;

    }
}