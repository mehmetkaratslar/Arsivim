namespace Arsivim.Shared.Constants
{
    /// <summary>
    /// Uygulama genelindeki sabit değerler
    /// </summary>
    public static class Sabitler
    {
        // Veritabanı
        public const string VeritabaniAdi = "arsivim.db";
        public const string VeritabaniKlasoru = "Arsivim";

        // Dosya boyut limitleri
        public const long MaksimumDosyaBoyutu = 50 * 1024 * 1024; // 50MB
        public const long MinimumDosyaBoyutu = 1; // 1 byte

        // Sayfalama
        public const int VarsayilanSayfaBoyutu = 25;
        public const int MaksimumSayfaBoyutu = 100;

        // Arama
        public const int MinimumAramaKarakterSayisi = 2;
        public const int MaksimumAramaSonucSayisi = 500;

        // Oturum
        public const int OturumSuresiDakika = 60;
        public const int MaxBasarisizGirisDenemesi = 5;
        public const int HesapKilitSuresiDakika = 30;

        // Yedekleme
        public const int VarsayilanYedeklemeAraligi = 7; // gün
        public const int MaksimumYedekSayisi = 10;

        // Tema
        public const string VarsayilanTema = "Acik";
        public const string KoyuTema = "Koyu";

        // Formatlar
        public const string TarihFormati = "dd.MM.yyyy";
        public const string SaatFormati = "HH:mm";
        public const string TarihSaatFormati = "dd.MM.yyyy HH:mm";

        // İzinli dosya uzantıları
        public static readonly string[] IzinliDosyaUzantilari = new[]
        {
            ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff",
            ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            ".txt", ".xml", ".json", ".zip", ".rar", ".7z",
            ".mp3", ".wav", ".flac", ".mp4", ".avi", ".mkv", ".mov"
        };

        // Hata mesajları
        public static class HataMesajlari
        {
            public const string GenelHata = "Bir hata oluştu. Lütfen tekrar deneyin.";
            public const string DosyaBulunamadi = "Belge bulunamadı.";
            public const string DosyaCokBuyuk = "Dosya boyutu çok büyük.";
            public const string GecersizDosyaTipi = "Geçersiz dosya tipi.";
            public const string BosBelgeAdi = "Belge adı boş olamaz.";
            public const string YetkisizErisim = "Bu işlem için yetkiniz bulunmuyor.";
            public const string OturumSuresiDolmus = "Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.";
        }

        // Başarı mesajları
        public static class BasariMesajlari
        {
            public const string BelgeEklendi = "Belge başarıyla eklendi.";
            public const string BelgeGuncellendi = "Belge başarıyla güncellendi.";
            public const string BelgeSilindi = "Belge başarıyla silindi.";
            public const string YedeklemeOlusturuldu = "Yedekleme başarıyla oluşturuldu.";
            public const string AyarlarKaydedildi = "Ayarlar başarıyla kaydedildi.";
        }

        // Uygulama bilgileri
        public static class UygulamaBilgileri
        {
            public const string Ad = "Arsivim";
            public const string Versiyon = "1.0.0";
            public const string Aciklama = "Kişisel Belge Arşiv Sistemi";
            public const string Gelistirici = "Arsivim Ekibi";
        }

        // Log seviyeleri
        public enum LogSeviye
        {
            Bilgi,
            Uyari,
            Hata,
            Kritik
        }

        // Belge durumları
        public enum BelgeDurumu
        {
            Aktif,
            Arsivlendi,
            Silindi
        }
    }
} 