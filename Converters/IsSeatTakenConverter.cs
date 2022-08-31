using System;
using System.Collections.Generic;
using System.Globalization;
using MauiJass.Helpers;
using JassWeb.Shared;

namespace MauiJass.Converters;

public class IsSeatTakenConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var players = (Dictionary<int, Player>)value;
        int param = System.Convert.ToInt32(parameter);
        try
        {
            return players.Keys.Contains(param);
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
