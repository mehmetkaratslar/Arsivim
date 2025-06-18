using System.Globalization;

namespace Arsivim.Converters
{
    public class BelgeTuruToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string belgeTuru)
            {
                return belgeTuru.ToLowerInvariant() switch
                {
                    "pdf" => Color.FromArgb("#E53E3E"),
                    "word" => Color.FromArgb("#3182CE"),
                    "excel" => Color.FromArgb("#38A169"),
                    "powerpoint" => Color.FromArgb("#D69E2E"),
                    "resim" => Color.FromArgb("#805AD5"),
                    "diğer" => Color.FromArgb("#718096"),
                    _ => Color.FromArgb("#718096")
                };
            }
            return Color.FromArgb("#718096");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 