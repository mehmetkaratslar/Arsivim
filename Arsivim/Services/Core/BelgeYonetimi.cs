using Arsivim.Core.Models;
using Arsivim.Core.Enums;
using Arsivim.Data.Repositories;

namespace Arsivim.Services.Core
{
    public class BelgeYonetimi
    {
        private readonly BelgeRepository _belgeRepository;

        public BelgeYonetimi(BelgeRepository belgeRepository)
        {
            _belgeRepository = belgeRepository;
        }

        #region Belge CRUD İşlemleri

        /// <summary>
        /// Yeni belge ekler
        /// </summary>
        public async Task<Belge?> BelgeEkleAsync(Belge belge)
        {
            try
            {
                if (belge == null)
                    throw new ArgumentNullException(nameof(belge));

                // Validasyon
                if (string.IsNullOrWhiteSpace(belge.BelgeAdi))
                    throw new ArgumentException("Belge adı boş olamaz.");

                if (belge.Dosya == null || belge.Dosya.Length == 0)
                    throw new ArgumentException("Belge dosyası boş olamaz.");

                return await _belgeRepository.EkleAsync(belge);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"BelgeEkleAsync Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Belgeyi günceller
        /// </summary>
        public async Task<bool> BelgeGuncelleAsync(Belge belge)
        {
            try
            {
                if (belge == null)
                    throw new ArgumentNullException(nameof(belge));

                belge.SonGuncelleme = DateTime.Now;
                return await _belgeRepository.GuncelleAsync(belge);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BelgeGuncelleAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Belgeyi siler
        /// </summary>
        public async Task<bool> BelgeSilAsync(int belgeId)
        {
            try
            {
                return await _belgeRepository.SilAsync(belgeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BelgeSilAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Belgeyi ID'ye göre getirir
        /// </summary>
        public async Task<Belge?> BelgeGetirAsync(int belgeId)
        {
            try
            {
                var belge = await _belgeRepository.GetirAsync(belgeId);
                
                // Görüntülenme sayısını artır
                if (belge != null)
                {
                    belge.GoruntulenmeSayisi++;
                    belge.SonGoruntulenmeTarihi = DateTime.Now;
                    await _belgeRepository.GuncelleAsync(belge);
                }

                return belge;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BelgeGetirAsync Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tüm belgeleri getirir
        /// </summary>
        public async Task<IEnumerable<Belge>> TumBelgeleriGetirAsync()
        {
            try
            {
                return await _belgeRepository.TumunuGetirAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TumBelgeleriGetirAsync Error: {ex.Message}");
                return new List<Belge>();
            }
        }

        /// <summary>
        /// Belge tipine göre belgeleri getirir
        /// </summary>
        public async Task<IEnumerable<Belge>> TipineGoreBelgeleriGetirAsync(BelgeTipi tip)
        {
            try
            {
                return await _belgeRepository.TipineGoreGetirAsync(tip);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TipineGoreBelgeleriGetirAsync Error: {ex.Message}");
                return new List<Belge>();
            }
        }

        /// <summary>
        /// Belge arar
        /// </summary>
        public async Task<IEnumerable<Belge>> BelgeAraAsync(string aramaMetni)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(aramaMetni))
                    return await TumBelgeleriGetirAsync();

                return await _belgeRepository.AraAsync(aramaMetni);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BelgeAraAsync Error: {ex.Message}");
                return new List<Belge>();
            }
        }

        #endregion

        #region Favori İşlemleri

        /// <summary>
        /// Belgeyi favorilere ekler
        /// </summary>
        public async Task<bool> FavoriyeEkleAsync(int belgeId)
        {
            try
            {
                var belge = await _belgeRepository.GetirAsync(belgeId);
                if (belge == null) return false;

                // Zaten favoride mi kontrol et
                if (belge.Favori != null) return true;

                var favori = new Favori
                {
                    BelgeID = belgeId,
                    Tarih = DateTime.Now
                };

                // Bu implementation repository'de yapılmalı
                // Şimdilik basit bir yaklaşım
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FavoriyeEkleAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Belgeyi favorilerden çıkarır
        /// </summary>
        public async Task<bool> FavoridanCikarAsync(int belgeId)
        {
            try
            {
                // Bu implementation repository'de yapılmalı
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FavoridanCikarAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Favori belgeleri getirir
        /// </summary>
        public async Task<IEnumerable<Belge>> FavoriBelgeleriGetirAsync()
        {
            try
            {
                var tumBelgeler = await TumBelgeleriGetirAsync();
                return tumBelgeler.Where(b => b.Favori != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FavoriBelgeleriGetirAsync Error: {ex.Message}");
                return new List<Belge>();
            }
        }

        #endregion

        #region Dosya İşlemleri

        /// <summary>
        /// Belge dosyasını indirir
        /// </summary>
        public async Task<byte[]?> BelgeDosyasiniIndirAsync(int belgeId)
        {
            try
            {
                var belge = await _belgeRepository.GetirAsync(belgeId);
                return belge?.Dosya;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BelgeDosyasiniIndirAsync Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Belge dosyasını paylaş
        /// </summary>
        public async Task<string?> BelgePaylasAsync(int belgeId)
        {
            try
            {
                var belge = await _belgeRepository.GetirAsync(belgeId);
                if (belge?.Dosya == null) return null;

                // Geçici dosya oluştur
                var tempPath = Path.GetTempFileName();
                var fileName = $"{belge.BelgeAdi}{belge.DosyaTipi}";
                var fullPath = Path.Combine(Path.GetDirectoryName(tempPath)!, fileName);

                await File.WriteAllBytesAsync(fullPath, belge.Dosya);
                return fullPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BelgePaylasAsync Error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Not İşlemleri

        /// <summary>
        /// Belgeye not ekler
        /// </summary>
        public async Task<bool> NotEkleAsync(Not not)
        {
            try
            {
                if (not == null)
                    throw new ArgumentNullException(nameof(not));

                // Bu implementation repository'de yapılmalı
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NotEkleAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Notu siler
        /// </summary>
        public async Task<bool> NotSilAsync(int notId)
        {
            try
            {
                // Bu implementation repository'de yapılmalı
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NotSilAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Belgeye ait notları getirir
        /// </summary>
        public async Task<IEnumerable<Not>> BelgeNotlariGetirAsync(int belgeId)
        {
            try
            {
                // Bu implementation repository'de yapılmalı
                return new List<Not>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BelgeNotlariGetirAsync Error: {ex.Message}");
                return new List<Not>();
            }
        }

        #endregion

        #region Ana Sayfa İçin Özel Metodlar

        /// <summary>
        /// Son eklenen belgeleri getirir
        /// </summary>
        public async Task<IEnumerable<Belge>> SonEklenenBelgeleriGetirAsync(int adet = 5)
        {
            try
            {
                var belgeler = await TumBelgeleriGetirAsync();
                return belgeler.OrderByDescending(b => b.YuklemeTarihi).Take(adet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SonEklenenBelgeleriGetirAsync Error: {ex.Message}");
                return new List<Belge>();
            }
        }

        /// <summary>
        /// En çok görüntülenen belgeleri getirir
        /// </summary>
        public async Task<IEnumerable<Belge>> EnCokGoruntulenenBelgeleriGetirAsync(int adet = 5)
        {
            try
            {
                var belgeler = await TumBelgeleriGetirAsync();
                return belgeler.OrderByDescending(b => b.GoruntulenmeSayisi).Take(adet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EnCokGoruntulenenBelgeleriGetirAsync Error: {ex.Message}");
                return new List<Belge>();
            }
        }

        #endregion

        #region İstatistik İşlemleri

        /// <summary>
        /// Toplam belge sayısını getirir
        /// </summary>
        public async Task<int> ToplamBelgeSayisiAsync()
        {
            try
            {
                var belgeler = await TumBelgeleriGetirAsync();
                return belgeler.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ToplamBelgeSayisiAsync Error: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Toplam dosya boyutunu getirir
        /// </summary>
        public async Task<long> ToplamDosyaBoyutuAsync()
        {
            try
            {
                var belgeler = await TumBelgeleriGetirAsync();
                return belgeler.Sum(b => b.DosyaBoyutu);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ToplamDosyaBoyutuAsync Error: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Belge tiplerine göre sayıları getirir
        /// </summary>
        public async Task<Dictionary<BelgeTipi, int>> BelgeTipiIstatistikleriAsync()
        {
            try
            {
                var belgeler = await TumBelgeleriGetirAsync();
                return belgeler.GroupBy(b => b.BelgeTipi)
                              .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BelgeTipiIstatistikleriAsync Error: {ex.Message}");
                return new Dictionary<BelgeTipi, int>();
            }
        }

        #endregion
    }
} 