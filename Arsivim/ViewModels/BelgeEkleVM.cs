using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Core.Enums;
using Arsivim.Services.Core;
using Arsivim.Shared.Helpers;
using Arsivim.Data.Repositories;
using Microsoft.Maui.Storage;

namespace Arsivim.ViewModels
{
    [QueryProperty(nameof(BelgeId), "belgeId")]
    [QueryProperty(nameof(DosyaYolu), "dosyaYolu")]
    public class BelgeEkleVM : BaseViewModel
    {
        private readonly BelgeYonetimi _belgeYonetimi;
        private readonly KisiRepository _kisiRepository;
        
        private string _belgeId = string.Empty;
        private string _dosyaYolu = string.Empty;
        private string _belgeAdi = string.Empty;
        private string _aciklama = string.Empty;
        private string _etiketler = string.Empty;
        private BelgeTipi _seciliBelgeTipi = BelgeTipi.Bilinmeyen;
        private FileResult? _seciliDosya;
        private bool _otomatikIsimlendir = true;
        private bool _favoriyeEkle = false;
        private bool _duzenlemeModu = false;
        private Belge? _mevcutBelge;
        private Kisi? _seciliKisi;

        public ObservableCollection<BelgeTipi> BelgeTipleri { get; } = new();
        public ObservableCollection<Kisi> Kisiler { get; } = new();

