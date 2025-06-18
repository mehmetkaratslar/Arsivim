using System.Globalization;
using System.Text;

namespace Arsivim.Shared.Helpers
{
    public static class AramaYardimcisi
    {
        /// <summary>
        /// Türkçe karakterleri normalleştirir ve büyük-küçük harf duyarsız hale getirir
        /// </summary>
        public static string NormalizeSearchTerm(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Türkçe karakterleri İngilizce karşılıklarına çevir
            var normalized = text
                .Replace('ğ', 'g').Replace('Ğ', 'g')
                .Replace('ü', 'u').Replace('Ü', 'u')
                .Replace('ş', 's').Replace('Ş', 's')
                .Replace('ı', 'i').Replace('İ', 'i')
                .Replace('ö', 'o').Replace('Ö', 'o')
                .Replace('ç', 'c').Replace('Ç', 'c')
                .ToLowerInvariant()
                .Trim();

            return normalized;
        }

        /// <summary>
        /// İki metin arasında benzerlik kontrolü yapar
        /// </summary>
        public static bool IsMatch(string text, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(searchTerm))
                return false;

            var normalizedText = NormalizeSearchTerm(text);
            var normalizedSearch = NormalizeSearchTerm(searchTerm);

            // Tam kelime arama
            if (normalizedText.Contains(normalizedSearch))
                return true;

            // Kelime başlangıçları arama
            var words = normalizedText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (word.StartsWith(normalizedSearch))
                    return true;
            }

            // Benzer yazım kontrolü (basit)
            if (normalizedSearch.Length >= 3)
            {
                foreach (var word in words)
                {
                    if (word.Length >= 3 && GetSimilarity(word, normalizedSearch) > 0.7)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// İki string arasındaki benzerlik oranını hesaplar (0-1 arası)
        /// </summary>
        private static double GetSimilarity(string text1, string text2)
        {
            if (text1 == text2) return 1.0;
            if (text1.Length == 0 || text2.Length == 0) return 0.0;

            int maxLength = Math.Max(text1.Length, text2.Length);
            int distance = GetLevenshteinDistance(text1, text2);
            
            return 1.0 - (double)distance / maxLength;
        }

        /// <summary>
        /// Levenshtein Distance algoritması
        /// </summary>
        private static int GetLevenshteinDistance(string text1, string text2)
        {
            int[,] matrix = new int[text1.Length + 1, text2.Length + 1];

            for (int i = 0; i <= text1.Length; i++)
                matrix[i, 0] = i;
            
            for (int j = 0; j <= text2.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= text1.Length; i++)
            {
                for (int j = 1; j <= text2.Length; j++)
                {
                    int cost = text1[i - 1] == text2[j - 1] ? 0 : 1;
                    
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[text1.Length, text2.Length];
        }

        /// <summary>
        /// Arama metnindeki özel karakterleri temizler
        /// </summary>
        public static string CleanSearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return string.Empty;

            // Özel karakterleri kaldır, sadece harf, rakam ve boşluk bırak
            var cleaned = new StringBuilder();
            foreach (char c in searchTerm)
            {
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                {
                    cleaned.Append(c);
                }
            }

            return cleaned.ToString().Trim();
        }
    }
} 