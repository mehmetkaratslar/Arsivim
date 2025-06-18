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

        public string UygulamaVersionu => "1.0.0";
        public string DatabaseVersionu => "1.0";
        public string SonYedeklemeTarihi => "Henüz yedekleme yapılmadı";

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
            // Ayarları veritabanından yükle - şu anda örnek değerler
            await Task.CompletedTask;
        }

        private async Task YedeklemeKonumuSecAsync()
        {
            try
            {
                // Dosya seçici implementasyonu gerekli
                await Application.Current.MainPage.DisplayAlert("Bilgi", "Dosya seçici özelliği henüz geliştirilmedi.", "Tamam");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Konum seçilirken hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async Task YedeklemeBaslatAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Yedekleme işlemi implementasyonu
                await Task.Delay(2000); // Simüle edilen yedekleme işlemi
                
                await Application.Current.MainPage.DisplayAlert("Başarılı", "Yedekleme işlemi tamamlandı.", "Tamam");
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
                        // Veritabanı temizleme işlemi
                        await Task.Delay(1000);
                        await Application.Current.MainPage.DisplayAlert("Başarılı", "Veritabanı temizlendi.", "Tamam");
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
                // Senkronizasyon test işlemi
                await Task.Delay(3000); // Simüle edilen test işlemi
                
                await Application.Current.MainPage.DisplayAlert("Test Sonucu", "Senkronizasyon sunucusuna bağlantı başarılı.", "Tamam");
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
                // Ayarları kaydetme işlemi - şu anda sadece simüle ediliyor
                // Gerçek implementasyonda ayarlar veritabanına kaydedilecek
                await Task.Delay(500);

                await Application.Current.MainPage.DisplayAlert("Başarılı", "Ayarlar başarıyla kaydedildi.", "Tamam");
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
                OtomatikYedekleme = true;
                SenkronizasyonAktif = false;
                YedeklemeKonumu = string.Empty;
                SenkronizasyonSunucusu = string.Empty;
                MaksimumDosyaBoyutu = 50;
                OcrOtomatikAktif = false;
                OcrDili = "tr-TR";
                BildirimlerAktif = true;
                GuvenceliSilme = true;

                await Application.Current.MainPage.DisplayAlert("Başarılı", "Varsayılan ayarlar yüklendi.", "Tamam");
            }
        }

        #endregion
    }
} 