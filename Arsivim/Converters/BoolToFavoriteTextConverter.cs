using System.Globalization;

namespace Arsivim.Converters
{
    public class BoolToFavoriteTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool favorimi)
            {
                return favorimi ? "❤️" : "🤍";
            }
            
            return "🤍";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException("BoolToFavoriteTextConverter is a one-way converter.");
        }
    }
} 