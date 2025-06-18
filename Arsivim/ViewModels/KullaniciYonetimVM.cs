using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;

namespace Arsivim.ViewModels
{
    public class KullaniciYonetimVM : BaseViewModel
    {
        public ObservableCollection<Kullanici> Kullanicilar { get; } = new();

        private string _yeniKullaniciAdi = string.Empty;
        private string _yeniKullaniciEmail = string.Empty;
        private string _yeniTamAd = string.Empty;
        private string _yeniKullaniciSifre = string.Empty;
        private Kullanici? _seciliKullanici;
        private string _aramaMetni = string.Empty;
        private bool _duzenlemeModu = false;
        private bool _sifreGoster = false;
        private bool _kullaniciEklemePaneli = false;

        public KullaniciYonetimVM()
        {
            Title = "Kullanıcı Yönetimi";

            // Commands
            KullaniciEkleCommand = new Command(async () => await KullaniciEkleAsync());
            KullaniciSilCommand = new Command<Kullanici>(async (kullanici) => await KullaniciSilAsync(kullanici));
            KullaniciGuncelleCommand = new Command<Kullanici>(async (kullanici) => await KullaniciGuncelleAsync(kullanici));
            AraCommand = new Command(async () => await AraAsync());
            YenileCommand = new Command(async () => await YenileAsync());
            KullaniciEklemePaneliToggleCommand = new Command(async () => await KullaniciEklemePaneliToggleAsync());
            SifreGosterToggleCommand = new Command(() => SifreGoster = !SifreGoster);
            DuzenlemeIptalCommand = new Command(async () => await DuzenlemeIptalAsync());
            YeniKullaniciCommand = new Command(async () => await KullaniciEkleAsync());

            // UI thread'de güvenli başlatma
            MainThread.BeginInvokeOnMainThread(async () => await InitializeAsync());
        }

        #region Properties

        public string YeniKullaniciAdi
        {
            get => _yeniKullaniciAdi;
            set => SetProperty(ref _yeniKullaniciAdi, value);
        }

        public string YeniKullaniciEmail
        {
            get => _yeniKullaniciEmail;
            set => SetProperty(ref _yeniKullaniciEmail, value);
        }

        public string YeniTamAd
        {
            get => _yeniTamAd;
            set => SetProperty(ref _yeniTamAd, value);
        }

        // YeniEmail property'si - YeniKullaniciEmail'e yönlendirme
        public string YeniEmail
        {
            get => YeniKullaniciEmail;
            set => YeniKullaniciEmail = value;
        }

        // YeniSifre property'si - YeniKullaniciSifre'ye yönlendirme
        public string YeniSifre
        {
            get => YeniKullaniciSifre;
            set => YeniKullaniciSifre = value;
        }

        private string _yeniSifreTekrar = string.Empty;
        public string YeniSifreTekrar
        {
            get => _yeniSifreTekrar;
            set => SetProperty(ref _yeniSifreTekrar, value);
        }

        private bool _yeniYonetici = false;
        public bool YeniYonetici
        {
            get => _yeniYonetici;
            set => SetProperty(ref _yeniYonetici, value);
        }

        private bool _yeniAktif = true;
        public bool YeniAktif
        {
            get => _yeniAktif;
            set => SetProperty(ref _yeniAktif, value);
        }

        public string YeniKullaniciSifre
        {
            get => _yeniKullaniciSifre;
            set => SetProperty(ref _yeniKullaniciSifre, value);
        }

        public Kullanici? SeciliKullanici
        {
            get => _seciliKullanici;
            set => SetProperty(ref _seciliKullanici, value);
        }

        public string AramaMetni
        {
            get => _aramaMetni;
            set => SetProperty(ref _aramaMetni, value);
        }

        // Metin property for empty message
        public string Metin => KullaniciYok ? "Henüz kullanıcı eklenmemiş." : string.Empty;

        public bool DuzenlemeModu
        {
            get => _duzenlemeModu;
            set => SetProperty(ref _duzenlemeModu, value);
        }

        public bool SifreGoster
        {
            get => _sifreGoster;
            set => SetProperty(ref _sifreGoster, value);
        }

        public bool KullaniciEklemePaneli
        {
            get => _kullaniciEklemePaneli;
            set => SetProperty(ref _kullaniciEklemePaneli, value);
        }

        public int ToplamKullaniciSayisi => Kullanicilar.Count;
        public string ToplamKullaniciSayisiMetni => $"{ToplamKullaniciSayisi} kullanıcı";
        public bool KullaniciYok => Kullanicilar.Count == 0;

        #endregion

        #region Commands

