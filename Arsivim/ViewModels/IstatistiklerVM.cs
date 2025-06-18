using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Data.Repositories;

namespace Arsivim.ViewModels
{
    public class IstatistiklerVM : BaseViewModel
    {
        private readonly BelgeRepository? _belgeRepository;
        private readonly KisiRepository? _kisiRepository;
        
        private int _toplamBelgeSayisi = 0;
        private int _toplamKisiSayisi = 0;
        private int _toplamEtiketSayisi = 0;
        private int _buAyEklenen = 0;
        private int _buHaftaEklenen = 0;
        private int _bugünEklenen = 0;
        private long _toplamDosyaBoyutu = 0;
        private string _enPopulerEtiket = string.Empty;
        private string _sonIslem = string.Empty;
        private DateTime _sonGuncelleme = DateTime.Now;
        private int _favorilenmisBelgeSayisi = 0;
        private int _bugunkuIslemSayisi = 0;

        public IstatistiklerVM(BelgeRepository? belgeRepository = null, KisiRepository? kisiRepository = null)
        {
            _belgeRepository = belgeRepository;
            _kisiRepository = kisiRepository;
            
            Title = "İstatistikler";

            // Commands
            YenileCommand = new Command(async () => await YenileAsync());
            DetayliRaporCommand = new Command(async () => await DetayliRaporAsync());
            VeriAktarCommand = new Command(async () => await VeriAktarAsync());

            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public int ToplamBelgeSayisi
        {
            get => _toplamBelgeSayisi;
            set => SetProperty(ref _toplamBelgeSayisi, value);
        }

        public int ToplamKisiSayisi
        {
            get => _toplamKisiSayisi;
            set => SetProperty(ref _toplamKisiSayisi, value);
        }

        public int ToplamEtiketSayisi
        {
            get => _toplamEtiketSayisi;
            set => SetProperty(ref _toplamEtiketSayisi, value);
        }

        public int BuAyEklenen
        {
            get => _buAyEklenen;
            set => SetProperty(ref _buAyEklenen, value);
        }

        public int BuHaftaEklenen
        {
            get => _buHaftaEklenen;
            set => SetProperty(ref _buHaftaEklenen, value);
        }

        public int BugünEklenen
        {
            get => _bugünEklenen;
            set => SetProperty(ref _bugünEklenen, value);
        }

        public long ToplamDosyaBoyutu
        {
            get => _toplamDosyaBoyutu;
            set => SetProperty(ref _toplamDosyaBoyutu, value);
        }

        public string ToplamDosyaBoyutuMetni => FormatFileSize(ToplamDosyaBoyutu);

        public string EnPopulerEtiket
        {
            get => _enPopulerEtiket;
            set => SetProperty(ref _enPopulerEtiket, value);
        }

        public string SonIslem
        {
            get => _sonIslem;
            set => SetProperty(ref _sonIslem, value);
        }

        public DateTime SonGuncelleme
        {
            get => _sonGuncelleme;
            set => SetProperty(ref _sonGuncelleme, value);
        }

        public int FavorilenmisBelgeSayisi
        {
            get => _favorilenmisBelgeSayisi;
            set => SetProperty(ref _favorilenmisBelgeSayisi, value);
        }

        public int BugunkuIslemSayisi
        {
            get => _bugunkuIslemSayisi;
            set => SetProperty(ref _bugunkuIslemSayisi, value);
        }

        // En çok kullanılan etiket property'si
        public string EnCokKullanilanEtiket
        {
            get => EnPopulerEtiket;
            set => EnPopulerEtiket = value;
        }

        // Belge türü istatistikleri (örnek veri)
        public List<BelgeTuruIstatistik> BelgeTuruIstatistikleri { get; } = new List<BelgeTuruIstatistik>
        {
            new BelgeTuruIstatistik { Tur = "PDF", Sayi = 456, Yuzde = 36.7 },
            new BelgeTuruIstatistik { Tur = "Word", Sayi = 234, Yuzde = 18.8 },
            new BelgeTuruIstatistik { Tur = "Excel", Sayi = 189, Yuzde = 15.2 },
            new BelgeTuruIstatistik { Tur = "PowerPoint", Sayi = 156, Yuzde = 12.5 },
            new BelgeTuruIstatistik { Tur = "Resim", Sayi = 134, Yuzde = 10.7 },
            new BelgeTuruIstatistik { Tur = "Diğer", Sayi = 78, Yuzde = 6.3 }
        };

        // Aylık aktiviteler (örnek veri)
        public List<AylikAktivite> AylikAktiviteler { get; } = new List<AylikAktivite>
        {
            new AylikAktivite { Ay = "Ocak", Aktivite = 45, EklemeSayisi = 25, GuncellemeSayisi = 15, SilmeSayisi = 5 },
            new AylikAktivite { Ay = "Şubat", Aktivite = 67, EklemeSayisi = 38, GuncellemeSayisi = 22, SilmeSayisi = 7 },
            new AylikAktivite { Ay = "Mart", Aktivite = 89, EklemeSayisi = 52, GuncellemeSayisi = 28, SilmeSayisi = 9 },
            new AylikAktivite { Ay = "Nisan", Aktivite = 112, EklemeSayisi = 68, GuncellemeSayisi = 35, SilmeSayisi = 9 },
            new AylikAktivite { Ay = "Mayıs", Aktivite = 98, EklemeSayisi = 58, GuncellemeSayisi = 32, SilmeSayisi = 8 },
            new AylikAktivite { Ay = "Haziran", Aktivite = 134, EklemeSayisi = 82, GuncellemeSayisi = 41, SilmeSayisi = 11 }
        };

        #endregion

        #region Commands

        public ICommand YenileCommand { get; }
        public ICommand DetayliRaporCommand { get; }
        public ICommand VeriAktarCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Gerçek verilerden istatistikleri hesapla
                await GercekVerileriYukleAsync();
                
                OnPropertyChanged(nameof(ToplamDosyaBoyutuMetni));
                await Task.Delay(100);
            });
        }

        private async Task GercekVerileriYukleAsync()
        {
            try
            {
                // Repository null ise varsayılan değerleri kullan
                if (_belgeRepository == null || _kisiRepository == null)
                {
                    // Örnek veriler
                    ToplamBelgeSayisi = 245;
                    ToplamKisiSayisi = 89;
                    ToplamEtiketSayisi = 34;
                    BuAyEklenen = 23;
                    BuHaftaEklenen = 8;
                    BugünEklenen = 2;
                    ToplamDosyaBoyutu = 1024 * 1024 * 500; // 500 MB
                    FavorilenmisBelgeSayisi = 45;
                    EnPopulerEtiket = "Önemli";
                    SonIslem = "Belge eklendi: Rapor.pdf";
                    BugunkuIslemSayisi = 12;
                    SonGuncelleme = DateTime.Now;
                    return;
                }

                // Belgeler tablosundan gerçek veriler
                var belgeler = await GetBelgelerAsync();
                ToplamBelgeSayisi = belgeler.Count();
                
                // Bu ay eklenen belgeler
                var buAyBaslangic = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                BuAyEklenen = belgeler.Count(b => b.EklemeTarihi >= buAyBaslangic);
                
                // Bu hafta eklenen belgeler
                var buHaftaBaslangic = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                BuHaftaEklenen = belgeler.Count(b => b.EklemeTarihi >= buHaftaBaslangic);
                
                // Bugün eklenen belgeler
                BugünEklenen = belgeler.Count(b => b.EklemeTarihi.Date == DateTime.Today);
                
                // Toplam dosya boyutu hesapla
                ToplamDosyaBoyutu = belgeler.Sum(b => b.DosyaBoyutu ?? 0);
                
                // Favori belgeler
                FavorilenmisBelgeSayisi = belgeler.Count(b => b.Favori);

                // Kişiler tablosundan gerçek veriler
                var kisiler = await GetKisilerAsync();
                ToplamKisiSayisi = kisiler.Count();

                // Etiketler tablosundan gerçek veriler
                var etiketler = await GetEtiketlerAsync();
                ToplamEtiketSayisi = etiketler.Count();
                
                // En popüler etiketi bul
                if (etiketler.Any())
                {
                    var etiketKullanimlari = new Dictionary<string, int>();
                    foreach (var belge in belgeler)
                    {
                        if (!string.IsNullOrEmpty(belge.Etiketler))
                        {
                            var belgeEtiketleri = belge.Etiketler.Split(',');
                            foreach (var etiket in belgeEtiketleri)
                            {
                                var temizEtiket = etiket.Trim();
                                if (!string.IsNullOrEmpty(temizEtiket))
                                {
                                    etiketKullanimlari[temizEtiket] = etiketKullanimlari.ContainsKey(temizEtiket) ? etiketKullanimlari[temizEtiket] + 1 : 1;
                                }
                            }
                        }
                    }
                    
                    EnPopulerEtiket = etiketKullanimlari.Any() 
                        ? etiketKullanimlari.OrderByDescending(x => x.Value).First().Key
                        : "Henüz etiket yok";
                }
                else
                {
                    EnPopulerEtiket = "Henüz etiket yok";
                }

                // Geçmiş tablosundan son işlem
                var gecmisler = await GetGecmislerAsync();
                if (gecmisler.Any())
                {
                    var sonIslemObj = gecmisler.OrderByDescending(g => g.Zaman).First();
                    var gecenSure = DateTime.Now - sonIslemObj.Zaman;
                    
                    if (gecenSure.TotalMinutes < 60)
                        SonIslem = $"{sonIslemObj.IslemTuru} - {(int)gecenSure.TotalMinutes} dakika önce";
                    else if (gecenSure.TotalHours < 24)
                        SonIslem = $"{sonIslemObj.IslemTuru} - {(int)gecenSure.TotalHours} saat önce";
                    else
                        SonIslem = $"{sonIslemObj.IslemTuru} - {(int)gecenSure.TotalDays} gün önce";
                }
                else
                {
                    SonIslem = "Henüz işlem yok";
                }
                
                // Bugünkü işlem sayısı
                BugunkuIslemSayisi = gecmisler.Count(g => g.Zaman.Date == DateTime.Today);
                
                SonGuncelleme = DateTime.Now;

                // Belge türü istatistiklerini güncelle
                await BelgeTuruIstatistikleriniGuncelleAsync(belgeler);
                
                // Aylık aktiviteleri güncelle
                await AylikAktiviteleriGuncelleAsync(gecmisler);
            }
            catch (Exception ex)
            {
                // Hata durumunda varsayılan değerler
                ToplamBelgeSayisi = 0;
                ToplamKisiSayisi = 0;
                ToplamEtiketSayisi = 0;
                BuAyEklenen = 0;
                BuHaftaEklenen = 0;
                BugünEklenen = 0;
                ToplamDosyaBoyutu = 0;
                EnPopulerEtiket = "Veri yüklenemedi";
                SonIslem = "Veri yüklenemedi";
                FavorilenmisBelgeSayisi = 0;
                BugunkuIslemSayisi = 0;
                
                await Application.Current.MainPage.DisplayAlert("Hata", $"İstatistikler yüklenirken hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async Task<IEnumerable<dynamic>> GetBelgelerAsync()
        {
            // Repository null ise örnek veriler döndür
            if (_belgeRepository == null)
            {
                return new[]
                {
                    new { 
                        BelgeID = 1, 
                        BelgeAdi = "Örnek Belge", 
                        EklemeTarihi = DateTime.Now.AddDays(-1), 
                        DosyaBoyutu = 1024000L, 
                        Favori = true, 
                        Etiketler = "Önemli",
                        DosyaTuru = "PDF"
                    }
                };
            }

            try
            {
                // Gerçek veritabanından belgeleri al
                var belgeler = await _belgeRepository.GetAllAsync();
                
                // Etiket-belge bağlantılarını al
                var etiketBelgeBaglantilari = GetEtiketBelgeBaglantilari();
                
                return belgeler.Select(b => new
                {
                    BelgeID = b.BelgeID,
                    BelgeAdi = b.BelgeAdi,
                    EklemeTarihi = b.YuklemeTarihi,
                    DosyaBoyutu = b.DosyaBoyutu,
                    Favori = b.Favori != null,
                    Etiketler = GetBelgeEtiketleriText(b.BelgeID, etiketBelgeBaglantilari),
                    DosyaTuru = GetDosyaTuruFromExtension(b.DosyaTipi ?? string.Empty)
                });
            }
            catch
            {
                // Hata durumunda örnek veriler döndür
                return new[]
                {
                    new { 
                        BelgeID = 1, 
                        BelgeAdi = "Örnek Belge", 
                        EklemeTarihi = DateTime.Now.AddDays(-1), 
                        DosyaBoyutu = 1024000L, 
                        Favori = true, 
                        Etiketler = "Önemli",
                        DosyaTuru = "PDF"
                    }
                };
            }
        }

        private string GetDosyaTuruFromExtension(string dosyaYolu)
        {
            if (string.IsNullOrEmpty(dosyaYolu)) return "Diğer";
            
            var extension = Path.GetExtension(dosyaYolu).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "PDF",
                ".doc" or ".docx" => "Word",
                ".xls" or ".xlsx" => "Excel",
                ".ppt" or ".pptx" => "PowerPoint",
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "Resim",
                _ => "Diğer"
            };
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
                                .Select(id => int.TryParse(id, out var belgeId) ? belgeId : 0)
                                .Where(id => id > 0)
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
        /// Belgeye ait etiketleri virgülle ayrılmış string olarak döndürür
        /// </summary>
        private string GetBelgeEtiketleriText(int belgeId, Dictionary<string, List<int>> etiketBelgeBaglantilari)
        {
            try
            {
                var belgeEtiketleri = etiketBelgeBaglantilari
                    .Where(kvp => kvp.Value.Contains(belgeId))
                    .Select(kvp => kvp.Key)
                    .ToList();

                return belgeEtiketleri.Any() ? string.Join(",", belgeEtiketleri) : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<IEnumerable<dynamic>> GetKisilerAsync()
        {
            // Repository null ise örnek veriler döndür
            if (_kisiRepository == null)
            {
                return new[]
                {
                    new { KisiID = 1, Ad = "Örnek Kişi", Email = "ornek@example.com" }
                };
            }

            try
            {
                // Gerçek veritabanından kişileri al
                var kisiler = await _kisiRepository.GetAllAsync();
                
                return kisiler.Select(k => new
                {
                    KisiID = k.KisiID,
                    Ad = $"{k.Ad} {k.Soyad}",
                    Email = k.Email
                });
            }
            catch
            {
                // Hata durumunda örnek veriler döndür
                return new[]
                {
                    new { KisiID = 1, Ad = "Örnek Kişi", Email = "ornek@example.com" }
                };
            }
        }

        private async Task<IEnumerable<dynamic>> GetEtiketlerAsync()
        {
            try
            {
                // Preferences'tan global etiketleri al
                var globalEtiketler = Preferences.Get("GlobalEtiketler", string.Empty);
                var etiketListesi = new List<dynamic>();
                
                if (!string.IsNullOrEmpty(globalEtiketler))
                {
                    var etiketler = globalEtiketler.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    var id = 1;
                    
                    foreach (var etiketItem in etiketler)
                    {
                        var parcalar = etiketItem.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        if (parcalar.Length >= 2)
                        {
                            var etiketAdi = parcalar[0];
                            var renk = parcalar[1];
                            
                            etiketListesi.Add(new { 
                                EtiketID = id++, 
                                EtiketAdi = etiketAdi, 
                                Renk = renk 
                            });
                        }
                    }
                }
                
                await Task.Delay(50);
                return etiketListesi;
            }
            catch
            {
                // Hata durumunda boş liste döndür
                return new List<dynamic>();
            }
        }

        private async Task<IEnumerable<dynamic>> GetGecmislerAsync()
        {
            try
            {
                // Preferences'tan geçmiş işlemleri al
                var gecmisStr = Preferences.Get("IslemGecmisi", string.Empty);
                var gecmisListesi = new List<dynamic>();
                
                if (!string.IsNullOrEmpty(gecmisStr))
                {
                    var gecmisler = gecmisStr.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    var id = 1;
                    
                    foreach (var gecmisItem in gecmisler.Take(50)) // Son 50 işlem
                    {
                        var parcalar = gecmisItem.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        if (parcalar.Length >= 2)
                        {
                            var islemTuru = parcalar[0];
                            if (DateTime.TryParse(parcalar[1], out var zaman))
                            {
                                gecmisListesi.Add(new { 
                                    ID = id++, 
                                    IslemTuru = islemTuru, 
                                    Zaman = zaman 
                                });
                            }
                        }
                    }
                }
                
                // Eğer hiç geçmiş yoksa, en azından sistem başlangıcını ekle
                if (!gecmisListesi.Any())
                {
                    gecmisListesi.Add(new { 
                        ID = 1, 
                        IslemTuru = "Sistem Başlangıcı", 
                        Zaman = DateTime.Now.AddMinutes(-5) 
                    });
                }
                
                await Task.Delay(50);
                return gecmisListesi.OrderByDescending(g => g.Zaman);
            }
            catch
            {
                // Hata durumunda örnek veriler döndür
                return new[]
                {
                    new { ID = 1, IslemTuru = "Sistem Başlangıcı", Zaman = DateTime.Now.AddMinutes(-5) }
                };
            }
        }

        private async Task BelgeTuruIstatistikleriniGuncelleAsync(IEnumerable<dynamic> belgeler)
        {
            await Task.Delay(50);
            
            var turSayilari = belgeler
                .GroupBy(b => b.DosyaTuru)
                .ToDictionary(g => g.Key, g => g.Count());
            
            var toplam = belgeler.Count();
            
            // BelgeTuruIstatistikleri listesini güncelle
            foreach (var istatistik in BelgeTuruIstatistikleri)
            {
                var sayi = turSayilari.ContainsKey(istatistik.Tur) ? turSayilari[istatistik.Tur] : 0;
                istatistik.Sayi = sayi;
                istatistik.Yuzde = toplam > 0 ? (double)sayi / toplam * 100 : 0;
            }
        }

        private async Task AylikAktiviteleriGuncelleAsync(IEnumerable<dynamic> gecmisler)
        {
            await Task.Delay(50);
            
            var aylikGruplar = gecmisler
                .Where(g => g.Zaman >= DateTime.Now.AddMonths(-6))
                .GroupBy(g => new { g.Zaman.Year, g.Zaman.Month })
                .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month:00}", g => g.Count());
            
            // AylikAktiviteler listesini güncelle
            foreach (var aktivite in AylikAktiviteler)
            {
                var ay = DateTime.ParseExact($"01 {aktivite.Ay} 2024", "dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
                var anahtar = $"{ay.Year}-{ay.Month:00}";
                aktivite.Aktivite = aylikGruplar.ContainsKey(anahtar) ? aylikGruplar[anahtar] : 0;
                
                // Rastgele dağıtım (gerçek uygulamada işlem türüne göre grupla)
                var toplam = aktivite.Aktivite;
                aktivite.EklemeSayisi = (int)(toplam * 0.6);
                aktivite.GuncellemeSayisi = (int)(toplam * 0.3);
                aktivite.SilmeSayisi = toplam - aktivite.EklemeSayisi - aktivite.GuncellemeSayisi;
            }
        }

        private async Task YenileAsync()
        {
            await InitializeAsync();
        }

        private async Task DetayliRaporAsync()
        {
            var raporMetni = "DETAYLI İSTATİSTİK RAPORU\n\n" +
                           $"Toplam Belgeler: {ToplamBelgeSayisi}\n" +
                           $"Toplam Kişiler: {ToplamKisiSayisi}\n" +
                           $"Toplam Etiketler: {ToplamEtiketSayisi}\n" +
                           $"Bu Ay Eklenen: {BuAyEklenen}\n" +
                           $"Bu Hafta Eklenen: {BuHaftaEklenen}\n" +
                           $"Bugün Eklenen: {BugünEklenen}\n" +
                           $"Toplam Dosya Boyutu: {ToplamDosyaBoyutuMetni}\n" +
                           $"En Popüler Etiket: {EnPopulerEtiket}\n" +
                           $"Son İşlem: {SonIslem}";

            await Application.Current.MainPage.DisplayAlert("Detaylı Rapor", raporMetni, "Tamam");
        }

        private async Task VeriAktarAsync()
        {
            await ExecuteAsync(async () =>
            {
                try
                {
                    var csvContent = GenerateCSVReport();
                    var fileName = $"Arsivim_Istatistikler_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                    
                    await File.WriteAllTextAsync(filePath, csvContent);
                    
                    var shareRequest = new ShareFileRequest
                    {
                        Title = "İstatistik Raporu",
                        File = new ShareFile(filePath)
                    };
                    
                    await Share.Default.RequestAsync(shareRequest);
                    
                    await Application.Current.MainPage.DisplayAlert(
                        "Başarılı", 
                        "İstatistik raporu başarıyla oluşturuldu ve paylaşıldı!", 
                        "Tamam");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Hata", 
                        $"Rapor oluşturulurken hata oluştu: {ex.Message}", 
                        "Tamam");
                }
            });
        }

        private string GenerateCSVReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Arsivim İstatistik Raporu");
            sb.AppendLine($"Oluşturulma Tarihi,{DateTime.Now:dd.MM.yyyy HH:mm}");
            sb.AppendLine();
            sb.AppendLine("Genel İstatistikler");
            sb.AppendLine("Kategori,Değer");
            sb.AppendLine($"Toplam Belge Sayısı,{ToplamBelgeSayisi}");
            sb.AppendLine($"Toplam Kişi Sayısı,{ToplamKisiSayisi}");
            sb.AppendLine($"Toplam Etiket Sayısı,{ToplamEtiketSayisi}");
            sb.AppendLine($"Favori Belge Sayısı,{FavorilenmisBelgeSayisi}");
            sb.AppendLine($"Bu Ay Eklenen,{BuAyEklenen}");
            sb.AppendLine($"Bu Hafta Eklenen,{BuHaftaEklenen}");
            sb.AppendLine($"Bugün Eklenen,{BugünEklenen}");
            sb.AppendLine($"Toplam Dosya Boyutu,{ToplamDosyaBoyutuMetni}");
            sb.AppendLine($"En Popüler Etiket,{EnPopulerEtiket}");
            sb.AppendLine($"Bugünkü İşlem Sayısı,{BugunkuIslemSayisi}");
            sb.AppendLine();
            sb.AppendLine("Belge Türü Dağılımı");
            sb.AppendLine("Tür,Sayı,Yüzde");
            foreach (var item in BelgeTuruIstatistikleri)
            {
                sb.AppendLine($"{item.Tur},{item.Sayi},{item.Yuzde:F1}%");
            }
            sb.AppendLine();
            sb.AppendLine("Aylık Aktivite");
            sb.AppendLine("Ay,Aktivite Sayısı");
            foreach (var item in AylikAktiviteler)
            {
                sb.AppendLine($"{item.Ay},{item.Aktivite}");
            }
            return sb.ToString();
        }

        private static string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:N1} {suffixes[counter]}";
        }

        #endregion
    }

    // Belge türü istatistik modeli
    public class BelgeTuruIstatistik
    {
        public string Tur { get; set; } = string.Empty;
        public string BelgeTuru => Tur; // XAML için alias
        public int Sayi { get; set; }
        public double Yuzde { get; set; }
    }

    // Aylık aktivite modeli
    public class AylikAktivite
    {
        public string Ay { get; set; } = string.Empty;
        public int Aktivite { get; set; }

        // XAML'de IslemSayisi ile bind edebilmek için alias
        public int IslemSayisi
        {
            get => Aktivite;
            set => Aktivite = value;
        }

        public int EklemeSayisi { get; set; }
        public int GuncellemeSayisi { get; set; }
        public int SilmeSayisi { get; set; }
    }


}