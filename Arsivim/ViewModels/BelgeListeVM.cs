using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Core.Enums;
using Arsivim.Services.Core;
using Arsivim.Shared.Helpers;

namespace Arsivim.ViewModels
{
    public class BelgeListeVM : BaseViewModel
    {
        private readonly BelgeYonetimi _belgeYonetimi;
        
        public ObservableCollection<Belge> Belgeler { get; } = new();
        public ObservableCollection<BelgeTipi> BelgeTipleri { get; } = new();

        private string _aramaMetni = string.Empty;
        private BelgeTipi _seciliBelgeTipi = BelgeTipi.Bilinmeyen;
        private Belge? _seciliBelge;
        private bool _sadeceFavoriler = false;

        public BelgeListeVM(BelgeYonetimi belgeYonetimi)
        {
            _belgeYonetimi = belgeYonetimi;
            Title = "Belgeler";

            // Commands
            AraCommand = new Command(async () => await AraAsync());
            YenileCommand = new Command(async () => await YenileAsync());
            BelgeEkleCommand = new Command(async () => await BelgeEkleAsync());
            BelgeSecCommand = new Command<Belge>(async (belge) => await BelgeSecAsync(belge));
            BelgeSilCommand = new Command<Belge>(async (belge) => await BelgeSilAsync(belge));
            FiltreTemizleCommand = new Command(async () => await FiltreTemizleAsync());

            // Belge tiplerini yükle
            BelgeTipleriniYukle();
            
            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public string AramaMetni
        {
            get => _aramaMetni;
            set => SetProperty(ref _aramaMetni, value);
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
        public ICommand FiltreTemizleCommand { get; }

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
                var sonuclar = await _belgeYonetimi.BelgeAraAsync(AramaMetni);
                
                Belgeler.Clear();
                foreach (var belge in sonuclar.OrderByDescending(b => b.YuklemeTarihi))
                {
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
            await Shell.Current.GoToAsync("//BelgeEkle");
        }

        private async Task BelgeSecAsync(Belge belge)
        {
            if (belge == null) return;

            SeciliBelge = belge;
            await Shell.Current.GoToAsync($"//BelgeDetay?belgeId={belge.BelgeID}");
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

        private void BelgeTipleriniYukle()
        {
            BelgeTipleri.Clear();
            foreach (BelgeTipi tip in Enum.GetValues<BelgeTipi>())
            {
                BelgeTipleri.Add(tip);
            }
        }

        public string BelgeBoyutuFormatla(long boyut)
        {
            return DosyaYardimcisi.BoyutFormatla(boyut);
        }

        #endregion
    }
} 