        public ICommand KullaniciEkleCommand { get; }
        public ICommand KullaniciSilCommand { get; }
        public ICommand KullaniciGuncelleCommand { get; }
        public ICommand AraCommand { get; }
        public ICommand YenileCommand { get; }
        public ICommand KullaniciEklemePaneliToggleCommand { get; }
        public ICommand SifreGosterToggleCommand { get; }
        public ICommand DuzenlemeIptalCommand { get; }
        public ICommand YeniKullaniciCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Örnek kullanıcılar yükle
                Kullanicilar.Clear();
                var ornekKullanicilar = new[]
                {
                    new Kullanici 
                    { 
                        KullaniciID = 1, 
                        KullaniciAdi = "admin", 
                        Email = "admin@arsivim.com", 
                        TamAd = "Admin User",
                        Aktif = true,
                        HesapOlusturmaTarihi = DateTime.Now.AddMonths(-6)
                    },
                    new Kullanici 
                    { 
                        KullaniciID = 2, 
                        KullaniciAdi = "mehmet.yilmaz", 
                        Email = "mehmet@example.com", 
                        TamAd = "Mehmet Yılmaz",
                        Aktif = true,
                        HesapOlusturmaTarihi = DateTime.Now.AddMonths(-3)
                    },
                    new Kullanici 
                    { 
                        KullaniciID = 3, 
                        KullaniciAdi = "ayse.kaya", 
                        Email = "ayse@example.com", 
                        TamAd = "Ayşe Kaya",
                        Aktif = true,
                        HesapOlusturmaTarihi = DateTime.Now.AddMonths(-1)
                    }
                };

                foreach (var kullanici in ornekKullanicilar)
                {
                    Kullanicilar.Add(kullanici);
                }

