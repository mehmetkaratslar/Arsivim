using System.Globalization;

namespace Arsivim.Converters
{
    public class IslemTuruToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string islemTuru)
            {
                return islemTuru.ToLowerInvariant() switch
                {
                    "belge ekleme" => "📄",
                    "belge silme" => "🗑️",
                    "belge güncelleme" => "✏️",
                    "kişi ekleme" => "👤",
                    "kişi silme" => "❌",
                    "etiket oluşturma" => "🏷️",
                    "sistem girişi" => "🔑",
                    "sistem çıkışı" => "🚪",
                    _ => "📋"
                };
            }
            return "📋";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 