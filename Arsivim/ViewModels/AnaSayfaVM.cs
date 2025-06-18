using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Services.Core;

namespace Arsivim.ViewModels
{
    /// <summary>
    /// Ana sayfa ViewModel'i
    /// </summary>
    public class AnaSayfaVM : BaseViewModel
    {
        private readonly BelgeYonetimi _belgeYonetimi;
        
        public ObservableCollection<Belge> SonBelgeler { get; } = new();
        public ObservableCollection<Belge> PopulerBelgeler { get; } = new();

        private int _toplamBelgeSayisi;
        private long _toplamDosyaBoyutu;
        private string _hosgeldinizMesaji = string.Empty;

        public AnaSayfaVM(BelgeYonetimi belgeYonetimi)
        {
            _belgeYonetimi = belgeYonetimi;
            Title = "Ana Sayfa";
            
            // Commands
            YenileCommand = new Command(async () => await YenileAsync());
            BelgeEkleCommand = new Command(async () => await BelgeEkleAsync());
            BelgeAcCommand = new Command<Belge>(async (belge) => await BelgeAcAsync(belge));
            EtiketlerCommand = new Command(async () => await NavigateToAsync("//EtiketYonetimi"));
            IstatistiklerCommand = new Command(async () => await NavigateToAsync("//Istatistikler"));
            GecmisCommand = new Command(async () => await NavigateToAsync("//Gecmis"));
            KullanicilarCommand = new Command(async () => await NavigateToAsync("//KullaniciYonetimi"));
            BelgelerCommand = new Command(async () => await NavigateToAsync("//Belgeler"));
            NavigateToCommand = new Command<string>(async (route) => await NavigateToAsync(route));

            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public int ToplamBelgeSayisi
        {
            get => _toplamBelgeSayisi;
            set => SetProperty(ref _toplamBelgeSayisi, value);
        }

        public long ToplamDosyaBoyutu
        {
            get => _toplamDosyaBoyutu;
            set => SetProperty(ref _toplamDosyaBoyutu, value);
        }

        public string ToplamDosyaBoyutuMetni => 
            Arsivim.Shared.Helpers.DosyaYardimcisi.BoyutFormatla(ToplamDosyaBoyutu);

        public string HosgeldinizMesaji
        {
            get => _hosgeldinizMesaji;
            set => SetProperty(ref _hosgeldinizMesaji, value);
        }

        #endregion

        #region Commands

        public ICommand YenileCommand { get; }
        public ICommand BelgeEkleCommand { get; }
        public ICommand BelgeAcCommand { get; }
        public ICommand EtiketlerCommand { get; }
        public ICommand IstatistiklerCommand { get; }
        public ICommand GecmisCommand { get; }
        public ICommand KullanicilarCommand { get; }
        public ICommand BelgelerCommand { get; }
        public ICommand NavigateToCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                HosgeldinizMesaji = $"Hoşgeldiniz! Bugün {DateTime.Now:dd MMMM yyyy}";
                
                await SonBelgeleriYukleAsync();
                await PopulerBelgeleriYukleAsync();
                await IstatistikleriYukleAsync();
            });
        }

        private async Task YenileAsync()
        {
            await ExecuteAsync(async () =>
            {
                await SonBelgeleriYukleAsync();
                await PopulerBelgeleriYukleAsync();
                await IstatistikleriYukleAsync();
            });
        }

        private async Task SonBelgeleriYukleAsync()
        {
            var belgeler = await _belgeYonetimi.SonEklenenBelgeleriGetirAsync(5);
            
            SonBelgeler.Clear();
            foreach (var belge in belgeler)
            {
                SonBelgeler.Add(belge);
            }
        }

        private async Task PopulerBelgeleriYukleAsync()
        {
            var belgeler = await _belgeYonetimi.EnCokGoruntulenenBelgeleriGetirAsync(5);
            
            PopulerBelgeler.Clear();
            foreach (var belge in belgeler)
            {
                PopulerBelgeler.Add(belge);
            }
        }

        private async Task IstatistikleriYukleAsync()
        {
            var tumBelgeler = await _belgeYonetimi.TumBelgeleriGetirAsync();
            ToplamBelgeSayisi = tumBelgeler.Count();
            ToplamDosyaBoyutu = await _belgeYonetimi.ToplamDosyaBoyutuAsync();
            
            OnPropertyChanged(nameof(ToplamDosyaBoyutuMetni));
        }

        private async Task BelgeEkleAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("BelgeEkle");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", 
                    $"Belge ekleme sayfasına gidilemedi: {ex.Message}", "Tamam");
            }
        }

        private async Task BelgeAcAsync(Belge belge)
        {
            if (belge == null) return;
            
            try
            {
                await Shell.Current.GoToAsync($"BelgeDetay?belgeId={belge.BelgeID}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", 
                    $"Belge detayına gidilemedi: {ex.Message}", "Tamam");
            }
        }

        private async Task NavigateToAsync(string route)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Navigating to: {route}");
                await Shell.Current.GoToAsync(route);
                System.Diagnostics.Debug.WriteLine($"Navigation successful to: {route}");
            }
            catch (Exception ex)
            {
                // Hata durumunda ana sayfada kal
                System.Diagnostics.Debug.WriteLine($"Navigation error to {route}: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Navigation Hatası", 
                    $"Sayfa açılamadı: {route}\nHata: {ex.Message}", "Tamam");
            }
        }

        #endregion
    }
} 