using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;

namespace Arsivim.ViewModels
{
    public class EtiketYonetimVM : BaseViewModel
    {
        public ObservableCollection<Etiket> Etiketler { get; } = new();

        private string _yeniEtiketAdi = string.Empty;
        private string _yeniEtiketRengi = "#FF5722";
        private string _yeniEtiketAciklama = string.Empty;
        private Etiket? _seciliEtiket;
        private string _aramaMetni = string.Empty;
        private bool _duzenlemeModu = false;

        public EtiketYonetimVM()
        {
            Title = "Etiket Yönetimi";

            // Commands
            EtiketEkleCommand = new Command(async () => await EtiketEkleAsync());
            EtiketSilCommand = new Command<Etiket>(async (etiket) => await EtiketSilAsync(etiket));
            EtiketGuncelleCommand = new Command<Etiket>(async (etiket) => await EtiketGuncelleAsync(etiket));
            EtiketKaydetCommand = new Command(async () => await EtiketKaydetAsync());
            DuzenlemeIptalCommand = new Command(async () => await DuzenlemeIptalAsync());
            RenkSecCommand = new Command<string>(async (renk) => await RenkSecAsync(renk));
            AraCommand = new Command(async () => await AraAsync());
            YenileCommand = new Command(async () => await YenileAsync());

            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public string YeniEtiketAdi
        {
            get => _yeniEtiketAdi;
            set => SetProperty(ref _yeniEtiketAdi, value);
        }

        public string YeniEtiketRengi
        {
            get => _yeniEtiketRengi;
            set => SetProperty(ref _yeniEtiketRengi, value);
        }

        public string YeniEtiketRenk
        {
            get => _yeniEtiketRengi;
            set => SetProperty(ref _yeniEtiketRengi, value);
        }

        public string YeniEtiketAciklama
        {
            get => _yeniEtiketAciklama;
            set => SetProperty(ref _yeniEtiketAciklama, value);
        }

        public Etiket? SeciliEtiket
        {
            get => _seciliEtiket;
            set => SetProperty(ref _seciliEtiket, value);
        }

        public string AramaMetni
        {
            get => _aramaMetni;
            set => SetProperty(ref _aramaMetni, value);
        }

        public bool DuzenlemeModu
        {
            get => _duzenlemeModu;
            set => SetProperty(ref _duzenlemeModu, value);
        }

        public int ToplamEtiketSayisi => Etiketler.Count;
        public string ToplamEtiketSayisiMetni => $"{ToplamEtiketSayisi} etiket";
        public bool EtiketYok => Etiketler.Count == 0;
        public string Metin => EtiketYok ? "Henüz etiket eklenmemiş." : string.Empty;

        #endregion

        #region Commands

        public ICommand EtiketEkleCommand { get; }
        public ICommand EtiketSilCommand { get; }
        public ICommand EtiketGuncelleCommand { get; }
        public ICommand EtiketKaydetCommand { get; }
        public ICommand DuzenlemeIptalCommand { get; }
        public ICommand RenkSecCommand { get; }
        public ICommand AraCommand { get; }
        public ICommand YenileCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Örnek etiketler yükle
                Etiketler.Clear();
                var ornekEtiketler = new[]
                {
                    new Etiket { EtiketID = 1, EtiketAdi = "Önemli", Renk = "#F44336" },
                    new Etiket { EtiketID = 2, EtiketAdi = "İş", Renk = "#2196F3" },
                    new Etiket { EtiketID = 3, EtiketAdi = "Kişisel", Renk = "#4CAF50" },
                    new Etiket { EtiketID = 4, EtiketAdi = "Arşiv", Renk = "#FF9800" },
                    new Etiket { EtiketID = 5, EtiketAdi = "Fatura", Renk = "#9C27B0" }
                };

                foreach (var etiket in ornekEtiketler)
                {
                    Etiketler.Add(etiket);
                }

                OnPropertyChanged(nameof(ToplamEtiketSayisi), nameof(ToplamEtiketSayisiMetni));
                await Task.Delay(100); // Simulate loading
            });
        }

        private async Task EtiketEkleAsync()
        {
            if (string.IsNullOrWhiteSpace(YeniEtiketAdi))
                return;

            await ExecuteAsync(async () =>
            {
                var yeniEtiket = new Etiket
                {
                    EtiketID = Etiketler.Count + 1,
                    EtiketAdi = YeniEtiketAdi.Trim(),
                    Renk = YeniEtiketRengi
                };

                Etiketler.Add(yeniEtiket);
                YeniEtiketAdi = string.Empty;
                YeniEtiketRengi = "#FF5722";
                YeniEtiketAciklama = string.Empty;

                OnPropertyChanged(nameof(ToplamEtiketSayisi), nameof(ToplamEtiketSayisiMetni), nameof(EtiketYok), nameof(Metin));
                await Task.Delay(100); // Simulate save
            });
        }

        private async Task EtiketSilAsync(Etiket etiket)
        {
            if (etiket == null) return;

            var result = await Application.Current.MainPage.DisplayAlert(
                "Etiket Sil",
                $"'{etiket.EtiketAdi}' etiketini silmek istediğinizden emin misiniz?",
                "Evet",
                "Hayır");

            if (result)
            {
                await ExecuteAsync(async () =>
                {
                    Etiketler.Remove(etiket);
                    OnPropertyChanged(nameof(ToplamEtiketSayisi), nameof(ToplamEtiketSayisiMetni), nameof(EtiketYok), nameof(Metin));
                    await Task.Delay(100); // Simulate delete
                });
            }
        }

        private async Task EtiketGuncelleAsync(Etiket etiket)
        {
            if (etiket == null) return;

            SeciliEtiket = etiket;
            DuzenlemeModu = true;
            YeniEtiketAdi = etiket.EtiketAdi;
            YeniEtiketRengi = etiket.Renk ?? "#FF5722";
            await Task.Delay(100); // Simulate update
        }

        private async Task AraAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Arama işlemi simülasyonu
                await Task.Delay(100);
            });
        }

        private async Task YenileAsync()
        {
            await InitializeAsync();
        }

        private async Task EtiketKaydetAsync()
        {
            if (SeciliEtiket != null)
            {
                // Güncelleme işlemi
                SeciliEtiket.EtiketAdi = YeniEtiketAdi.Trim();
                SeciliEtiket.Renk = YeniEtiketRengi;
                
                await Task.Delay(100);
                await DuzenlemeIptalAsync();
            }
        }

        private async Task DuzenlemeIptalAsync()
        {
            DuzenlemeModu = false;
            SeciliEtiket = null;
            YeniEtiketAdi = string.Empty;
            YeniEtiketRengi = "#FF5722";
            await Task.Delay(100);
        }

        private async Task RenkSecAsync(string renk)
        {
            if (!string.IsNullOrEmpty(renk))
            {
                YeniEtiketRengi = renk;
            }
            await Task.Delay(50);
        }

        #endregion
    }
} 