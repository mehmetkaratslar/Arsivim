using System.Globalization;

namespace Arsivim.Converters
{
    public class BoolToFavoriteColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool favorimi)
            {
                return favorimi ? Colors.Red : Colors.Gray;
            }
            
            return Colors.Gray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException("BoolToFavoriteColorConverter is a one-way converter.");
        }
    }
} 