                OnPropertyChanged(nameof(ToplamKullaniciSayisi), nameof(ToplamKullaniciSayisiMetni));
                await Task.Delay(100); // Simulate loading
            });
        }

        private async Task KullaniciEkleAsync()
        {
            if (string.IsNullOrWhiteSpace(YeniKullaniciAdi))
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", "Kullanıcı adı boş olamaz.", "Tamam");
                return;
            }

            if (string.IsNullOrWhiteSpace(YeniSifre) && !DuzenlemeModu)
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", "Şifre boş olamaz.", "Tamam");
                return;
            }

            if (YeniSifre != YeniSifreTekrar)
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", "Şifreler eşleşmiyor.", "Tamam");
                return;
            }

            // E-posta kontrolü
            if (!string.IsNullOrWhiteSpace(YeniEmail) && !IsValidEmail(YeniEmail))
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", "Geçerli bir e-posta adresi giriniz.", "Tamam");
                return;
            }

            // Kullanıcı adı benzersizlik kontrolü
            if (Kullanicilar.Any(k => k.KullaniciAdi.ToLowerInvariant() == YeniKullaniciAdi.ToLowerInvariant() && 
                                     (!DuzenlemeModu || k.KullaniciID != SeciliKullanici?.KullaniciID)))
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", "Bu kullanıcı adı zaten kullanılıyor.", "Tamam");
                return;
            }

            await ExecuteAsync(async () =>
            {
                if (DuzenlemeModu && SeciliKullanici != null)
                {
                    // Güncelleme işlemi
                    SeciliKullanici.KullaniciAdi = YeniKullaniciAdi.Trim();
                    SeciliKullanici.Email = YeniEmail?.Trim();
                    SeciliKullanici.TamAd = YeniTamAd?.Trim();
                    SeciliKullanici.Aktif = YeniAktif;

                    await Application.Current.MainPage.DisplayAlert("Başarılı", "Kullanıcı başarıyla güncellendi!", "Tamam");
                    await DuzenlemeIptalAsync();
                }
                else
                {
                    // Yeni kullanıcı ekleme
                    var yeniKullanici = new Kullanici
                    {
                        KullaniciID = Kullanicilar.Count + 1,
                        KullaniciAdi = YeniKullaniciAdi.Trim(),
                        Email = YeniEmail?.Trim(),
                        TamAd = string.IsNullOrWhiteSpace(YeniTamAd) ? YeniKullaniciAdi.Trim() : YeniTamAd.Trim(),
                        Aktif = YeniAktif,
                        HesapOlusturmaTarihi = DateTime.Now
                    };

                    Kullanicilar.Add(yeniKullanici);
                    await Application.Current.MainPage.DisplayAlert("Başarılı", "Kullanıcı başarıyla eklendi!", "Tamam");
                    
                    // Formu temizle
                    YeniKullaniciAdi = string.Empty;
                    YeniEmail = string.Empty;
                    YeniTamAd = string.Empty;
                    YeniSifre = string.Empty;
                    YeniSifreTekrar = string.Empty;
                    YeniYonetici = false;
                    YeniAktif = true;
                    KullaniciEklemePaneli = false;
                }

                OnPropertyChanged(nameof(ToplamKullaniciSayisi), nameof(ToplamKullaniciSayisiMetni), nameof(KullaniciYok));
                await Task.Delay(500);
            });
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task KullaniciSilAsync(Kullanici kullanici)
        {
            if (kullanici == null) return;

            if (kullanici.KullaniciAdi == "admin")
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Hata", 
                    "Admin kullanıcısı silinemez.", 
                    "Tamam");
                return;
            }

            var result = await Application.Current.MainPage.DisplayAlert(
                "Kullanıcı Sil",
                $"'{kullanici.KullaniciAdi}' kullanıcısını silmek istediğinizden emin misiniz?",
                "Evet",
                "Hayır");

            if (result)
            {
                await ExecuteAsync(async () =>
                {
                    Kullanicilar.Remove(kullanici);
                    OnPropertyChanged(nameof(ToplamKullaniciSayisi), nameof(ToplamKullaniciSayisiMetni));
                    await Task.Delay(100); // Simulate delete
                });
            }
        }

        private async Task KullaniciGuncelleAsync(Kullanici kullanici)
        {
            if (kullanici == null) return;

            // Düzenleme moduna geç
            DuzenlemeModu = true;
            SeciliKullanici = kullanici;
            KullaniciEklemePaneli = true;

            // Form alanlarını doldur
            YeniKullaniciAdi = kullanici.KullaniciAdi;
            YeniEmail = kullanici.Email ?? string.Empty;
            YeniTamAd = kullanici.TamAd ?? string.Empty;
            YeniSifre = string.Empty;
            YeniSifreTekrar = string.Empty;
            YeniAktif = kullanici.Aktif;

            await Task.Delay(100);
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

                var tumKullanicilar = new[]
                {
                    new Kullanici { KullaniciID = 1, KullaniciAdi = "admin", Email = "admin@arsivim.com", TamAd = "Admin User", Aktif = true, HesapOlusturmaTarihi = DateTime.Now.AddMonths(-6) },
                    new Kullanici { KullaniciID = 2, KullaniciAdi = "mehmet.yilmaz", Email = "mehmet@example.com", TamAd = "Mehmet Yılmaz", Aktif = true, HesapOlusturmaTarihi = DateTime.Now.AddMonths(-3) },
                    new Kullanici { KullaniciID = 3, KullaniciAdi = "ayse.kaya", Email = "ayse@example.com", TamAd = "Ayşe Kaya", Aktif = true, HesapOlusturmaTarihi = DateTime.Now.AddMonths(-1) },
                    new Kullanici { KullaniciID = 4, KullaniciAdi = "ali.demir", Email = "ali@example.com", TamAd = "Ali Demir", Aktif = false, HesapOlusturmaTarihi = DateTime.Now.AddMonths(-2) },
                    new Kullanici { KullaniciID = 5, KullaniciAdi = "fatma.ozkan", Email = "fatma@example.com", TamAd = "Fatma Özkan", Aktif = true, HesapOlusturmaTarihi = DateTime.Now.AddDays(-14) }
                };

                var filtrelenmisKullanicilar = tumKullanicilar
                    .Where(k => k.KullaniciAdi.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ||
                               (k.TamAd?.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ?? false) ||
                               (k.Email?.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ?? false))
                    .OrderBy(k => k.KullaniciAdi)
                    .ToList();

                Kullanicilar.Clear();
                foreach (var kullanici in filtrelenmisKullanicilar)
                {
                    Kullanicilar.Add(kullanici);
                }

                OnPropertyChanged(nameof(ToplamKullaniciSayisi), nameof(ToplamKullaniciSayisiMetni), nameof(KullaniciYok));
                await Task.Delay(300);
            });
        }

        private async Task YenileAsync()
        {
            await InitializeAsync();
        }

        private async Task KullaniciEklemePaneliToggleAsync()
        {
            KullaniciEklemePaneli = !KullaniciEklemePaneli;
            await Task.Delay(100);
        }

        private async Task DuzenlemeIptalAsync()
        {
            // Düzenleme modunu kapat ve formu temizle
            DuzenlemeModu = false;
            SeciliKullanici = null;
            
            // Form alanlarını temizle
            YeniKullaniciAdi = string.Empty;
            YeniKullaniciEmail = string.Empty;
            YeniTamAd = string.Empty;
            YeniKullaniciSifre = string.Empty;
            YeniSifreTekrar = string.Empty;
            YeniYonetici = false;
            YeniAktif = true;
            
            await Task.Delay(50); // UI güncellenmesi için kısa bekleme
        }

        #endregion
    }
} 