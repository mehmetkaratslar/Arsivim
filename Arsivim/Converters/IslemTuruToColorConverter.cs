using System.Globalization;

namespace Arsivim.Converters
{
    public class IslemTuruToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string islemTuru)
            {
                return islemTuru.ToLowerInvariant() switch
                {
                    "belge ekleme" => Color.FromArgb("#38A169"),
                    "belge silme" => Color.FromArgb("#E53E3E"),
                    "belge güncelleme" => Color.FromArgb("#3182CE"),
                    "kişi ekleme" => Color.FromArgb("#805AD5"),
                    "kişi silme" => Color.FromArgb("#D69E2E"),
                    "etiket oluşturma" => Color.FromArgb("#DD6B20"),
                    "sistem girişi" => Color.FromArgb("#38A169"),
                    "sistem çıkışı" => Color.FromArgb("#718096"),
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