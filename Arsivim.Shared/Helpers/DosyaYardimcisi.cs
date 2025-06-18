namespace Arsivim.Shared.Helpers
{
    /// <summary>
    /// Dosya işlemleri için yardımcı sınıf
    /// </summary>
    public static class DosyaYardimcisi
    {
        /// <summary>
        /// Dosya boyutunu okunabilir formata çevirir
        /// </summary>
        public static string BoyutFormatla(long boyut)
        {
            string[] birimler = { "B", "KB", "MB", "GB", "TB" };
            double size = boyut;
            int birimIndex = 0;

            while (size >= 1024 && birimIndex < birimler.Length - 1)
            {
                size /= 1024;
                birimIndex++;
            }

            return $"{size:F2} {birimler[birimIndex]}";
        }

        /// <summary>
        /// Dosya uzantısından MIME type belirler
        /// </summary>
        public static string MimeTypeBelirle(string dosyaUzantisi)
        {
            var uzanti = dosyaUzantisi.ToLower().TrimStart('.');
            
            return uzanti switch
            {
                "pdf" => "application/pdf",
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "xls" => "application/vnd.ms-excel",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ppt" => "application/vnd.ms-powerpoint",
                "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "txt" => "text/plain",
                "xml" => "application/xml",
                "json" => "application/json",
                "zip" => "application/zip",
                "rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };
        }

        /// <summary>
        /// Güvenli dosya adı oluşturur
        /// </summary>
        public static string GuvenliDosyaAdiOlustur(string dosyaAdi)
        {
            var gecersizKarakterler = Path.GetInvalidFileNameChars();
            var temizAd = new string(dosyaAdi.Where(c => !gecersizKarakterler.Contains(c)).ToArray());
            
            if (string.IsNullOrWhiteSpace(temizAd))
            {
                temizAd = "Belge_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }

            return temizAd;
        }

        /// <summary>
        /// Dosya uzantısını kontrol eder
        /// </summary>
        public static bool GecerliDosyaUzantisi(string dosyaUzantisi)
        {
            var izinliUzantilar = new[]
            {
                ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff",
                ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
                ".txt", ".xml", ".json", ".zip", ".rar", ".7z",
                ".mp3", ".wav", ".mp4", ".avi", ".mkv"
            };

            return izinliUzantilar.Contains(dosyaUzantisi.ToLower());
        }

        /// <summary>
        /// Dosyadan hash değeri hesaplar
        /// </summary>
        public static async Task<string> DosyaHashHesaplaAsync(Stream dosyaStream)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = await Task.Run(() => sha256.ComputeHash(dosyaStream));
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
} 