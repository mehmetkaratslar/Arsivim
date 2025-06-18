using Arsivim.Core.Models;

namespace Arsivim.Core.Interfaces
{
    /// <summary>
    /// Güvenlik işlemleri için arayüz
    /// </summary>
    public interface IGuvenlik
    {
        /// <summary>
        /// Kullanıcı girişi yapar
        /// </summary>
        Task<(bool basarili, Kullanici? kullanici, string mesaj)> GirisYapAsync(string kullaniciAdi, string sifre);

        /// <summary>
        /// Kullanıcı çıkışı yapar
        /// </summary>
        Task CikisYapAsync();

        /// <summary>
        /// Şifre doğrular
        /// </summary>
        Task<bool> SifreDogrulaAsync(string sifre, string hash, string? salt = null);

        /// <summary>
        /// Şifre hash'ler
        /// </summary>
        Task<(string hash, string salt)> SifreHashleAsync(string sifre);

        /// <summary>
        /// Güçlü şifre oluşturur
        /// </summary>
        string GucluSifreOlustur(int uzunluk = 12);

        /// <summary>
        /// Şifre güçlülüğünü kontrol eder
        /// </summary>
        (bool guclu, string mesaj) SifreGucluluguKontrolEt(string sifre);

        /// <summary>
        /// Dosya şifreler
        /// </summary>
        Task<byte[]> DosyaSifreleAsync(byte[] veri, string sifre);

        /// <summary>
        /// Dosya şifresini çözer
        /// </summary>
        Task<byte[]> DosyaSifreCozAsync(byte[] sifreliVeri, string sifre);

        /// <summary>
        /// Hash değeri hesaplar
        /// </summary>
        string HashHesapla(byte[] veri);

        /// <summary>
        /// Hash değeri doğrular
        /// </summary>
        bool HashDogrula(byte[] veri, string beklenenHash);

        /// <summary>
        /// Mevcut kullanıcıyı döndürür
        /// </summary>
        Task<Kullanici?> MevcutKullaniciAsync();

        /// <summary>
        /// Kullanıcının giriş yapıp yapmadığını kontrol eder
        /// </summary>
        bool GirisYapildiMi();

        /// <summary>
        /// Oturum süresini uzatır
        /// </summary>
        Task OturumUzatAsync();

        /// <summary>
        /// Oturum süresini kontrol eder
        /// </summary>
        Task<bool> OturumGecerliligiKontrolEtAsync();

        /// <summary>
        /// İki faktörlü kimlik doğrulama kodu oluşturur
        /// </summary>
        string IkiFaktorKoduOlustur();

        /// <summary>
        /// İki faktörlü kimlik doğrulama kodunu doğrular
        /// </summary>
        bool IkiFaktorKoduDogrula(string kod, string beklenenKod);

        /// <summary>
        /// Hesap kilitlenme durumunu kontrol eder
        /// </summary>
        Task<bool> HesapKilitliMiAsync(string kullaniciAdi);

        /// <summary>
        /// Başarısız giriş denemesini kaydeder
        /// </summary>
        Task BasarisizGirisKaydetAsync(string kullaniciAdi);

        /// <summary>
        /// Hesap kilidi açar
        /// </summary>
        Task HesapKilidiAcAsync(string kullaniciAdi);
    }
} 