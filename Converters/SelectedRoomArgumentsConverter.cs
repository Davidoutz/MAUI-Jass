using System;
using System.Globalization;
using JassWeb.Shared;

namespace MauiJass.Converters;

public class SelectedRoomArgumentsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (values[0] != null && values[1] != null && values.Length == 2)
        {
            var game = values[0] as Game;
            int param = System.Convert.ToInt32(values[1]);

            return new SelectedRoomArguments {  Game = game, Seat = param };
        }
        return null;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectedRoomArguments
{
    public Game Game { get; set; }
    public int Seat { get; set; }
}
