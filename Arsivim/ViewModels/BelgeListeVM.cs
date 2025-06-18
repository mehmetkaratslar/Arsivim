using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Core.Enums;
using Arsivim.Services.Core;
using Arsivim.Shared.Helpers;
using Arsivim.Data.Repositories;
using Microsoft.Maui.ApplicationModel; // Replace Microsoft.Maui.Essentials with this namespace

namespace Arsivim.ViewModels
{
    public class BelgeListeVM : BaseViewModel
    {
        private readonly BelgeYonetimi _belgeYonetimi;
        private readonly KisiRepository _kisiRepository;
        
        public ObservableCollection<Belge> Belgeler { get; } = new();
        public ObservableCollection<BelgeTipi> BelgeTipleri { get; } = new();

        private string _aramaMetni = string.Empty;
        private BelgeTipi _seciliBelgeTipi = BelgeTipi.Bilinmeyen;
        private Belge? _seciliBelge;
        private bool _sadeceFavoriler = false;

        public BelgeListeVM(BelgeYonetimi belgeYonetimi, KisiRepository kisiRepository)
        {
            _belgeYonetimi = belgeYonetimi;
            _kisiRepository = kisiRepository;
            Title = "Belgeler";

            // Commands
            AraCommand = new Command(async () => await AraAsync());
            YenileCommand = new Command(async () => await YenileAsync());
            BelgeEkleCommand = new Command(async () => await BelgeEkleAsync());
            BelgeSecCommand = new Command<Belge>(async (belge) => await BelgeSecAsync(belge));
            BelgeSilCommand = new Command<Belge>(async (belge) => await BelgeSilAsync(belge));
            BelgeIndirCommand = new Command<Belge>(async (belge) => await BelgeIndirAsync(belge));
            BelgePaylasCommand = new Command<Belge>(async (belge) => await BelgePaylasAsync(belge));
            BelgeAcCommand = new Command<Belge>(async (belge) => await BelgeAcAsync(belge));
            FiltreTemizleCommand = new Command(async () => await FiltreTemizleAsync());
            
            // PC Özel Komutlar
            BelgeDuzenleCommand = new Command<Belge>(async (belge) => await BelgeDuzenleAsync(belge));
            YoluKopyalaCommand = new Command<Belge>(async (belge) => await YoluKopyalaAsync(belge));
            KlasordGosterCommand = new Command<Belge>(belge => KlasordGoster(belge));
            FavoriToggleCommand = new Command<Belge>(async (belge) => await FavoriToggleAsync(belge));

            // Belge tiplerini yükle
            BelgeTipleriniYukle();
            
            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public string AramaMetni
        {
            get => _aramaMetni;
            set
            {
                SetProperty(ref _aramaMetni, value);
                // 500ms gecikme ile otomatik arama
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    if (_aramaMetni == value) // Hala aynı metin ise ara
                    {
                        await AraAsync();
                    }
                });
            }
        }

        public BelgeTipi SeciliBelgeTipi
        {
            get => _seciliBelgeTipi;
            set
            {
                SetProperty(ref _seciliBelgeTipi, value);
                _ = Task.Run(FiltreUygulaAsync);
            }
        }

        public Belge? SeciliBelge
        {
            get => _seciliBelge;
            set => SetProperty(ref _seciliBelge, value);
        }

        public bool SadeceFavoriler
        {
            get => _sadeceFavoriler;
            set
            {
                SetProperty(ref _sadeceFavoriler, value);
                _ = Task.Run(FiltreUygulaAsync);
            }
        }

        public int ToplamBelgeSayisi => Belgeler.Count;
        public string ToplamBelgeSayisiMetni => $"{ToplamBelgeSayisi} belge";

        #endregion

        #region Commands