        public BelgeEkleVM(BelgeYonetimi belgeYonetimi, KisiRepository kisiRepository)
        {
            _belgeYonetimi = belgeYonetimi;
            _kisiRepository = kisiRepository;
            Title = "Yeni Belge Ekle";

            // Commands
            DosyaSecCommand = new Command(async () => await DosyaSecAsync());
            KameraCekCommand = new Command(async () => await KameraCekAsync());
            DosyaKaldirCommand = new Command(DosyaKaldir);
            KaydetCommand = new Command(async () => await KaydetAsync(), () => KaydetOlabilirMi());
            IptalCommand = new Command(async () => await IptalAsync());

            // Belge tiplerini yükle
            BelgeTipleriniYukle();
            
            // Kişileri yükle
            _ = Task.Run(KisileriYukleAsync);
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

        public string DosyaYolu
        {
            get => _dosyaYolu;
            set
            {
                SetProperty(ref _dosyaYolu, value);
                if (!string.IsNullOrEmpty(value))
                {
                    _ = Task.Run(() => DragDropDosyaYukleAsync(Uri.UnescapeDataString(value)));
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

        public string Etiketler
        {
            get => _etiketler;
            set => SetProperty(ref _etiketler, value);
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

        public Kisi? SeciliKisi
        {
            get => _seciliKisi;
            set => SetProperty(ref _seciliKisi, value);
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
                    Etiketler = string.Join(", ", _mevcutBelge.BelgeEtiketleri?.Select(be => be.Etiket.EtiketAdi) ?? new string[0]);
                    FavoriyeEkle = _mevcutBelge.Favori != null;
                    OtomatikIsimlendir = false;
                    
                    // Belgeye bağlı kişiyi yükle
                    await BelgeKisisiYukleAsync(_mevcutBelge.BelgeID);
                }
            });
        }

        private void AlanlariTemizle()
        {
            BelgeAdi = string.Empty;
            Aciklama = string.Empty;
            Etiketler = string.Empty;
            SeciliBelgeTipi = BelgeTipi.Bilinmeyen;
            SeciliDosya = null;
            SeciliKisi = null;
            OtomatikIsimlendir = true;
            FavoriyeEkle = false;
            _mevcutBelge = null;
        }

        private async Task DosyaSecAsync()
        {
            try
            {
                // Belge dosya tiplerini tanımla
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.content", "public.item", "public.data" } },
                        { DevicePlatform.Android, new[] { "*/*" } },
                        { DevicePlatform.WinUI, new[] { 
                            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
                            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif",
                            ".txt", ".rtf", ".csv"
                        } },
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "public.content", "public.item" } },
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
                    
                    // Debug bilgisi
                    System.Diagnostics.Debug.WriteLine($"Dosya seçildi: {result.FileName}");
                    System.Diagnostics.Debug.WriteLine($"Dosya yolu: {result.FullPath}");
                    System.Diagnostics.Debug.WriteLine($"Content type: {result.ContentType}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Dosya seçimi iptal edildi");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FilePicker hatası: {ex}");
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

                        // Etiketleri işle (düzenleme modunda da)
                        await EtiketleriIsleAsync(_mevcutBelge.BelgeID);

                        // Kişi-belge bağlantısını güncelle
                        if (SeciliKisi != null)
                        {
                            await KisiBelgeBaglantisiKaydetAsync(SeciliKisi.KisiID, _mevcutBelge.BelgeID);
                        }

                        // İşlem geçmişine kaydet
                        await IslemGecmisineEkleAsync("Belge Güncelleme");

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
                            // Etiketleri işle
                            await EtiketleriIsleAsync(kaydedilenBelge.BelgeID);

                            // Kişi-belge bağlantısını kaydet
                            if (SeciliKisi != null)
                            {
                                await KisiBelgeBaglantisiKaydetAsync(SeciliKisi.KisiID, kaydedilenBelge.BelgeID);
                            }

                            // Favoriye ekleme
                            if (FavoriyeEkle)
                            {
                                await _belgeYonetimi.FavoriyeEkleAsync(kaydedilenBelge.BelgeID);
                            }

                            // İşlem geçmişine kaydet
                            await IslemGecmisineEkleAsync("Belge Ekleme");

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

        private async Task KisileriYukleAsync()
        {
            try
            {
                var kisiler = await _kisiRepository.TumunuGetirAsync();
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Kisiler.Clear();
                    foreach (var kisi in kisiler.Where(k => k.Aktif))
                    {
                        Kisiler.Add(kisi);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Kişi yükleme hatası: {ex.Message}");
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

        private async Task EtiketleriIsleAsync(int belgeId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Etiketler))
                    return;

                // Etiketleri virgülle ayır ve temizle
                var etiketListesi = Etiketler
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(10) // Maksimum 10 etiket
                    .ToList();

                if (!etiketListesi.Any())
                    return;

                // Her etiket için işlem yap
                foreach (var etiketAdi in etiketListesi)
                {
                    // Etiket sistemdeki mevcut etiketler listesine ekle
                    await EtiketEkleVeyaGetirAsync(etiketAdi, belgeId);
                }

                // Başarı bildirimi
                var etiketSayisi = etiketListesi.Count;
                var mesaj = etiketSayisi == 1 
                    ? $"1 etiket eklendi: {etiketListesi[0]}" 
                    : $"{etiketSayisi} etiket eklendi: {string.Join(", ", etiketListesi)}";
                
                await BildirimGonderAsync(mesaj);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Etiket işleme hatası: {ex.Message}");
                // Etiket hatası belge kaydetmeyi engellemez
            }
        }

        private async Task EtiketEkleVeyaGetirAsync(string etiketAdi, int belgeId)
        {
            try
            {
                // Etiket rengi otomatik ata (döngüsel)
                var renkler = new[] { "#F44336", "#E91E63", "#9C27B0", "#673AB7", "#3F51B5", "#2196F3", "#03A9F4", "#00BCD4", "#009688", "#4CAF50", "#8BC34A", "#CDDC39", "#FFEB3B", "#FFC107", "#FF9800", "#FF5722" };
                var rastgeleRenk = renkler[Math.Abs(etiketAdi.GetHashCode()) % renkler.Length];

                // Global etiket listesine ekle (EtiketYonetimVM tarafından kullanılacak)
                await EtiketGlobalListeyeEkleAsync(etiketAdi, rastgeleRenk);

                // Etiket-belge bağlantısını kaydet
                await EtiketBelgeBaglantisiKaydetAsync(etiketAdi, belgeId);

                await Task.Delay(50); // Simüle edilmiş veritabanı işlemi
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Etiket ekleme hatası: {ex.Message}");
            }
        }

        private async Task EtiketGlobalListeyeEkleAsync(string etiketAdi, string renk)
        {
            try
            {
                // Preferences kullanarak etiketleri sakla
                var mevcutEtiketler = Preferences.Get("GlobalEtiketler", string.Empty);
                var etiketListesi = new List<string>();

                if (!string.IsNullOrEmpty(mevcutEtiketler))
                {
                    etiketListesi = mevcutEtiketler.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                // Yeni etiket formatı: "EtiketAdi:Renk:Tarih"
                var yeniEtiket = $"{etiketAdi}:{renk}:{DateTime.Now:yyyy-MM-dd}";
                
                // Aynı isimde etiket varsa güncelle, yoksa ekle
                var mevcutIndex = etiketListesi.FindIndex(e => e.StartsWith($"{etiketAdi}:"));
                if (mevcutIndex >= 0)
                {
                    etiketListesi[mevcutIndex] = yeniEtiket;
                }
                else
                {
                    etiketListesi.Add(yeniEtiket);
                }

                // Maksimum 50 etiket sakla
                if (etiketListesi.Count > 50)
                {
                    etiketListesi = etiketListesi.TakeLast(50).ToList();
                }

                // Güncellenmiş listeyi kaydet
                Preferences.Set("GlobalEtiketler", string.Join("|", etiketListesi));

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Global etiket kaydetme hatası: {ex.Message}");
            }
        }

        private async Task EtiketBelgeBaglantisiKaydetAsync(string etiketAdi, int belgeId)
        {
            try
            {
                // Mevcut etiket-belge bağlantılarını al
                var mevcutBaglantiler = Preferences.Get("EtiketBelgeBaglantilari", string.Empty);
                var baglantiMap = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

                // Mevcut bağlantıları parse et
                if (!string.IsNullOrEmpty(mevcutBaglantiler))
                {
                    var baglantiListesi = mevcutBaglantiler.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach (var baglantiItem in baglantiListesi)
                    {
                        var parcalar = baglantiItem.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        if (parcalar.Length >= 2)
                        {
                            var etiket = parcalar[0];
                            var belgeIdListesi = parcalar[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Where(id => int.TryParse(id, out _))
                                .Select(int.Parse)
                                .ToList();

                            baglantiMap[etiket] = belgeIdListesi;
                        }
                    }
                }

                // Bu etiket için belge ID'sini ekle
                if (baglantiMap.ContainsKey(etiketAdi))
                {
                    if (!baglantiMap[etiketAdi].Contains(belgeId))
                    {
                        baglantiMap[etiketAdi].Add(belgeId);
                    }
                }
                else
                {
                    baglantiMap[etiketAdi] = new List<int> { belgeId };
                }

                // Güncellenmiş bağlantıları string'e çevir ve kaydet
                var yeniBaglantiStr = string.Join("|", 
                    baglantiMap.Select(kvp => $"{kvp.Key}:{string.Join(",", kvp.Value)}"));

                Preferences.Set("EtiketBelgeBaglantilari", yeniBaglantiStr);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Etiket-belge bağlantısı kaydetme hatası: {ex.Message}");
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

        /// <summary>
        /// İşlem geçmişine yeni bir kayıt ekler
        /// </summary>
        private async Task IslemGecmisineEkleAsync(string islemTuru)
        {
            try
            {
                var mevcutGecmis = Preferences.Get("IslemGecmisi", string.Empty);
                var gecmisList = new List<string>();
                
                if (!string.IsNullOrEmpty(mevcutGecmis))
                {
                    gecmisList = mevcutGecmis.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                
                // Yeni işlem formatı: "IslemTuru:Tarih"
                var yeniIslem = $"{islemTuru}:{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                gecmisList.Insert(0, yeniIslem); // En başa ekle (en yeni)
                
                // Maksimum 100 kayıt tut
                if (gecmisList.Count > 100)
                {
                    gecmisList = gecmisList.Take(100).ToList();
                }
                
                // Preferences'a kaydet
                var yeniGecmisStr = string.Join("|", gecmisList);
                Preferences.Set("IslemGecmisi", yeniGecmisStr);
                
                await Task.Delay(50);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"İşlem geçmişi kaydetme hatası: {ex.Message}");
            }
        }

        private async Task KisiBelgeBaglantisiKaydetAsync(int kisiId, int belgeId)
        {
            try
            {
                // Preferences kullanarak kişi-belge bağlantılarını sakla
                var mevcutBaglantilar = Preferences.Get("KisiBelgeBaglantilari", string.Empty);
                var baglantiList = new List<string>();

                if (!string.IsNullOrEmpty(mevcutBaglantilar))
                {
                    baglantiList = mevcutBaglantilar.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                // Yeni bağlantı formatı: "KisiID:BelgeID:Tarih"
                var yeniBaglanti = $"{kisiId}:{belgeId}:{DateTime.Now:yyyy-MM-dd}";
                
                // Aynı kişi-belge bağlantısı varsa güncelle, yoksa ekle
                var mevcutIndex = baglantiList.FindIndex(b => b.StartsWith($"{kisiId}:{belgeId}:"));
                if (mevcutIndex >= 0)
                {
                    baglantiList[mevcutIndex] = yeniBaglanti;
                }
                else
                {
                    baglantiList.Add(yeniBaglanti);
                }

                // Maksimum 1000 bağlantı sakla
                if (baglantiList.Count > 1000)
                {
                    baglantiList = baglantiList.TakeLast(1000).ToList();
                }

                // Güncellenmiş listeyi kaydet
                Preferences.Set("KisiBelgeBaglantilari", string.Join("|", baglantiList));

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Kişi-belge bağlantısı kaydetme hatası: {ex.Message}");
            }
        }

        private async Task BelgeKisisiYukleAsync(int belgeId)
        {
            try
            {
                // Kişi-belge bağlantılarını al
                var baglantilar = Preferences.Get("KisiBelgeBaglantilari", string.Empty);
                if (string.IsNullOrEmpty(baglantilar))
                    return;

                var baglantiListesi = baglantilar.Split('|', StringSplitOptions.RemoveEmptyEntries);

                // Bu belgeye bağlı kişiyi bul
                foreach (var baglanti in baglantiListesi)
                {
                    var parts = baglanti.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && int.TryParse(parts[0], out int kisiId) && int.TryParse(parts[1], out int belgId))
                    {
                        if (belgId == belgeId)
                        {
                            // Kişiyi bul ve seç
                            var kisi = await _kisiRepository.GetirAsync(kisiId);
                            if (kisi != null && kisi.Aktif)
                            {
                                SeciliKisi = Kisiler.FirstOrDefault(k => k.KisiID == kisiId);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Belge kişisi yükleme hatası: {ex.Message}");
            }
        }

        private async Task DragDropDosyaYukleAsync(string dosyaYolu)
        {
            try
            {
                if (!File.Exists(dosyaYolu))
                    return;

                // Dosyayı FileResult olarak sarma
                var fileName = Path.GetFileName(dosyaYolu);
                var mimeType = GetMimeType(Path.GetExtension(fileName));
                var fileResult = new FileResult(dosyaYolu, mimeType);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    SeciliDosya = fileResult;
                    
                    // Otomatik isimlendirme açıksa dosya adını belge adı yap
                    if (OtomatikIsimlendir)
                    {
                        BelgeAdi = Path.GetFileNameWithoutExtension(fileName);
                    }
                });

                // Bildirim göster
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Başarılı", 
                        $"'{fileName}' dosyası yüklendi!", "Tamam");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Drag & Drop dosya yükleme hatası: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        "Dosya yüklenirken bir hata oluştu.", "Tamam");
                });
            }
        }

        private string GetMimeType(string extension)
        {
            return extension?.ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tiff" or ".tif" => "image/tiff",
                ".txt" => "text/plain",
                ".rtf" => "application/rtf",
                _ => "application/octet-stream"
            };
        }

        #endregion
    }
} 