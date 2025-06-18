using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Arsivim.ViewModels
{
    public class IstatistiklerVM : BaseViewModel
    {
        private int _toplamBelgeSayisi = 0;
        private int _toplamKisiSayisi = 0;
        private int _toplamEtiketSayisi = 0;
        private int _buAyEklenen = 0;
        private int _buHaftaEklenen = 0;
        private int _bugünEklenen = 0;
        private long _toplamDosyaBoyutu = 0;
        private string _enPopulerEtiket = string.Empty;
        private string _sonIslem = string.Empty;
        private DateTime _sonGuncelleme = DateTime.Now;
        private int _favorilenmisBelgeSayisi = 0;
        private int _bugunkuIslemSayisi = 0;

        public IstatistiklerVM()
        {
            Title = "İstatistikler";

            // Commands
            YenileCommand = new Command(async () => await YenileAsync());
            DetayliRaporCommand = new Command(async () => await DetayliRaporAsync());
            VeriAktarCommand = new Command(async () => await VeriAktarAsync());

            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public int ToplamBelgeSayisi
        {
            get => _toplamBelgeSayisi;
            set => SetProperty(ref _toplamBelgeSayisi, value);
        }

        public int ToplamKisiSayisi
        {
            get => _toplamKisiSayisi;
            set => SetProperty(ref _toplamKisiSayisi, value);
        }

        public int ToplamEtiketSayisi
        {
            get => _toplamEtiketSayisi;
            set => SetProperty(ref _toplamEtiketSayisi, value);
        }

        public int BuAyEklenen
        {
            get => _buAyEklenen;
            set => SetProperty(ref _buAyEklenen, value);
        }

        public int BuHaftaEklenen
        {
            get => _buHaftaEklenen;
            set => SetProperty(ref _buHaftaEklenen, value);
        }

        public int BugünEklenen
        {
            get => _bugünEklenen;
            set => SetProperty(ref _bugünEklenen, value);
        }

        public long ToplamDosyaBoyutu
        {
            get => _toplamDosyaBoyutu;
            set => SetProperty(ref _toplamDosyaBoyutu, value);
        }

        public string ToplamDosyaBoyutuMetni => FormatFileSize(ToplamDosyaBoyutu);

        public string EnPopulerEtiket
        {
            get => _enPopulerEtiket;
            set => SetProperty(ref _enPopulerEtiket, value);
        }

        public string SonIslem
        {
            get => _sonIslem;
            set => SetProperty(ref _sonIslem, value);
        }

        public DateTime SonGuncelleme
        {
            get => _sonGuncelleme;
            set => SetProperty(ref _sonGuncelleme, value);
        }

        public int FavorilenmisBelgeSayisi
        {
            get => _favorilenmisBelgeSayisi;
            set => SetProperty(ref _favorilenmisBelgeSayisi, value);
        }

        public int BugunkuIslemSayisi
        {
            get => _bugunkuIslemSayisi;
            set => SetProperty(ref _bugunkuIslemSayisi, value);
        }

        // En çok kullanılan etiket property'si
        public string EnCokKullanilanEtiket
        {
            get => EnPopulerEtiket;
            set => EnPopulerEtiket = value;
        }

        // Belge türü istatistikleri (örnek veri)
        public List<BelgeTuruIstatistik> BelgeTuruIstatistikleri { get; } = new List<BelgeTuruIstatistik>
        {
            new BelgeTuruIstatistik { Tur = "PDF", Sayi = 456, Yuzde = 36.7 },
            new BelgeTuruIstatistik { Tur = "Word", Sayi = 234, Yuzde = 18.8 },
            new BelgeTuruIstatistik { Tur = "Excel", Sayi = 189, Yuzde = 15.2 },
            new BelgeTuruIstatistik { Tur = "PowerPoint", Sayi = 156, Yuzde = 12.5 },
            new BelgeTuruIstatistik { Tur = "Resim", Sayi = 134, Yuzde = 10.7 },
            new BelgeTuruIstatistik { Tur = "Diğer", Sayi = 78, Yuzde = 6.3 }
        };

        // Aylık aktiviteler (örnek veri)
        public List<AylikAktivite> AylikAktiviteler { get; } = new List<AylikAktivite>
        {
            new AylikAktivite { Ay = "Ocak", Aktivite = 45 },
            new AylikAktivite { Ay = "Şubat", Aktivite = 67 },
            new AylikAktivite { Ay = "Mart", Aktivite = 89 },
            new AylikAktivite { Ay = "Nisan", Aktivite = 112 },
            new AylikAktivite { Ay = "Mayıs", Aktivite = 98 },
            new AylikAktivite { Ay = "Haziran", Aktivite = 134 }
        };

        #endregion

        #region Commands

        public ICommand YenileCommand { get; }
        public ICommand DetayliRaporCommand { get; }
        public ICommand VeriAktarCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Örnek istatistikleri yükle
                ToplamBelgeSayisi = 1247;
                ToplamKisiSayisi = 89;
                ToplamEtiketSayisi = 12;
                BuAyEklenen = 156;
                BuHaftaEklenen = 23;
                BugünEklenen = 4;
                ToplamDosyaBoyutu = 2567890123; // ~2.4 GB
                EnPopulerEtiket = "Önemli";
                SonIslem = "Belge eklendi - 2 saat önce";
                SonGuncelleme = DateTime.Now;
                FavorilenmisBelgeSayisi = 45;
                BugunkuIslemSayisi = 12;

                OnPropertyChanged(nameof(ToplamDosyaBoyutuMetni));
                await Task.Delay(100); // Simulate loading
            });
        }

        private async Task YenileAsync()
        {
            await InitializeAsync();
        }

        private async Task DetayliRaporAsync()
        {
            var raporMetni = "DETAYLI İSTATİSTİK RAPORU\n\n" +
                           $"Toplam Belgeler: {ToplamBelgeSayisi}\n" +
                           $"Toplam Kişiler: {ToplamKisiSayisi}\n" +
                           $"Toplam Etiketler: {ToplamEtiketSayisi}\n" +
                           $"Bu Ay Eklenen: {BuAyEklenen}\n" +
                           $"Bu Hafta Eklenen: {BuHaftaEklenen}\n" +
                           $"Bugün Eklenen: {BugünEklenen}\n" +
                           $"Toplam Dosya Boyutu: {ToplamDosyaBoyutuMetni}\n" +
                           $"En Popüler Etiket: {EnPopulerEtiket}\n" +
                           $"Son İşlem: {SonIslem}";

            await Application.Current.MainPage.DisplayAlert("Detaylı Rapor", raporMetni, "Tamam");
        }

        private async Task VeriAktarAsync()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Veri Aktarma", 
                "İstatistik verileri Excel formatında dışa aktarılacak. Bu özellik yakında kullanılabilir olacak.", 
                "Tamam");
        }

        private static string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:N1} {suffixes[counter]}";
        }

        #endregion
    }

    // Belge türü istatistik modeli
    public class BelgeTuruIstatistik
    {
        public string Tur { get; set; } = string.Empty;
        public string BelgeTuru => Tur; // XAML için alias
        public int Sayi { get; set; }
        public double Yuzde { get; set; }
    }

    // Aylık aktivite modeli
    public class AylikAktivite
    {
        public string Ay { get; set; } = string.Empty;
        public int Aktivite { get; set; }

        // XAML’de IslemSayisi ile bind edebilmek için alias
        public int IslemSayisi
        {
            get => Aktivite;
            set => Aktivite = value;
        }

        public int EklemeSayisi { get; set; }
        public int GuncellemeSayisi { get; set; }
        public int SilmeSayisi { get; set; }
    }


}