        public ICommand AraCommand { get; }
        public ICommand YenileCommand { get; }
        public ICommand BelgeEkleCommand { get; }
        public ICommand BelgeSecCommand { get; }
        public ICommand BelgeSilCommand { get; }
        public ICommand BelgeIndirCommand { get; }
        public ICommand BelgePaylasCommand { get; }
        public ICommand BelgeAcCommand { get; }
        public ICommand FiltreTemizleCommand { get; }
        
        // PC Özel Komutlar
        public ICommand BelgeDuzenleCommand { get; }
        public ICommand YoluKopyalaCommand { get; }
        public ICommand KlasordGosterCommand { get; }
        public ICommand FavoriToggleCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                await BelgeleriYukleAsync();
            });
        }

        private async Task BelgeleriYukleAsync()
        {
            var belgeler = await _belgeYonetimi.TumBelgeleriGetirAsync();
            
            Belgeler.Clear();
            foreach (var belge in belgeler.OrderByDescending(b => b.YuklemeTarihi))
            {
                // Belgeye ait etiketleri set et
                belge.EtiketlerText = GetBelgeEtiketleri(belge.BelgeID);
                
                // Belgeye ait kişi bilgilerini set et
                belge.KisiText = await GetBelgeKisisiAsync(belge.BelgeID);
                
                Belgeler.Add(belge);
            }

            OnPropertyChanged(nameof(ToplamBelgeSayisi), nameof(ToplamBelgeSayisiMetni));
        }

        private async Task AraAsync()
        {
            if (string.IsNullOrWhiteSpace(AramaMetni))
            {
                await BelgeleriYukleAsync();
                return;
            }

            await ExecuteAsync(async () =>
            {
                // Önce normal arama yap
                var normalSonuclar = await _belgeYonetimi.BelgeAraAsync(AramaMetni);
                var sonuclar = normalSonuclar.ToList();

                // Etiket araması yap
                var etiketSonuclari = EtiketAramasiYap(AramaMetni);
                
                // Sonuçları birleştir (dublicate'leri engellemek için)
                foreach (var etiketBelgesi in etiketSonuclari)
                {
                    if (!sonuclar.Any(s => s.BelgeID == etiketBelgesi.BelgeID))
                    {
                        sonuclar.Add(etiketBelgesi);
                    }
                }

                // Kişi araması yap
                var kisiSonuclari = await KisiAramasiYapAsync(AramaMetni);
                
                // Kişi sonuçlarını da birleştir
                foreach (var kisiBelgesi in kisiSonuclari)
                {
                    if (!sonuclar.Any(s => s.BelgeID == kisiBelgesi.BelgeID))
                    {
                        sonuclar.Add(kisiBelgesi);
                    }
                }
                
                Belgeler.Clear();
                foreach (var belge in sonuclar.OrderByDescending(b => b.YuklemeTarihi))
                {
                    // Belgeye ait etiketleri set et
                    belge.EtiketlerText = GetBelgeEtiketleri(belge.BelgeID);
                    
                    // Belgeye ait kişi bilgilerini set et
                    belge.KisiText = await GetBelgeKisisiAsync(belge.BelgeID);
                    
                    Belgeler.Add(belge);
                }

                OnPropertyChanged(nameof(ToplamBelgeSayisi), nameof(ToplamBelgeSayisiMetni));
            });
        }

        private async Task YenileAsync()
        {
            await ExecuteAsync(async () =>
            {
                await BelgeleriYukleAsync();
            });
        }

        private async Task FiltreUygulaAsync()
        {
            await ExecuteAsync(async () =>
            {
                IEnumerable<Belge> belgeler;

                if (SeciliBelgeTipi != BelgeTipi.Bilinmeyen)
                {
                    belgeler = await _belgeYonetimi.TipineGoreBelgeleriGetirAsync(SeciliBelgeTipi);
                }
                else
                {
                    belgeler = await _belgeYonetimi.TumBelgeleriGetirAsync();
                }

                if (SadeceFavoriler)
                {
                    belgeler = belgeler.Where(b => b.Favori != null);
                }

                Belgeler.Clear();
                foreach (var belge in belgeler.OrderByDescending(b => b.YuklemeTarihi))
                {
                    // Belgeye ait etiketleri set et
                    belge.EtiketlerText = GetBelgeEtiketleri(belge.BelgeID);
                    
                    // Belgeye ait kişi bilgilerini set et
                    belge.KisiText = await GetBelgeKisisiAsync(belge.BelgeID);
                    
                    Belgeler.Add(belge);
                }

                OnPropertyChanged(nameof(ToplamBelgeSayisi), nameof(ToplamBelgeSayisiMetni));
            });
        }

        private async Task FiltreTemizleAsync()
        {
            AramaMetni = string.Empty;
            SeciliBelgeTipi = BelgeTipi.Bilinmeyen;
            SadeceFavoriler = false;
            await BelgeleriYukleAsync();
        }

        private async Task BelgeEkleAsync()
        {
            // Belge ekleme sayfasına navigasyon
            await Shell.Current.GoToAsync("BelgeEkle");
        }

        private async Task BelgeSecAsync(Belge belge)
        {
            if (belge == null) return;

            SeciliBelge = belge;
            await Shell.Current.GoToAsync($"BelgeDetay?belgeId={belge.BelgeID}");
        }

        private async Task BelgeSilAsync(Belge belge)
        {
            if (belge == null) return;

            var result = await Application.Current.MainPage.DisplayAlert(
                "Belge Sil", 
                $"'{belge.BelgeAdi}' adlı belgeyi silmek istediğinize emin misiniz?", 
                "Evet", "Hayır");

            if (result)
            {
                await ExecuteAsync(async () =>
                {
                    var success = await _belgeYonetimi.BelgeSilAsync(belge.BelgeID);
                    if (success)
                    {
                        Belgeler.Remove(belge);
                        OnPropertyChanged(nameof(ToplamBelgeSayisi), nameof(ToplamBelgeSayisiMetni));
                        await Application.Current.MainPage.DisplayAlert("Başarılı", "Belge başarıyla silindi.", "Tamam");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Belge silinirken bir hata oluştu.", "Tamam");
                    }
                });
            }
        }

        private async Task BelgeIndirAsync(Belge belge)
        {
            if (belge == null) return;

            await ExecuteAsync(async () =>
            {
                try
                {
                    // Belge dosyasını al
                    var dosyaBytes = await _belgeYonetimi.BelgeDosyasiniIndirAsync(belge.BelgeID);
                    if (dosyaBytes == null || dosyaBytes.Length == 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Belge dosyası bulunamadı.", "Tamam");
                        return;
                    }

                    // Dosya adını oluştur
                    var fileName = $"{belge.BelgeAdi}{belge.DosyaTipi}";
                    
                    // Platform-specific indirme işlemi
                    await SaveFileAsync(fileName, dosyaBytes);
                    
                    await Application.Current.MainPage.DisplayAlert("Başarılı", 
                        $"'{fileName}' dosyası başarıyla indirildi.", "Tamam");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        $"Dosya indirilirken bir hata oluştu: {ex.Message}", "Tamam");
                }
            });
        }

        private async Task BelgePaylasAsync(Belge belge)
        {
            if (belge == null) return;

            await ExecuteAsync(async () =>
            {
                try
                {
                    // Geçici dosya yolu al
                    var tempFilePath = await _belgeYonetimi.BelgePaylasAsync(belge.BelgeID);
                    if (string.IsNullOrEmpty(tempFilePath))
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Paylaşım için dosya hazırlanamadı.", "Tamam");
                        return;
                    }

                    // Paylaşım işlemini başlat
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = $"{belge.BelgeAdi} - Belge Paylaşımı",
                        File = new ShareFile(tempFilePath)
                    });
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        $"Dosya paylaşılırken bir hata oluştu: {ex.Message}", "Tamam");
                }
            });
        }

        private async Task SaveFileAsync(string fileName, byte[] fileBytes)
        {
            try
            {
#if ANDROID
                // Android için Downloads klasörüne kaydet
                var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                var filePath = Path.Combine(downloadsPath!.AbsolutePath, fileName);
                
                await File.WriteAllBytesAsync(filePath, fileBytes);
#elif WINDOWS
                // Windows için Documents/Downloads klasörüne kaydet
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                if (!Directory.Exists(downloadsPath))
                    Directory.CreateDirectory(downloadsPath);
                    
                var filePath = Path.Combine(downloadsPath, fileName);
                await File.WriteAllBytesAsync(filePath, fileBytes);
#else
                // Diğer platformlar için geçici klasöre kaydet
                var tempPath = Path.GetTempPath();
                var filePath = Path.Combine(tempPath, fileName);
                await File.WriteAllBytesAsync(filePath, fileBytes);
#endif
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        private void BelgeTipleriniYukle()
        {
            BelgeTipleri.Clear();
            foreach (BelgeTipi tip in Enum.GetValues<BelgeTipi>())
            {
                BelgeTipleri.Add(tip);
            }
        }

        private async Task BelgeAcAsync(Belge belge)
        {
            if (belge == null) return;

            await ExecuteAsync(async () =>
            {
                try
                {
                    // Belge dosyasını al
                    var dosyaBytes = await _belgeYonetimi.BelgeDosyasiniIndirAsync(belge.BelgeID);
                    if (dosyaBytes == null || dosyaBytes.Length == 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Belge dosyası bulunamadı.", "Tamam");
                        return;
                    }

                    // Geçici dosya oluştur
                    var fileName = $"{belge.BelgeAdi}{belge.DosyaTipi}";
                    var tempPath = Path.GetTempPath();
                    var filePath = Path.Combine(tempPath, fileName);
                    
                    await File.WriteAllBytesAsync(filePath, dosyaBytes);

                    // Dosyayı varsayılan uygulama ile aç
                    await Launcher.Default.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath),
                        Title = $"{belge.BelgeAdi} - Belge Görüntüle"
                    });
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        $"Belge açılırken bir hata oluştu: {ex.Message}", "Tamam");
                }
            });
        }

        public string BelgeBoyutuFormatla(long boyut)
        {
            return DosyaYardimcisi.BoyutFormatla(boyut);
        }

        /// <summary>
        /// Belgeye ait etiketleri getirir
        /// </summary>
        public string GetBelgeEtiketleri(int belgeId)
        {
            try
            {
                var baglantiler = GetEtiketBelgeBaglantilari();
                var belgeEtiketleri = baglantiler
                    .Where(kvp => kvp.Value.Contains(belgeId))
                    .Select(kvp => kvp.Key)
                    .ToList();

                return belgeEtiketleri.Any() ? string.Join(", ", belgeEtiketleri) : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Etiketlere göre belge arama yapar
        /// </summary>
        private List<Belge> EtiketAramasiYap(string aramaMetni)
        {
            try
            {
                var sonuclar = new List<Belge>();
                
                // Global etiket-belge bağlantılarını al
                var etiketBelgeBaglantilari = GetEtiketBelgeBaglantilari();
                
                // Arama metnini küçük harfe çevir
                var aramaTerimi = aramaMetni.ToLowerInvariant();
                
                // Eşleşen etiketleri bul
                var eslesenBelgeIdler = etiketBelgeBaglantilari
                    .Where(kvp => kvp.Key.ToLowerInvariant().Contains(aramaTerimi))
                    .SelectMany(kvp => kvp.Value)
                    .Distinct()
                    .ToList();

                // Tüm belgeleri al ve eşleşenleri filtrele
                var tumBelgeler = _belgeYonetimi.TumBelgeleriGetirAsync().GetAwaiter().GetResult();
                
                foreach (var belgeId in eslesenBelgeIdler)
                {
                    var belge = tumBelgeler.FirstOrDefault(b => b.BelgeID == belgeId);
                    if (belge != null)
                    {
                        sonuclar.Add(belge);
                    }
                }

                return sonuclar;
            }
            catch
            {
                return new List<Belge>();
            }
        }

        /// <summary>
        /// Preferences'tan etiket-belge bağlantılarını getirir
        /// </summary>
        private Dictionary<string, List<int>> GetEtiketBelgeBaglantilari()
        {
            try
            {
                var baglantiler = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
                
                var baglantiStr = Preferences.Get("EtiketBelgeBaglantilari", string.Empty);
                
                if (!string.IsNullOrEmpty(baglantiStr))
                {
                    var baglantiListesi = baglantiStr.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach (var baglantiItem in baglantiListesi)
                    {
                        var parcalar = baglantiItem.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        if (parcalar.Length >= 2)
                        {
                            var etiketAdi = parcalar[0];
                            var belgeIdListesi = parcalar[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Where(id => int.TryParse(id, out _))
                                .Select(int.Parse)
                                .ToList();

                            if (belgeIdListesi.Any())
                            {
                                baglantiler[etiketAdi] = belgeIdListesi;
                            }
                        }
                    }
                }

                return baglantiler;
            }
            catch
            {
                return new Dictionary<string, List<int>>();
            }
        }

        /// <summary>
        /// Kişilere göre belge arama yapar
        /// </summary>
        private async Task<List<Belge>> KisiAramasiYapAsync(string aramaMetni)
        {
            try
            {
                var sonuclar = new List<Belge>();
                
                // Önce kişileri ara
                var kisiler = await _kisiRepository.SearchAsync(aramaMetni);
                var bulunanKisiIdler = kisiler.Select(k => k.KisiID).ToList();
                
                if (!bulunanKisiIdler.Any())
                    return sonuclar;
                
                // Kişi-belge bağlantılarını al
                var baglantilar = Preferences.Get("KisiBelgeBaglantilari", string.Empty);
                if (string.IsNullOrEmpty(baglantilar))
                    return sonuclar;

                var baglantiListesi = baglantilar.Split('|', StringSplitOptions.RemoveEmptyEntries);
                var eslenenBelgeIdler = new List<int>();

                // Bulunan kişilere ait belgeleri bul
                foreach (var baglanti in baglantiListesi)
                {
                    var parts = baglanti.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && int.TryParse(parts[0], out int kisiId) && int.TryParse(parts[1], out int belgeId))
                    {
                        if (bulunanKisiIdler.Contains(kisiId))
                        {
                            eslenenBelgeIdler.Add(belgeId);
                        }
                    }
                }

                // Tüm belgeleri al ve eşleşenleri filtrele
                var tumBelgeler = await _belgeYonetimi.TumBelgeleriGetirAsync();
                
                foreach (var belgeId in eslenenBelgeIdler.Distinct())
                {
                    var belge = tumBelgeler.FirstOrDefault(b => b.BelgeID == belgeId);
                    if (belge != null && belge.Aktif)
                    {
                        sonuclar.Add(belge);
                    }
                }

                return sonuclar;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Kişi araması hatası: {ex.Message}");
                return new List<Belge>();
            }
        }

        /// <summary>
        /// Belgeye ait kişi bilgilerini getirir
        /// </summary>
        private async Task<string> GetBelgeKisisiAsync(int belgeId)
        {
            try
            {
                // Kişi-belge bağlantılarını al
                var baglantilar = Preferences.Get("KisiBelgeBaglantilari", string.Empty);
                if (string.IsNullOrEmpty(baglantilar))
                    return string.Empty;

                var baglantiListesi = baglantilar.Split('|', StringSplitOptions.RemoveEmptyEntries);
                var kisiIdler = new List<int>();

                // Bu belgeye bağlı kişileri bul
                foreach (var baglanti in baglantiListesi)
                {
                    var parts = baglanti.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && int.TryParse(parts[0], out int kisiId) && int.TryParse(parts[1], out int belgId))
                    {
                        if (belgId == belgeId)
                        {
                            kisiIdler.Add(kisiId);
                        }
                    }
                }

                if (!kisiIdler.Any())
                    return string.Empty;

                // Kişi bilgilerini getir
                var kisiAdlari = new List<string>();
                foreach (var kisiId in kisiIdler.Distinct())
                {
                    var kisi = await _kisiRepository.GetirAsync(kisiId);
                    if (kisi != null && kisi.Aktif)
                    {
                        kisiAdlari.Add(kisi.TamAd);
                    }
                }

                return kisiAdlari.Any() ? string.Join(", ", kisiAdlari) : string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Kişi bilgisi getirme hatası: {ex.Message}");
                return string.Empty;
            }
        }

        #region PC Özel Metodlar

        private async Task BelgeDuzenleAsync(Belge belge)
        {
            if (belge == null) return;

            try
            {
                await Shell.Current.GoToAsync($"BelgeEkle?belgeId={belge.BelgeID}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", 
                    $"Belge düzenleme sayfası açılırken hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async Task YoluKopyalaAsync(Belge belge)
        {
            if (belge == null) return;

            try
            {
                // Dosya geçici bir konuma kaydet ve yolunu kopyala
                var tempFileName = $"{belge.BelgeAdi}_{DateTime.Now:yyyyMMdd_HHmmss}{belge.DosyaTipi}";
                var tempPath = Path.Combine(Path.GetTempPath(), tempFileName);

                await File.WriteAllBytesAsync(tempPath, belge.Dosya);

#if WINDOWS
                // Windows'ta clipboard'a kopyala
                await Clipboard.Default.SetTextAsync(tempPath);
#endif

                await Application.Current.MainPage.DisplayAlert("Başarılı", 
                    "Dosya yolu panoya kopyalandı.", "Tamam");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", 
                    $"Dosya yolu kopyalanırken hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private void KlasordGoster(Belge belge)
        {
            if (belge == null) return;

#if WINDOWS
            try
            {
                // Dosyayı geçici bir konuma kaydet
                var tempFileName = $"{belge.BelgeAdi}_{DateTime.Now:yyyyMMdd_HHmmss}{belge.DosyaTipi}";
                var tempPath = Path.Combine(Path.GetTempPath(), tempFileName);

                File.WriteAllBytes(tempPath, belge.Dosya);

                // Windows Explorer'da göster
                Arsivim.Platforms.Windows.WindowsSpecificService.OpenFileInExplorer(tempPath);
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        $"Dosya klasörde gösterilirken hata oluştu: {ex.Message}", "Tamam");
                });
            }
#else
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Bilgi", 
                    "Bu özellik sadece Windows'ta kullanılabilir.", "Tamam");
            });
#endif
        }

        private async Task FavoriToggleAsync(Belge belge)
        {
            if (belge == null) return;

            try
            {
                if (belge.Favori != null)
                {
                    // Favoriden çıkar
                    await _belgeYonetimi.FavoridanCikarAsync(belge.BelgeID);
                    belge.Favori = null;
                    await Application.Current.MainPage.DisplayAlert("Başarılı", 
                        "Belge favorilerden çıkarıldı.", "Tamam");
                }
                else
                {
                    // Favoriye ekle
                    await _belgeYonetimi.FavoriyeEkleAsync(belge.BelgeID);
                    belge.Favori = new Favori 
                    { 
                        BelgeID = belge.BelgeID, 
                        Tarih = DateTime.Now 
                    };
                    await Application.Current.MainPage.DisplayAlert("Başarılı", 
                        "Belge favorilere eklendi.", "Tamam");
                }

                // UI'ı güncelle
                OnPropertyChanged();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", 
                    $"Favori işlemi yapılırken hata oluştu: {ex.Message}", "Tamam");
            }
        }

        #endregion

        #endregion
    }
} 