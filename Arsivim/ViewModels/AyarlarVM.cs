using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Shared.Constants;

namespace Arsivim.ViewModels
{
    public class AyarlarVM : BaseViewModel
    {
        private bool _otomatikYedekleme = true;
        private bool _senkronizasyonAktif = false;
        private string _yedeklemeKonumu = string.Empty;
        private string _senkronizasyonSunucusu = string.Empty;
        private int _maksimumDosyaBoyutu = 50; // MB
        private bool _ocrOtomatikAktif = false;
        private string _ocrDili = "tr-TR";
        private bool _bildirimlerAktif = true;
        private bool _guvenceliSilme = true;
        private bool _karanlikTema = false;
        private string _sonYedeklemeTarihi = "Henüz yedekleme yapılmadı";

        public AyarlarVM()
        {
            Title = "Ayarlar";

            // Commands
            YedeklemeKonumuSecCommand = new Command(async () => await YedeklemeKonumuSecAsync());
            YedeklemeBaslatCommand = new Command(async () => await YedeklemeBaslatAsync());
            VeritabaniTemizleCommand = new Command(async () => await VeritabaniTemizleAsync());
            SenkronizasyonTestCommand = new Command(async () => await SenkronizasyonTestAsync());
            HakkindaCommand = new Command(async () => await HakkindaAsync());
            AyarlariKaydetCommand = new Command(async () => await AyarlariKaydetAsync());
            VarsayilanAyarlariYukleCommand = new Command(async () => await VarsayilanAyarlariYukleAsync());

            _ = Task.Run(AyarlariYukleAsync);
        }

        #region Properties

        public bool OtomatikYedekleme
        {
            get => _otomatikYedekleme;
            set => SetProperty(ref _otomatikYedekleme, value);
        }

        public bool SenkronizasyonAktif
        {
            get => _senkronizasyonAktif;
            set => SetProperty(ref _senkronizasyonAktif, value);
        }

        public string YedeklemeKonumu
        {
            get => _yedeklemeKonumu;
            set => SetProperty(ref _yedeklemeKonumu, value);
        }

        public string SenkronizasyonSunucusu
        {
            get => _senkronizasyonSunucusu;
            set => SetProperty(ref _senkronizasyonSunucusu, value);
        }

        public int MaksimumDosyaBoyutu
        {
            get => _maksimumDosyaBoyutu;
            set => SetProperty(ref _maksimumDosyaBoyutu, value);
        }

        public bool OcrOtomatikAktif
        {
            get => _ocrOtomatikAktif;
            set => SetProperty(ref _ocrOtomatikAktif, value);
        }

        public string OcrDili
        {
            get => _ocrDili;
            set => SetProperty(ref _ocrDili, value);
        }

        public bool BildirimlerAktif
        {
            get => _bildirimlerAktif;
            set => SetProperty(ref _bildirimlerAktif, value);
        }

        public bool GuvenceliSilme
        {
            get => _guvenceliSilme;
            set => SetProperty(ref _guvenceliSilme, value);
        }

        public bool KaranlikTema
        {
            get => _karanlikTema;
            set 
            { 
                if (SetProperty(ref _karanlikTema, value))
                {
                    // Tema değişikliğini uygula
                    Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
                }
            }
        }

        public string UygulamaVersionu => "1.0.0";
        public string DatabaseVersionu => "1.0";
        public string SonYedeklemeTarihi 
        {
            get => _sonYedeklemeTarihi;
            set => SetProperty(ref _sonYedeklemeTarihi, value);
        }

        #endregion

        #region Commands

        public ICommand YedeklemeKonumuSecCommand { get; }
        public ICommand YedeklemeBaslatCommand { get; }
        public ICommand VeritabaniTemizleCommand { get; }
        public ICommand SenkronizasyonTestCommand { get; }
        public ICommand HakkindaCommand { get; }
        public ICommand AyarlariKaydetCommand { get; }
        public ICommand VarsayilanAyarlariYukleCommand { get; }

