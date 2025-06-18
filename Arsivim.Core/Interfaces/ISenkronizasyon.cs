namespace Arsivim.Core.Interfaces
{
    /// <summary>
    /// LAN senkronizasyon işlemleri için arayüz
    /// </summary>
    public interface ISenkronizasyon
    {
        /// <summary>
        /// Senkronizasyon başlatır
        /// </summary>
        Task<bool> SenkronizasyonBaslatAsync();

        /// <summary>
        /// Senkronizasyon durdurur
        /// </summary>
        Task SenkronizasyonDurdurAsync();

        /// <summary>
        /// LAN'daki diğer cihazları tarar
        /// </summary>
        Task<IEnumerable<string>> CihazlariTaraAsync();

        /// <summary>
        /// Belirtilen cihazla senkronizasyon yapar
        /// </summary>
        Task<bool> CihazlaSenkronizeEtAsync(string ipAdresi);

        /// <summary>
        /// Değişiklikleri diğer cihazlara gönderir
        /// </summary>
        Task<bool> DegisiklikleriGonderAsync();

        /// <summary>
        /// Diğer cihazlardan değişiklikleri alır
        /// </summary>
        Task<bool> DegisiklikleriAlAsync();

        /// <summary>
        /// Çakışma tespiti yapar
        /// </summary>
        Task<bool> CakismaVarMiAsync();

        /// <summary>
        /// Çakışmaları çözer
        /// </summary>
        Task<bool> CakismalariCozAsync();

        /// <summary>
        /// Son senkronizasyon zamanını döndürür
        /// </summary>
        Task<DateTime?> SonSenkronizasyonZamaniAsync();

        /// <summary>
        /// Otomatik senkronizasyon durumunu ayarlar
        /// </summary>
        Task<bool> OtomatikSenkronizasyonAyarlaAsync(bool aktif, TimeSpan aralik);

        /// <summary>
        /// Senkronizasyon durumunu döndürür
        /// </summary>
        Task<bool> SenkronizasyonDurumuAsync();

        /// <summary>
        /// Senkronizasyon günlüğünü döndürür
        /// </summary>
        Task<IEnumerable<string>> SenkronizasyonGunluguAsync();
    }
} 