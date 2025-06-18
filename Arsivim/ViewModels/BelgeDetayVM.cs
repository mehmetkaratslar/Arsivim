using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Services.Core;
using Arsivim.Shared.Helpers;

namespace Arsivim.ViewModels
{
    [QueryProperty(nameof(BelgeId), "belgeId")]
    public class BelgeDetayVM : BaseViewModel
    {
        private readonly BelgeYonetimi _belgeYonetimi;
        
        private Belge? _belge;
        private string _belgeId = string.Empty;
        private bool _favorimi = false;
        private string _yeniNotBaslik = string.Empty;
        private string _yeniNotIcerik = string.Empty;

        public ObservableCollection<Not> Notlar { get; } = new();
        public ObservableCollection<BelgeVersiyon> Versiyonlar { get; } = new();

        public BelgeDetayVM(BelgeYonetimi belgeYonetimi)
        {
            _belgeYonetimi = belgeYonetimi;
            Title = "Belge Detayı";

            // Commands
            FavoriyeEkleCommand = new Command(async () => await FavoriyeEkleAsync());
            BelgeDuzenleCommand = new Command(async () => await BelgeDuzenleAsync());
            BelgeSilCommand = new Command(async () => await BelgeSilAsync());
            NotEkleCommand = new Command(async () => await NotEkleAsync());
            NotSilCommand = new Command<Not>(async (not) => await NotSilAsync(not));
            BelgeIndir = new Command(async () => await BelgeIndirAsync());
            BelgePaylas = new Command(async () => await BelgePaylasAsync());
            BelgeAcCommand = new Command(async () => await BelgeAcAsync());
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
                    _ = Task.Run(() => BelgeYukleAsync(id));
                }
            }
        }

        public Belge? Belge
        {
            get => _belge;
            set
            {
                SetProperty(ref _belge, value);
                OnPropertyChanged(nameof(BelgeAdi), nameof(BelgeBoyutuMetni), nameof(YuklemeTarihiMetni));
            }
        }

        public string BelgeAdi => Belge?.BelgeAdi ?? "Belge Bulunamadı";
        public string BelgeBoyutuMetni => Belge != null ? DosyaYardimcisi.BoyutFormatla(Belge.DosyaBoyutu) : "0 B";
        public string YuklemeTarihiMetni => Belge?.YuklemeTarihi.ToString("dd.MM.yyyy HH:mm") ?? "";

        public bool Favorimi
        {
            get => _favorimi;
            set => SetProperty(ref _favorimi, value);
        }

        public string YeniNotBaslik
        {
            get => _yeniNotBaslik;
            set => SetProperty(ref _yeniNotBaslik, value);
        }

        public string YeniNotIcerik
        {
            get => _yeniNotIcerik;
            set => SetProperty(ref _yeniNotIcerik, value);
        }

        #endregion

        #region Commands

        public ICommand FavoriyeEkleCommand { get; }
        public ICommand BelgeDuzenleCommand { get; }
        public ICommand BelgeSilCommand { get; }
        public ICommand NotEkleCommand { get; }
        public ICommand NotSilCommand { get; }
        public ICommand BelgeIndir { get; }
        public ICommand BelgePaylas { get; }
        public ICommand BelgeAcCommand { get; }

        #endregion

        #region Methods

        private async Task BelgeYukleAsync(int belgeId)
        {
            await ExecuteAsync(async () =>
            {
                Belge = await _belgeYonetimi.BelgeGetirAsync(belgeId);
                
                if (Belge != null)
                {
                    Title = $"Belge: {Belge.BelgeAdi}";
                    Favorimi = Belge.Favori != null;
                    
                    await NotlariYukleAsync();
                    await VersiyonlariYukleAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Belge bulunamadı.", "Tamam");
                    await Shell.Current.GoToAsync("..");
                }
            });
        }

        private async Task NotlariYukleAsync()
        {
            if (Belge == null) return;

            // Bu kısım tam implementasyon gerektirir - şimdilik örnek veriler
            Notlar.Clear();
            // var notlar = await _belgeYonetimi.BelgeNotlariGetirAsync(Belge.BelgeID);
            // foreach (var not in notlar) { Notlar.Add(not); }
        }

        private async Task VersiyonlariYukleAsync()
        {
            if (Belge == null) return;

            // Bu kısım tam implementasyon gerektirir
            Versiyonlar.Clear();
            // var versiyonlar = await _belgeYonetimi.BelgeVersiyonlariGetirAsync(Belge.BelgeID);
            // foreach (var versiyon in versiyonlar) { Versiyonlar.Add(versiyon); }
        }

        private async Task FavoriyeEkleAsync()
        {
            if (Belge == null) return;

            // Favori durumunu tersine çevir
            Favorimi = !Favorimi;
            
            // Bu kısım tam implementasyon gerektirir
            await Task.CompletedTask;
        }

        private async Task BelgeDuzenleAsync()
        {
            if (Belge == null) return;

            await Shell.Current.GoToAsync($"///BelgeDuzenle?belgeId={Belge.BelgeID}");
        }

        private async Task BelgeSilAsync()
        {
            if (Belge == null) return;

            var result = await Application.Current.MainPage.DisplayAlert(
                "Belge Sil", 
                $"'{Belge.BelgeAdi}' adlı belgeyi silmek istediğinize emin misiniz?", 
                "Evet", "Hayır");

            if (result)
            {
                await ExecuteAsync(async () =>
                {
                    var success = await _belgeYonetimi.BelgeSilAsync(Belge.BelgeID);
                    if (success)
                    {
                        await Application.Current.MainPage.DisplayAlert("Başarılı", "Belge başarıyla silindi.", "Tamam");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Belge silinirken bir hata oluştu.", "Tamam");
                    }
                });
            }
        }

        private async Task NotEkleAsync()
        {
            if (Belge == null || string.IsNullOrWhiteSpace(YeniNotBaslik) || string.IsNullOrWhiteSpace(YeniNotIcerik))
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Not başlığı ve içeriği boş olamaz.", "Tamam");
                return;
            }

            var yeniNot = new Not
            {
                Baslik = YeniNotBaslik,
                Icerik = YeniNotIcerik,
                BelgeID = Belge.BelgeID,
                Tarih = DateTime.Now
            };

            // Bu kısım tam implementasyon gerektirir
            Notlar.Add(yeniNot);
            
            YeniNotBaslik = string.Empty;
            YeniNotIcerik = string.Empty;

            await Application.Current.MainPage.DisplayAlert("Başarılı", "Not başarıyla eklendi.", "Tamam");
        }

        private async Task NotSilAsync(Not not)
        {
            if (not == null) return;

            var result = await Application.Current.MainPage.DisplayAlert(
                "Not Sil", 
                $"'{not.Baslik}' adlı notu silmek istediğinize emin misiniz?", 
                "Evet", "Hayır");

            if (result)
            {
                Notlar.Remove(not);
                await Application.Current.MainPage.DisplayAlert("Başarılı", "Not başarıyla silindi.", "Tamam");
            }
        }

        private async Task BelgeIndirAsync()
        {
            if (Belge == null) return;

            await ExecuteAsync(async () =>
            {
                try
                {
                    // Belge dosyasını al
                    var dosyaBytes = await _belgeYonetimi.BelgeDosyasiniIndirAsync(Belge.BelgeID);
                    if (dosyaBytes == null || dosyaBytes.Length == 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Belge dosyası bulunamadı.", "Tamam");
                        return;
                    }

                    // Dosya adını oluştur
                    var fileName = $"{Belge.BelgeAdi}{Belge.DosyaTipi}";
                    
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

        private async Task BelgePaylasAsync()
        {
            if (Belge == null) return;

            await ExecuteAsync(async () =>
            {
                try
                {
                    // Geçici dosya yolu al
                    var tempFilePath = await _belgeYonetimi.BelgePaylasAsync(Belge.BelgeID);
                    if (string.IsNullOrEmpty(tempFilePath))
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Paylaşım için dosya hazırlanamadı.", "Tamam");
                        return;
                    }

                    // Paylaşım işlemini başlat
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = $"{Belge.BelgeAdi} - Belge Paylaşımı",
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

        private async Task BelgeAcAsync()
        {
            if (Belge == null) return;

            await ExecuteAsync(async () =>
            {
                try
                {
                    // Belge dosyasını al
                    var dosyaBytes = await _belgeYonetimi.BelgeDosyasiniIndirAsync(Belge.BelgeID);
                    if (dosyaBytes == null || dosyaBytes.Length == 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Belge dosyası bulunamadı.", "Tamam");
                        return;
                    }

                    // Geçici dosya oluştur
                    var fileName = $"{Belge.BelgeAdi}{Belge.DosyaTipi}";
                    var tempPath = Path.GetTempPath();
                    var filePath = Path.Combine(tempPath, fileName);
                    
                    await File.WriteAllBytesAsync(filePath, dosyaBytes);

                    // Dosyayı varsayılan uygulama ile aç
                    await Launcher.Default.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath),
                        Title = $"{Belge.BelgeAdi} - Belge Görüntüle"
                    });
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        $"Belge açılırken bir hata oluştu: {ex.Message}", "Tamam");
                }
            });
        }

        #endregion
    }
} 