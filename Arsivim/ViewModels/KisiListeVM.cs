using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Data.Repositories;

namespace Arsivim.ViewModels
{
    public class KisiListeVM : BaseViewModel
    {
        private readonly KisiRepository _kisiRepository;
        
        public ObservableCollection<Kisi> Kisiler { get; } = new();

        private string _aramaMetni = string.Empty;
        private Kisi? _seciliKisi;

        public KisiListeVM(KisiRepository kisiRepository)
        {
            _kisiRepository = kisiRepository;
            Title = "Kişiler";

            // Commands
            AraCommand = new Command(async () => await AraAsync());
            YenileCommand = new Command(async () => await YenileAsync());
            KisiEkleCommand = new Command(async () => await KisiEkleAsync());
            KisiSecCommand = new Command<Kisi>(async (kisi) => await KisiSecAsync(kisi));
            KisiSilCommand = new Command<Kisi>(async (kisi) => await KisiSilAsync(kisi));

            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public string AramaMetni
        {
            get => _aramaMetni;
            set => SetProperty(ref _aramaMetni, value);
        }

        public Kisi? SeciliKisi
        {
            get => _seciliKisi;
            set => SetProperty(ref _seciliKisi, value);
        }

        public int ToplamKisiSayisi => Kisiler.Count;
        public string ToplamKisiSayisiMetni => $"{ToplamKisiSayisi} kişi";

        #endregion

        #region Commands

        public ICommand AraCommand { get; }
        public ICommand YenileCommand { get; }
        public ICommand KisiEkleCommand { get; }
        public ICommand KisiSecCommand { get; }
        public ICommand KisiSilCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                await KisileriYukleAsync();
            });
        }

        private async Task KisileriYukleAsync()
        {
            var kisiler = await _kisiRepository.TumunuGetirAsync();
            
            Kisiler.Clear();
            foreach (var kisi in kisiler.OrderBy(k => k.TamAd))
            {
                Kisiler.Add(kisi);
            }

            OnPropertyChanged(nameof(ToplamKisiSayisi), nameof(ToplamKisiSayisiMetni));
        }

        private async Task AraAsync()
        {
            if (string.IsNullOrWhiteSpace(AramaMetni))
            {
                await KisileriYukleAsync();
                return;
            }

            await ExecuteAsync(async () =>
            {
                var tumKisiler = await _kisiRepository.TumunuGetirAsync();
                var filtrelenmisKisiler = tumKisiler.Where(k =>
                    k.TamAd.Contains(AramaMetni, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(k.Email) && k.Email.Contains(AramaMetni, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(k.Telefon) && k.Telefon.Contains(AramaMetni, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(k.Adres) && k.Adres.Contains(AramaMetni, StringComparison.OrdinalIgnoreCase))
                );
                
                Kisiler.Clear();
                foreach (var kisi in filtrelenmisKisiler.OrderBy(k => k.TamAd))
                {
                    Kisiler.Add(kisi);
                }

                OnPropertyChanged(nameof(ToplamKisiSayisi), nameof(ToplamKisiSayisiMetni));
            });
        }

        private async Task YenileAsync()
        {
            await ExecuteAsync(async () =>
            {
                await KisileriYukleAsync();
            });
        }

        private async Task KisiEkleAsync()
        {
            await Shell.Current.GoToAsync("///KisiEkle");
        }

        private async Task KisiSecAsync(Kisi kisi)
        {
            if (kisi == null) return;

            SeciliKisi = kisi;
            await Shell.Current.GoToAsync($"///KisiDetay?kisiId={kisi.KisiID}");
        }

        private async Task KisiSilAsync(Kisi kisi)
        {
            if (kisi == null) return;

            var result = await Application.Current.MainPage.DisplayAlert(
                "Kişi Sil", 
                $"'{kisi.TamAd}' adlı kişiyi silmek istediğinize emin misiniz?", 
                "Evet", "Hayır");

            if (result)
            {
                await ExecuteAsync(async () =>
                {
                    await _kisiRepository.SilAsync(kisi.KisiID);
                    Kisiler.Remove(kisi);
                    OnPropertyChanged(nameof(ToplamKisiSayisi), nameof(ToplamKisiSayisiMetni));
                    await Application.Current.MainPage.DisplayAlert("Başarılı", "Kişi başarıyla silindi.", "Tamam");
                });
            }
        }

        #endregion
    }
} 