        #endregion

        #region Methods

        private async Task AyarlariYukleAsync()
        {
            try
            {
                // Ayarları Preferences'tan yükle
                BildirimlerAktif = Preferences.Get("BildirimlerAktif", true);
                GuvenceliSilme = Preferences.Get("GuvenceliSilme", true);
                KaranlikTema = Preferences.Get("KaranlikTema", false);
                OtomatikYedekleme = Preferences.Get("OtomatikYedekleme", true);
                SenkronizasyonAktif = Preferences.Get("SenkronizasyonAktif", false);
                YedeklemeKonumu = Preferences.Get("YedeklemeKonumu", string.Empty);
                SenkronizasyonSunucusu = Preferences.Get("SenkronizasyonSunucusu", string.Empty);
                MaksimumDosyaBoyutu = Preferences.Get("MaksimumDosyaBoyutu", 50);
                OcrOtomatikAktif = Preferences.Get("OcrOtomatikAktif", false);
                OcrDili = Preferences.Get("OcrDili", "tr-TR");
                SonYedeklemeTarihi = Preferences.Get("SonYedeklemeTarihi", "Henüz yedekleme yapılmadı");

                // Tema ayarını uygula
                Application.Current.UserAppTheme = KaranlikTema ? AppTheme.Dark : AppTheme.Light;
            }
            catch (Exception ex)
            {
                // Hata durumunda varsayılan değerleri kullan
                await Application.Current.MainPage.DisplayAlert("Uyarı", 
                    $"Ayarlar yüklenirken hata oluştu, varsayılan değerler kullanılıyor: {ex.Message}", "Tamam");
            }

            await Task.CompletedTask;
        }

