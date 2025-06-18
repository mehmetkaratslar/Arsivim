using System.Globalization;

namespace Arsivim.Converters
{
    public class CountToOpacityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                // If count is 0, return low opacity (0.3), otherwise full opacity (1.0)
                return count > 0 ? 1.0 : 0.3;
            }
            
            // Default to medium opacity if value is not an integer
            return 0.7;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException("CountToOpacityConverter is a one-way converter.");
        }
    }
} 