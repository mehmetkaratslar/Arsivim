using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Core.Enums;
using Arsivim.Services.Core;
using Arsivim.Shared.Helpers;
using Microsoft.Maui.Storage;

namespace Arsivim.ViewModels
{
    [QueryProperty(nameof(BelgeId), "belgeId")]
    public class BelgeEkleVM : BaseViewModel
    {
        private readonly BelgeYonetimi _belgeYonetimi;
        
        private string _belgeId = string.Empty;
        private string _belgeAdi = string.Empty;
        private string _aciklama = string.Empty;
        private BelgeTipi _seciliBelgeTipi = BelgeTipi.Bilinmeyen;
        private FileResult? _seciliDosya;
        private bool _otomatikIsimlendir = true;
        private bool _favoriyeEkle = false;
        private bool _duzenlemeModu = false;
        private Belge? _mevcutBelge;

        public ObservableCollection<BelgeTipi> BelgeTipleri { get; } = new();

        public BelgeEkleVM(BelgeYonetimi belgeYonetimi)
        {
            _belgeYonetimi = belgeYonetimi;
            Title = "Yeni Belge Ekle";

            // Commands
            DosyaSecCommand = new Command(async () => await DosyaSecAsync());
            KameraCekCommand = new Command(async () => await KameraCekAsync());
            DosyaKaldirCommand = new Command(DosyaKaldir);
            KaydetCommand = new Command(async () => await KaydetAsync(), () => KaydetOlabilirMi());
            IptalCommand = new Command(async () => await IptalAsync());

            // Belge tiplerini yükle
            BelgeTipleriniYukle();
        }

        #region Properties

