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

        // Metin property for empty message
        public string Metin => GecmisYok ? "Henüz işlem geçmişi bulunmuyor." : string.Empty;

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
                if (string.IsNullOrWhiteSpace(AramaMetni))
                {
                    await YenileAsync();
                    return;
                }

                var tumIslemler = new[]
                {
                    new Gecmis { ID = 1, IslemTuru = "Belge Ekleme", Aciklama = "Yeni belge eklendi: Fatura_2024.pdf", Zaman = DateTime.Now.AddHours(-2), Kullanici = "admin" },
                    new Gecmis { ID = 2, IslemTuru = "Belge Silme", Aciklama = "Belge silindi: Eski_Dosya.doc", Zaman = DateTime.Now.AddHours(-5), Kullanici = "admin" },
                    new Gecmis { ID = 3, IslemTuru = "Kişi Ekleme", Aciklama = "Yeni kişi eklendi: Mehmet Yılmaz", Zaman = DateTime.Now.AddDays(-1), Kullanici = "admin" },
                    new Gecmis { ID = 4, IslemTuru = "Etiket Oluşturma", Aciklama = "Yeni etiket oluşturuldu: Acil", Zaman = DateTime.Now.AddDays(-2), Kullanici = "admin" },
                    new Gecmis { ID = 5, IslemTuru = "Sistem Girişi", Aciklama = "Kullanıcı sisteme giriş yaptı", Zaman = DateTime.Now.AddDays(-3), Kullanici = "admin" },
                    new Gecmis { ID = 6, IslemTuru = "Belge Güncelleme", Aciklama = "Belge güncellendi: Sözleşme.pdf", Zaman = DateTime.Now.AddDays(-4), Kullanici = "mehmet.yilmaz" },
                    new Gecmis { ID = 7, IslemTuru = "Kişi Silme", Aciklama = "Kişi silindi: Eski Müşteri", Zaman = DateTime.Now.AddDays(-5), Kullanici = "ayse.kaya" },
                    new Gecmis { ID = 8, IslemTuru = "Sistem Çıkışı", Aciklama = "Kullanıcı sistemden çıkış yaptı", Zaman = DateTime.Now.AddDays(-6), Kullanici = "admin" }
                };

                var filtrelenmisIslemler = tumIslemler
                    .Where(i => i.IslemTuru.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ||
                               i.Aciklama.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ||
                               i.Kullanici.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()))
                    .OrderByDescending(i => i.Zaman)
                    .ToList();

                GecmisListesi.Clear();
                foreach (var islem in filtrelenmisIslemler)
                {
                    GecmisListesi.Add(islem);
                }

                OnPropertyChanged(nameof(ToplamIslemSayisi), nameof(ToplamIslemSayisiMetni), nameof(GecmisYok));
                await Task.Delay(300);
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
                var tumIslemler = new[]
                {
                    new Gecmis { ID = 1, IslemTuru = "Belge Ekleme", Aciklama = "Yeni belge eklendi: Fatura_2024.pdf", Zaman = DateTime.Now.AddHours(-2), Kullanici = "admin" },
                    new Gecmis { ID = 2, IslemTuru = "Belge Silme", Aciklama = "Belge silindi: Eski_Dosya.doc", Zaman = DateTime.Now.AddHours(-5), Kullanici = "admin" },
                    new Gecmis { ID = 3, IslemTuru = "Kişi Ekleme", Aciklama = "Yeni kişi eklendi: Mehmet Yılmaz", Zaman = DateTime.Now.AddDays(-1), Kullanici = "admin" },
                    new Gecmis { ID = 4, IslemTuru = "Etiket Oluşturma", Aciklama = "Yeni etiket oluşturuldu: Acil", Zaman = DateTime.Now.AddDays(-2), Kullanici = "admin" },
                    new Gecmis { ID = 5, IslemTuru = "Sistem Girişi", Aciklama = "Kullanıcı sisteme giriş yaptı", Zaman = DateTime.Now.AddDays(-3), Kullanici = "admin" },
                    new Gecmis { ID = 6, IslemTuru = "Belge Güncelleme", Aciklama = "Belge güncellendi: Sözleşme.pdf", Zaman = DateTime.Now.AddDays(-4), Kullanici = "mehmet.yilmaz" },
                    new Gecmis { ID = 7, IslemTuru = "Kişi Silme", Aciklama = "Kişi silindi: Eski Müşteri", Zaman = DateTime.Now.AddDays(-5), Kullanici = "ayse.kaya" },
                    new Gecmis { ID = 8, IslemTuru = "Sistem Çıkışı", Aciklama = "Kullanıcı sistemden çıkış yaptı", Zaman = DateTime.Now.AddDays(-6), Kullanici = "admin" }
                };

                var filtrelenmisIslemler = tumIslemler.AsEnumerable();

                // İşlem türü filtresi
                if (SeciliIslemTuru != "Tümü")
                {
                    filtrelenmisIslemler = filtrelenmisIslemler.Where(i => i.IslemTuru == SeciliIslemTuru);
                }

                // Tarih aralığı filtresi
                filtrelenmisIslemler = filtrelenmisIslemler.Where(i => 
                    i.Zaman.Date >= BaslangicTarihi.Date && 
                    i.Zaman.Date <= BitisTarihi.Date);

                // Arama metni filtresi
                if (!string.IsNullOrWhiteSpace(AramaMetni))
                {
                    filtrelenmisIslemler = filtrelenmisIslemler.Where(i => 
                        i.IslemTuru.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ||
                        i.Aciklama.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ||
                        i.Kullanici.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()));
                }

                GecmisListesi.Clear();
                foreach (var islem in filtrelenmisIslemler.OrderByDescending(i => i.Zaman))
                {
                    GecmisListesi.Add(islem);
                }

                OnPropertyChanged(nameof(ToplamIslemSayisi), nameof(ToplamIslemSayisiMetni), nameof(GecmisYok));
                await Task.Delay(500);
                
                await Application.Current.MainPage.DisplayAlert("Filtre", $"{GecmisListesi.Count} işlem bulundu.", "Tamam");
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