        private async Task YedeklemeKonumuSecAsync()
        {
            try
            {
                // Manuel yol girişi - MAUI'de klasör seçici sınırlı
                var manuelYol = await Application.Current.MainPage.DisplayPromptAsync(
                    "Yedekleme Konumu", 
                    "Yedekleme klasörü yolunu girin:", 
                    "Tamam", 
                    "İptal", 
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Arsivim", "Backup"),
                    keyboard: Keyboard.Default);

                if (!string.IsNullOrWhiteSpace(manuelYol))
                {
                    try
                    {
                        if (Directory.Exists(manuelYol) || Directory.CreateDirectory(manuelYol).Exists)
                        {
                            YedeklemeKonumu = manuelYol;
                            await Application.Current.MainPage.DisplayAlert("Başarılı", "Yedekleme konumu ayarlandı.", "Tamam");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Hata", "Geçersiz klasör yolu.", "Tamam");
                        }
                    }
                    catch
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Klasör oluşturulamadı.", "Tamam");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Yol seçilirken hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async Task YedeklemeBaslatAsync()
        {
            await ExecuteAsync(async () =>
            {
                try
                {
                    // Yedekleme konumu kontrolü
                    var yedeklemeKlasoru = string.IsNullOrWhiteSpace(YedeklemeKonumu) 
                        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Arsivim", "Backup")
                        : YedeklemeKonumu;

                    // Klasörü oluştur
                    Directory.CreateDirectory(yedeklemeKlasoru);

                    // Yedekleme dosya adı (tarih ile)
                    var tarih = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    var yedeklemeDosyasi = Path.Combine(yedeklemeKlasoru, $"Arsivim_Backup_{tarih}.zip");

                    // Veritabanı dosyasını bul ve kopyala
                    var appDataPath = FileSystem.AppDataDirectory;
                    var dbPath = Path.Combine(appDataPath, "arsivim.db");

                    if (File.Exists(dbPath))
                    {
                        // Basit yedekleme - sadece veritabanı dosyasını kopyala
                        var backupDbPath = Path.Combine(yedeklemeKlasoru, $"arsivim_backup_{tarih}.db");
                        File.Copy(dbPath, backupDbPath, true);

                        // Son yedekleme tarihini güncelle
                        _sonYedeklemeTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                        OnPropertyChanged(nameof(SonYedeklemeTarihi));

                        // Bildirim gönder
                        await BildirimGonderAsync("Yedekleme işlemi başarıyla tamamlandı!");

                        await Application.Current.MainPage.DisplayAlert("Başarılı", 
                            $"Yedekleme tamamlandı!\n\nKonum: {backupDbPath}\nTarih: {_sonYedeklemeTarihi}", "Tamam");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Uyarı", "Yedeklenecek veritabanı dosyası bulunamadı.", "Tamam");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", $"Yedekleme sırasında hata oluştu: {ex.Message}", "Tamam");
                }
            });
        }

        private async Task VeritabaniTemizleAsync()
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Uyarı", 
                "Veritabanı temizleme işlemi geri alınamaz. Tüm veriler silinecek. Devam etmek istediğinize emin misiniz?", 
                "Evet", "Hayır");

            if (result)
            {
                var confirm = await Application.Current.MainPage.DisplayAlert(
                    "Son Uyarı", 
                    "Bu işlem TÜM belgelerinizi ve kişilerinizi silecek. Gerçekten devam etmek istiyor musunuz?", 
                    "Evet, Sil", "Vazgeç");

                if (confirm)
                {
                    await ExecuteAsync(async () =>
                    {
                        try
                        {
                            // Veritabanı dosyasını bul ve sil
                            var appDataPath = FileSystem.AppDataDirectory;
                            var dbPath = Path.Combine(appDataPath, "arsivim.db");

                            if (File.Exists(dbPath))
                            {
                                File.Delete(dbPath);
                                
                                // Ayarları da sıfırla
                                await VarsayilanAyarlariYukleAsync();
                                
                                await Application.Current.MainPage.DisplayAlert("Başarılı", 
                                    "Veritabanı başarıyla temizlendi.\nUygulama yeniden başlatılacak.", "Tamam");
                                
                                // Uygulamayı yeniden başlat
                                System.Diagnostics.Process.Start(Environment.ProcessPath);
                                Application.Current.Quit();
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Bilgi", "Temizlenecek veritabanı bulunamadı.", "Tamam");
                            }
                        }
                        catch (Exception ex)
                        {
                            await Application.Current.MainPage.DisplayAlert("Hata", 
                                $"Veritabanı temizlenirken hata oluştu: {ex.Message}", "Tamam");
                        }
                    });
                }
            }
        }

        private async Task SenkronizasyonTestAsync()
        {
            if (string.IsNullOrWhiteSpace(SenkronizasyonSunucusu))
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Senkronizasyon sunucusu adresi belirtiniz.", "Tamam");
                return;
            }

            await ExecuteAsync(async () =>
            {
                try
                {
                    // HTTP bağlantı testi
                    using var httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    
                    // URL formatını kontrol et
                    if (!Uri.TryCreate(SenkronizasyonSunucusu, UriKind.Absolute, out var uri))
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Geçersiz sunucu adresi formatı.", "Tamam");
                        return;
                    }

                    // Ping testi
                    var response = await httpClient.GetAsync(uri);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Test Sonucu", 
                            $"✅ Bağlantı başarılı!\n\nSunucu: {SenkronizasyonSunucusu}\nDurum: {response.StatusCode}\nSüre: {DateTime.Now:HH:mm:ss}", "Tamam");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Test Sonucu", 
                            $"⚠️ Sunucu yanıt verdi ama hata döndü.\n\nDurum: {response.StatusCode}\nMesaj: {response.ReasonPhrase}", "Tamam");
                    }
                }
                catch (TaskCanceledException)
                {
                    await Application.Current.MainPage.DisplayAlert("Test Sonucu", 
                        "❌ Bağlantı zaman aşımına uğradı.\nSunucu adresini kontrol edin.", "Tamam");
                }
                catch (HttpRequestException ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Test Sonucu", 
                        $"❌ Bağlantı hatası:\n{ex.Message}", "Tamam");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Test Sonucu", 
                        $"❌ Beklenmeyen hata:\n{ex.Message}", "Tamam");
                }
            });
        }

        private async Task HakkindaAsync()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Arsivim Hakkında", 
                $"Arsivim - Belge Arşiv Yönetim Sistemi\n\n" +
                $"Versiyon: {UygulamaVersionu}\n" +
                $"Database Versiyon: {DatabaseVersionu}\n\n" +
                "Geliştirici: Arsivim Takımı\n" +
                "© 2024 Tüm hakları saklıdır.", 
                "Tamam");
        }

        private async Task AyarlariKaydetAsync()
        {
            await ExecuteAsync(async () =>
            {
                try
                {
                    // Ayarları Preferences ile kaydet
                    Preferences.Set("BildirimlerAktif", BildirimlerAktif);
                    Preferences.Set("GuvenceliSilme", GuvenceliSilme);
                    Preferences.Set("KaranlikTema", KaranlikTema);
                    Preferences.Set("OtomatikYedekleme", OtomatikYedekleme);
                    Preferences.Set("SenkronizasyonAktif", SenkronizasyonAktif);
                    Preferences.Set("YedeklemeKonumu", YedeklemeKonumu);
                    Preferences.Set("SenkronizasyonSunucusu", SenkronizasyonSunucusu);
                    Preferences.Set("MaksimumDosyaBoyutu", MaksimumDosyaBoyutu);
                    Preferences.Set("OcrOtomatikAktif", OcrOtomatikAktif);
                    Preferences.Set("OcrDili", OcrDili);
                    Preferences.Set("SonYedeklemeTarihi", SonYedeklemeTarihi);

                    await Application.Current.MainPage.DisplayAlert("Başarılı", 
                        "Ayarlar başarıyla kaydedildi ve uygulandı!", "Tamam");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        $"Ayarlar kaydedilirken hata oluştu: {ex.Message}", "Tamam");
                }
            });
        }

        private async Task VarsayilanAyarlariYukleAsync()
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Varsayılan Ayarları Yükle", 
                "Tüm ayarlar varsayılan değerlere sıfırlanacak. Devam etmek istiyor musunuz?", 
                "Evet", "Hayır");

            if (result)
            {
                // Varsayılan değerleri ata
                OtomatikYedekleme = true;
                SenkronizasyonAktif = false;
                YedeklemeKonumu = string.Empty;
                SenkronizasyonSunucusu = string.Empty;
                MaksimumDosyaBoyutu = 50;
                OcrOtomatikAktif = false;
                OcrDili = "tr-TR";
                BildirimlerAktif = true;
                GuvenceliSilme = true;
                KaranlikTema = false;
                SonYedeklemeTarihi = "Henüz yedekleme yapılmadı";

                // Preferences'ı temizle
                Preferences.Clear();

                // Tema ayarını uygula
                Application.Current.UserAppTheme = AppTheme.Light;

                await Application.Current.MainPage.DisplayAlert("Başarılı", 
                    "Varsayılan ayarlar yüklendi ve uygulandı!", "Tamam");
            }
        }

        private async Task BildirimGonderAsync(string mesaj)
        {
            try
            {
                // Bildirim ayarı kontrol et
                var bildirimAktif = Preferences.Get("BildirimlerAktif", true);
                if (!bildirimAktif)
                    return;

                // Basit bildirim simülasyonu
                await Task.Delay(100);
                System.Diagnostics.Debug.WriteLine($"📱 Bildirim: {mesaj}");
                
                // Gerçek implementasyonda BildirimServisi kullanılabilir
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bildirim gönderilirken hata: {ex.Message}");
            }
        }

        #endregion
    }
} 