        public string BelgeId
        {
            get => _belgeId;
            set
            {
                SetProperty(ref _belgeId, value);
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id))
                {
                    _duzenlemeModu = true;
                    Title = "Belge Düzenle";
                    _ = Task.Run(() => BelgeYukleAsync(id));
                }
                else
                {
                    _duzenlemeModu = false;
                    Title = "Yeni Belge Ekle";
                }
            }
        }

        public bool DuzenlemeModu
        {
            get => _duzenlemeModu;
            set => SetProperty(ref _duzenlemeModu, value);
        }

        public string BelgeAdi
        {
            get => _belgeAdi;
            set
            {
                SetProperty(ref _belgeAdi, value);
                ((Command)KaydetCommand).ChangeCanExecute();
            }
        }

        public string Aciklama
        {
            get => _aciklama;
            set => SetProperty(ref _aciklama, value);
        }

        public BelgeTipi SeciliBelgeTipi
        {
            get => _seciliBelgeTipi;
            set
            {
                SetProperty(ref _seciliBelgeTipi, value);
                ((Command)KaydetCommand).ChangeCanExecute();
            }
        }

        public FileResult? SeciliDosya
        {
            get => _seciliDosya;
            set
            {
                SetProperty(ref _seciliDosya, value);
                OnPropertyChanged(nameof(SeciliDosyaAdi), nameof(SeciliDosyaBoyutu));
                
                // Otomatik isimlendirme açıksa dosya adını belge adı olarak kullan
                if (value != null && OtomatikIsimlendir)
                {
                    BelgeAdi = Path.GetFileNameWithoutExtension(value.FileName);
                }
                
                ((Command)KaydetCommand).ChangeCanExecute();
            }
        }

        public string SeciliDosyaAdi => SeciliDosya?.FileName ?? string.Empty;
        
        public string SeciliDosyaBoyutu
        {
            get
            {
                if (SeciliDosya == null) return string.Empty;
                
                // Dosya boyutunu hesapla (yaklaşık)
                try
                {
                    using var stream = SeciliDosya.OpenReadAsync().Result;
                    return DosyaYardimcisi.BoyutFormatla(stream.Length);
                }
                catch
                {
                    return "Bilinmiyor";
                }
            }
        }

        public bool OtomatikIsimlendir
        {
            get => _otomatikIsimlendir;
            set
            {
                SetProperty(ref _otomatikIsimlendir, value);
                
                // Eğer özellik açıldı ve dosya seçiliyse, dosya adını belge adı yap
                if (value && SeciliDosya != null)
                {
                    BelgeAdi = Path.GetFileNameWithoutExtension(SeciliDosya.FileName);
                }
            }
        }

        public bool FavoriyeEkle
        {
            get => _favoriyeEkle;
            set => SetProperty(ref _favoriyeEkle, value);
        }

        #endregion

        #region Commands

        public ICommand DosyaSecCommand { get; }
        public ICommand KameraCekCommand { get; }
        public ICommand DosyaKaldirCommand { get; }
        public ICommand KaydetCommand { get; }
        public ICommand IptalCommand { get; }

        #endregion

        #region Methods

        private async Task BelgeYukleAsync(int belgeId)
        {
            await ExecuteAsync(async () =>
            {
                _mevcutBelge = await _belgeYonetimi.BelgeGetirAsync(belgeId);
                if (_mevcutBelge != null)
                {
                    BelgeAdi = _mevcutBelge.BelgeAdi;
                    SeciliBelgeTipi = _mevcutBelge.BelgeTipi;
                    Aciklama = _mevcutBelge.DosyaAciklamasi ?? string.Empty;
                    FavoriyeEkle = _mevcutBelge.Favori != null;
                    OtomatikIsimlendir = false;
                }
            });
        }

        private void AlanlariTemizle()
        {
            BelgeAdi = string.Empty;
            Aciklama = string.Empty;
            SeciliBelgeTipi = BelgeTipi.Bilinmeyen;
            SeciliDosya = null;
            OtomatikIsimlendir = true;
            FavoriyeEkle = false;
            _mevcutBelge = null;
        }

        private async Task DosyaSecAsync()
        {
            try
            {
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.content", "public.item" } },
                        { DevicePlatform.Android, new[] { "*/*" } },
                        { DevicePlatform.WinUI, new[] { "*" } },
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "*" } },
                    });

                var options = new PickOptions()
                {
                    PickerTitle = "Belge dosyasını seçin",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    SeciliDosya = result;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", 
                    $"Dosya seçilirken bir hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async Task KameraCekAsync()
        {
            try
            {
                // Kamera izni kontrolü
                var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                    if (cameraStatus != PermissionStatus.Granted)
                    {
                        await Application.Current.MainPage.DisplayAlert("İzin Gerekli", 
                            "Kamera kullanmak için izin vermeniz gerekiyor.", "Tamam");
                        return;
                    }
                }

                // MediaPicker ile fotoğraf çek
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    // Geçici dosya oluştur
                    var tempFile = Path.Combine(FileSystem.CacheDirectory, $"camera_capture_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
                    
                    using var sourceStream = await photo.OpenReadAsync();
                    using var fileStream = File.Create(tempFile);
                    await sourceStream.CopyToAsync(fileStream);

                    // FileResult olarak sarma
                    SeciliDosya = new FileResult(tempFile, "image/jpeg");

                    await Application.Current.MainPage.DisplayAlert("Başarılı", 
                        "Fotoğraf başarıyla çekildi!", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", 
                    $"Kamera kullanılırken bir hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private void DosyaKaldir()
        {
            SeciliDosya = null;
            if (OtomatikIsimlendir)
            {
                BelgeAdi = string.Empty;
            }
        }

        private bool KaydetOlabilirMi()
        {
            return !string.IsNullOrWhiteSpace(BelgeAdi) && 
                   SeciliBelgeTipi != BelgeTipi.Bilinmeyen && 
                   SeciliDosya != null;
        }

        private async Task KaydetAsync()
        {
            // Düzenleme modunda dosya zorunlu değil
            bool temelValidasyon = !string.IsNullOrWhiteSpace(BelgeAdi) && SeciliBelgeTipi != BelgeTipi.Bilinmeyen;
            bool dosyaValidasyon = DuzenlemeModu || SeciliDosya != null;
            
            if (!temelValidasyon || !dosyaValidasyon)
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", 
                    "Lütfen tüm gerekli alanları doldurun.", "Tamam");
                return;
            }

            await ExecuteAsync(async () =>
            {
                try
                {
                    if (DuzenlemeModu && _mevcutBelge != null)
                    {
                        // Düzenleme modu
                        _mevcutBelge.BelgeAdi = BelgeAdi.Trim();
                        _mevcutBelge.BelgeTipi = SeciliBelgeTipi;
                        _mevcutBelge.DosyaAciklamasi = string.IsNullOrWhiteSpace(Aciklama) ? null : Aciklama.Trim();
                        _mevcutBelge.SonGuncelleme = DateTime.Now;

                        // Yeni dosya seçildiyse dosyayı güncelle
                        if (SeciliDosya != null)
                        {
                            using var stream = await SeciliDosya.OpenReadAsync();
                            using var memoryStream = new MemoryStream();
                            await stream.CopyToAsync(memoryStream);
                            var dosyaIcerigi = memoryStream.ToArray();

                            _mevcutBelge.Dosya = dosyaIcerigi;
                            _mevcutBelge.DosyaTipi = Path.GetExtension(SeciliDosya.FileName);
                            _mevcutBelge.DosyaBoyutu = dosyaIcerigi.Length;
                            _mevcutBelge.Versiyon++;

                            // Hash hesapla
                            using var sha256 = System.Security.Cryptography.SHA256.Create();
                            var hashBytes = sha256.ComputeHash(dosyaIcerigi);
                            _mevcutBelge.DosyaHash = Convert.ToHexString(hashBytes);

                            // OCR işlemi (ayarlarda aktifse)
                            await OCRIslemYapAsync(_mevcutBelge, dosyaIcerigi);
                        }

                        await _belgeYonetimi.BelgeGuncelleAsync(_mevcutBelge);

                        // Favoriye ekleme/çıkarma
                        var favoriMevcut = _mevcutBelge.Favori != null;
                        if (FavoriyeEkle && !favoriMevcut)
                        {
                            await _belgeYonetimi.FavoriyeEkleAsync(_mevcutBelge.BelgeID);
                        }
                        else if (!FavoriyeEkle && favoriMevcut)
                        {
                            await _belgeYonetimi.FavoridanCikarAsync(_mevcutBelge.BelgeID);
                        }

                        await Application.Current.MainPage.DisplayAlert("Başarılı", 
                            "Belge başarıyla güncellendi!", "Tamam");
                    }
                    else
                    {
                        // Yeni belge ekleme
                        using var stream = await SeciliDosya!.OpenReadAsync();
                        using var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);
                        var dosyaIcerigi = memoryStream.ToArray();

                        var yeniBelge = new Belge
                        {
                            BelgeAdi = BelgeAdi.Trim(),
                            BelgeTipi = SeciliBelgeTipi,
                            DosyaAciklamasi = string.IsNullOrWhiteSpace(Aciklama) ? null : Aciklama.Trim(),
                            Dosya = dosyaIcerigi,
                            DosyaTipi = Path.GetExtension(SeciliDosya.FileName),
                            DosyaBoyutu = dosyaIcerigi.Length,
                            YuklemeTarihi = DateTime.Now,
                            SonGuncelleme = DateTime.Now,
                            Aktif = true,
                            Versiyon = 1
                        };

                        // Hash hesapla
                        using var sha256 = System.Security.Cryptography.SHA256.Create();
                        var hashBytes = sha256.ComputeHash(dosyaIcerigi);
                        yeniBelge.DosyaHash = Convert.ToHexString(hashBytes);

                        // OCR işlemi (ayarlarda aktifse)
                        await OCRIslemYapAsync(yeniBelge, dosyaIcerigi);

                        // Belgeyi kaydet
                        var kaydedilenBelge = await _belgeYonetimi.BelgeEkleAsync(yeniBelge);

                        if (kaydedilenBelge != null)
                        {
                            // Favoriye ekleme
                            if (FavoriyeEkle)
                            {
                                await _belgeYonetimi.FavoriyeEkleAsync(kaydedilenBelge.BelgeID);
                            }

                            // Bildirim gönder
                            await BildirimGonderAsync($"'{yeniBelge.BelgeAdi}' başarıyla eklendi!");

                            await Application.Current.MainPage.DisplayAlert("Başarılı", 
                                "Belge başarıyla kaydedildi!", "Tamam");

                            // Alanları temizle
                            AlanlariTemizle();
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Hata", 
                                "Belge kaydedilirken bir hata oluştu.", "Tamam");
                            return;
                        }
                    }

                    // Ana sayfaya dön
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        $"Belge kaydedilirken bir hata oluştu: {ex.Message}", "Tamam");
                }
            });
        }

        private async Task IptalAsync()
        {
            if (SeciliDosya != null || !string.IsNullOrWhiteSpace(BelgeAdi))
            {
                var result = await Application.Current.MainPage.DisplayAlert("Onay", 
                    "Değişiklikler kaydedilmeyecek. Çıkmak istediğinize emin misiniz?", 
                    "Evet", "Hayır");
                
                if (!result) return;
            }

            await Shell.Current.GoToAsync("..");
        }

        private void BelgeTipleriniYukle()
        {
            BelgeTipleri.Clear();
            
            foreach (BelgeTipi tip in Enum.GetValues<BelgeTipi>())
            {
                if (tip != BelgeTipi.Bilinmeyen)
                {
                    BelgeTipleri.Add(tip);
                }
            }
        }

        private async Task OCRIslemYapAsync(Belge belge, byte[] dosyaIcerigi)
        {
            try
            {
                // OCR ayarı kontrol et
                var ocrAktif = Preferences.Get("OcrOtomatikAktif", false);
                if (!ocrAktif)
                    return;

                // Sadece görüntü dosyaları için OCR yap
                var dosyaTipi = belge.DosyaTipi?.ToLower();
                if (dosyaTipi == null || (!dosyaTipi.Contains("jpg") && !dosyaTipi.Contains("jpeg") && 
                    !dosyaTipi.Contains("png") && !dosyaTipi.Contains("bmp") && !dosyaTipi.Contains("tiff")))
                    return;

                // Basit OCR simülasyonu - gerçek OCR kütüphanesi entegrasyonu yapılabilir
                // Tesseract veya Azure Computer Vision gibi
                await Task.Delay(1000); // OCR işlemi simülasyonu

                // Örnek OCR metni
                var ocrMetni = await SimulateOCRAsync(dosyaIcerigi);
                if (!string.IsNullOrWhiteSpace(ocrMetni))
                {
                    belge.OCRMetni = ocrMetni;
                    
                    // OCR tamamlandı bildirimi gönder
                    await BildirimGonderAsync($"'{belge.BelgeAdi}' için OCR işlemi tamamlandı!");
                }
            }
            catch (Exception ex)
            {
                // OCR hatası belge kaydetmeyi engellemez
                System.Diagnostics.Debug.WriteLine($"OCR Error: {ex.Message}");
            }
        }

        private async Task<string> SimulateOCRAsync(byte[] imageBytes)
        {
            // Gerçek OCR implementasyonu için buraya Tesseract, Azure Computer Vision 
            // veya Google Vision API entegrasyonu yapılabilir
            
            await Task.Delay(500); // Simülasyon

            // Dosya boyutuna göre örnek metin döndür
            if (imageBytes.Length > 100000) // 100KB'dan büyük
            {
                return "Bu belge OCR ile işlenmiştir. Görüntüden çıkarılan metin: " +
                       "Örnek belge metni. Tarih, sayılar ve önemli bilgiler burada görünecektir. " +
                       "OCR teknolojisi ile otomatik olarak çıkarılmıştır.";
            }
            else
            {
                return "OCR işlemi tamamlandı. Kısa metin içeriği tespit edildi.";
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