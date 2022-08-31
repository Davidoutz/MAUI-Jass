using System.Globalization;
using MauiJass.Helpers;
using JassWeb.Shared;

namespace MauiJass.Converters;

public class AtoutToMDIConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        var atout = (SUIT)value;
        switch (atout)
        {
            case SUIT.HEART:
                return MDIconsHelper.CardsPlayingHeart;
            case SUIT.CLUB:
                return MDIconsHelper.CardsPlayingClub;
            case SUIT.DIAMOND:
                return MDIconsHelper.CardsPlayingDiamond;
            case SUIT.SPADE:
                return MDIconsHelper.CardsPlayingSpade;
            default:
                return "";
        }


    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? 1 : 0;

    }
}
