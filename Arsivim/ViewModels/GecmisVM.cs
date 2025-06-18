using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;

namespace Arsivim.ViewModels
{
    public class GecmisVM : BaseViewModel
    {
        public ObservableCollection<Gecmis> GecmisListesi { get; } = new();

        private string _aramaMetni = string.Empty;
        private DateTime _baslangicTarihi = DateTime.Today.AddDays(-30);
        private DateTime _bitisTarihi = DateTime.Today;
        private Gecmis? _seciliIslem;
        private bool _filtreAcik = false;

        public GecmisVM()
        {
            Title = "İşlem Geçmişi";

            // Commands
            AraCommand = new Command(async () => await AraAsync());
            YenileCommand = new Command(async () => await YenileAsync());
            FiltreleCommand = new Command(async () => await FiltreleAsync());
            TemizleCommand = new Command(async () => await TemizleAsync());
            DetayGorCommand = new Command<Gecmis>(async (islem) => await DetayGorAsync(islem));
            FiltreToggleCommand = new Command(async () => await FiltreToggleAsync());
            TumGecmisiSilCommand = new Command(async () => await TumGecmisiSilAsync());
            FiltreUygulaCommand = new Command(async () => await FiltreleAsync());
            FiltreSifirlaCommand = new Command(async () => await TemizleAsync());

            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public string AramaMetni
        {
            get => _aramaMetni;
            set => SetProperty(ref _aramaMetni, value);
        }

        public DateTime BaslangicTarihi
        {
            get => _baslangicTarihi;
            set => SetProperty(ref _baslangicTarihi, value);
        }

        public DateTime BitisTarihi
        {
            get => _bitisTarihi;
            set => SetProperty(ref _bitisTarihi, value);
        }

        public Gecmis? SeciliIslem
        {
            get => _seciliIslem;
            set => SetProperty(ref _seciliIslem, value);
        }

        public bool FiltreAcik
        {
            get => _filtreAcik;
            set => SetProperty(ref _filtreAcik, value);
        }

        public int ToplamIslemSayisi => GecmisListesi.Count;
        public string ToplamIslemSayisiMetni => $"{ToplamIslemSayisi} işlem";
        public bool GecmisYok => GecmisListesi.Count == 0;

        // İşlem türleri listesi
        public List<string> IslemTurleri { get; } = new List<string>
        {
            "Tümü",
            "Belge Ekleme",
            "Belge Silme",
            "Belge Güncelleme",
            "Kişi Ekleme",
            "Kişi Silme",
            "Etiket Oluşturma",
            "Sistem Girişi",
            "Sistem Çıkışı"
        };

        private string _seciliIslemTuru = "Tümü";
        public string SeciliIslemTuru
        {
            get => _seciliIslemTuru;
            set => SetProperty(ref _seciliIslemTuru, value);
        }

        // Metin alias for AramaMetni
        public string Metin
        {
            get => AramaMetni;
            set => AramaMetni = value;
        }

        #endregion

        #region Commands

        public ICommand AraCommand { get; }
        public ICommand YenileCommand { get; }
        public ICommand FiltreleCommand { get; }
        public ICommand TemizleCommand { get; }
        public ICommand DetayGorCommand { get; }
        public ICommand FiltreToggleCommand { get; }
        public ICommand TumGecmisiSilCommand { get; }
        public ICommand FiltreUygulaCommand { get; }
        public ICommand FiltreSifirlaCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Örnek işlem geçmişi yükle
                GecmisListesi.Clear();
                var ornekIslemler = new[]
                {
                    new Gecmis 
                    { 
                        ID = 1, 
                        IslemTuru = "Belge Ekleme", 
                        Aciklama = "Yeni belge eklendi: Fatura_2024.pdf", 
                        Zaman = DateTime.Now.AddHours(-2),
                        Kullanici = "admin"
                    },
                    new Gecmis 
                    { 
                        ID = 2, 
                        IslemTuru = "Belge Silme", 
                        Aciklama = "Belge silindi: Eski_Dosya.doc", 
                        Zaman = DateTime.Now.AddHours(-5),
                        Kullanici = "admin"
                    },
                    new Gecmis 
                    { 
                        ID = 3, 
                        IslemTuru = "Kişi Ekleme", 
                        Aciklama = "Yeni kişi eklendi: Mehmet Yılmaz", 
                        Zaman = DateTime.Now.AddDays(-1),
                        Kullanici = "admin"
                    },
                    new Gecmis 
                    { 
                        ID = 4, 
                        IslemTuru = "Etiket Oluşturma", 
                        Aciklama = "Yeni etiket oluşturuldu: Acil", 
                        Zaman = DateTime.Now.AddDays(-2),
                        Kullanici = "admin"
                    },
                    new Gecmis 
                    { 
                        ID = 5, 
                        IslemTuru = "Sistem Girişi", 
                        Aciklama = "Kullanıcı sisteme giriş yaptı", 
                        Zaman = DateTime.Now.AddDays(-3),
                        Kullanici = "admin"
                    }
                };

                foreach (var islem in ornekIslemler.OrderByDescending(i => i.Zaman))
                {
                    GecmisListesi.Add(islem);
                }

                OnPropertyChanged(nameof(ToplamIslemSayisi), nameof(ToplamIslemSayisiMetni));
                await Task.Delay(100); // Simulate loading
            });
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

        private async Task FiltreleAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Filtreleme işlemi simülasyonu
                await Task.Delay(100);
            });
        }

        private async Task TemizleAsync()
        {
            AramaMetni = string.Empty;
            BaslangicTarihi = DateTime.Today.AddDays(-30);
            BitisTarihi = DateTime.Today;
            await InitializeAsync();
        }

        private async Task DetayGorAsync(Gecmis islem)
        {
            if (islem == null) return;

            SeciliIslem = islem;
            await Application.Current.MainPage.DisplayAlert(
                "İşlem Detayı",
                $"Tip: {islem.IslemTuru}\nAçıklama: {islem.Aciklama}\nTarih: {islem.Zaman:dd.MM.yyyy HH:mm}",
                "Tamam");
        }

        private async Task FiltreToggleAsync()
        {
            FiltreAcik = !FiltreAcik;
            await Task.Delay(100);
        }

        private async Task TumGecmisiSilAsync()
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Tüm Geçmişi Sil",
                "Tüm işlem geçmişini silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.",
                "Evet",
                "Hayır");

            if (result)
            {
                await ExecuteAsync(async () =>
                {
                    GecmisListesi.Clear();
                    OnPropertyChanged(nameof(ToplamIslemSayisi), nameof(ToplamIslemSayisiMetni), nameof(GecmisYok));
                    await Task.Delay(100);
                });
            }
        }

        #endregion
    }
} 