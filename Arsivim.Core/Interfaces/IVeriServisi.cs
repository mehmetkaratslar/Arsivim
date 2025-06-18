using Arsivim.Core.Models;

namespace Arsivim.Core.Interfaces
{
    /// <summary>
    /// Veri işlemleri için temel arayüz
    /// </summary>
    public interface IVeriServisi
    {
        // Belge işlemleri
        Task<IEnumerable<Belge>> BelgeleriGetirAsync();
        Task<Belge?> BelgeGetirAsync(int belgeId);
        Task<bool> BelgeEkleAsync(Belge belge);
        Task<bool> BelgeGuncelleAsync(Belge belge);
        Task<bool> BelgeSilAsync(int belgeId);
        Task<IEnumerable<Belge>> BelgeAraAsync(string aramaMetni);
        Task<IEnumerable<Belge>> BelgeleriTipinaGoreGetirAsync(Enums.BelgeTipi tip);

        // Kişi işlemleri
        Task<IEnumerable<Kisi>> KisileriGetirAsync();
        Task<Kisi?> KisiGetirAsync(int kisiId);
        Task<bool> KisiEkleAsync(Kisi kisi);
        Task<bool> KisiGuncelleAsync(Kisi kisi);
        Task<bool> KisiSilAsync(int kisiId);
        Task<IEnumerable<Kisi>> KisiAraAsync(string aramaMetni);

        // Etiket işlemleri
        Task<IEnumerable<Etiket>> EtiketleriGetirAsync();
        Task<Etiket?> EtiketGetirAsync(int etiketId);
        Task<bool> EtiketEkleAsync(Etiket etiket);
        Task<bool> EtiketGuncelleAsync(Etiket etiket);
        Task<bool> EtiketSilAsync(int etiketId);

        // Favori işlemleri
        Task<IEnumerable<Favori>> FavorileriGetirAsync();
        Task<bool> FavoriyeEkleAsync(int belgeId, string? not = null);
        Task<bool> FavoridenCikarAsync(int belgeId);
        Task<bool> FavorimiAsync(int belgeId);

        // Not işlemleri
        Task<IEnumerable<Not>> NotlariGetirAsync();
        Task<Not?> NotGetirAsync(int notId);
        Task<bool> NotEkleAsync(Not not);
        Task<bool> NotGuncelleAsync(Not not);
        Task<bool> NotSilAsync(int notId);
        Task<IEnumerable<Not>> BelgeNotlariGetirAsync(int belgeId);

        // Geçmiş işlemleri
        Task<IEnumerable<Gecmis>> GecmisGetirAsync(int sayfa = 1, int sayfalamaBoyutu = 50);
        Task<bool> GecmisEkleAsync(Gecmis gecmis);
        Task<bool> GecmisTemizleAsync(DateTime tarihinden);

        // İstatistik işlemleri
        Task<int> ToplamBelgeSayisiAsync();
        Task<int> ToplamKisiSayisiAsync();
        Task<long> ToplamDosyaBoyutuAsync();
        Task<IEnumerable<Belge>> SonEklenenBelgelerAsync(int adet = 10);
        Task<IEnumerable<Belge>> EnCokGoruntulenenBelgelerAsync(int adet = 10);

        // Backup işlemleri
        Task<bool> VeritabaniYedekleAsync(string yol);
        Task<bool> VeritabaniGeriYukleAsync(string yol);
    }
} 