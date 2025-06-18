using System.Windows.Input;
using Arsivim.Core.Models;
using Arsivim.Data.Repositories;

namespace Arsivim.ViewModels
{
    [QueryProperty(nameof(KisiId), "kisiId")]
    public class KisiEkleVM : BaseViewModel
    {
        private readonly KisiRepository _kisiRepository;
        
        private string _kisiId = string.Empty;
        private string _ad = string.Empty;
        private string _soyad = string.Empty;
        private string _unvan = string.Empty;
        private string _sirket = string.Empty;
        private string _telefon = string.Empty;
        private string _email = string.Empty;
        private string _adres = string.Empty;
        private string _notlar = string.Empty;
        private bool _duzenlemeModu = false;
        private Kisi? _mevcutKisi;

        public KisiEkleVM(KisiRepository kisiRepository)
        {
            _kisiRepository = kisiRepository;
            Title = "Yeni Kişi Ekle";
            
            // Commands
            KaydetCommand = new Command(async () => await KaydetAsync(), KaydetOlabilirMi);
            IptalCommand = new Command(async () => await IptalAsync());
        }

        #region Properties

        public string KisiId
        {
            get => _kisiId;
            set
            {
                SetProperty(ref _kisiId, value);
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id))
                {
                    _duzenlemeModu = true;
                    Title = "Kişi Düzenle";
                    _ = Task.Run(() => KisiYukleAsync(id));
                }
                else
                {
                    _duzenlemeModu = false;
                    Title = "Yeni Kişi Ekle";
                }
            }
        }

        public bool DuzenlemeModu
        {
            get => _duzenlemeModu;
            set => SetProperty(ref _duzenlemeModu, value);
        }

        public string Ad
        {
            get => _ad;
            set
            {
                SetProperty(ref _ad, value);
                ((Command)KaydetCommand).ChangeCanExecute();
            }
        }

        public string Soyad
        {
            get => _soyad;
            set
            {
                SetProperty(ref _soyad, value);
                ((Command)KaydetCommand).ChangeCanExecute();
            }
        }

        public string Unvan
        {
            get => _unvan;
            set => SetProperty(ref _unvan, value);
        }

        public string Sirket
        {
            get => _sirket;
            set => SetProperty(ref _sirket, value);
        }

        public string Telefon
        {
            get => _telefon;
            set
            {
                SetProperty(ref _telefon, value);
                ((Command)KaydetCommand).ChangeCanExecute();
            }
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Adres
        {
            get => _adres;
            set => SetProperty(ref _adres, value);
        }

        public string Notlar
        {
            get => _notlar;
            set => SetProperty(ref _notlar, value);
        }

        #endregion

        #region Commands

        public ICommand KaydetCommand { get; }
        public ICommand IptalCommand { get; }

        #endregion

        #region Methods

        private async Task KisiYukleAsync(int kisiId)
        {
            await ExecuteAsync(async () =>
            {
                _mevcutKisi = await _kisiRepository.GetirAsync(kisiId);
                if (_mevcutKisi != null)
                {
                    Ad = _mevcutKisi.Ad;
                    Soyad = _mevcutKisi.Soyad;
                    Unvan = _mevcutKisi.Unvan ?? string.Empty;
                    Sirket = _mevcutKisi.Sirket ?? string.Empty;
                    Telefon = _mevcutKisi.Telefon ?? string.Empty;
                    Email = _mevcutKisi.Email ?? string.Empty;
                    Adres = _mevcutKisi.Adres ?? string.Empty;
                    Notlar = _mevcutKisi.Notlar ?? string.Empty;
                }
            });
        }

        private void AlanlariTemizle()
        {
            Ad = string.Empty;
            Soyad = string.Empty;
            Unvan = string.Empty;
            Sirket = string.Empty;
            Telefon = string.Empty;
            Email = string.Empty;
            Adres = string.Empty;
            Notlar = string.Empty;
            _mevcutKisi = null;
        }

        private bool KaydetOlabilirMi()
        {
            return !string.IsNullOrWhiteSpace(Ad) && 
                   !string.IsNullOrWhiteSpace(Soyad) && 
                   !string.IsNullOrWhiteSpace(Telefon);
        }

        private async Task KaydetAsync()
        {
            if (!KaydetOlabilirMi())
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", 
                    "Lütfen ad, soyad ve telefon alanlarını doldurun.", "Tamam");
                return;
            }

            // E-posta validasyonu
            if (!string.IsNullOrEmpty(Email) && !IsValidEmail(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Uyarı", 
                    "Geçerli bir e-posta adresi girin.", "Tamam");
                return;
            }

            await ExecuteAsync(async () =>
            {
                try
                {
                    if (DuzenlemeModu && _mevcutKisi != null)
                    {
                        // Düzenleme modu
                        _mevcutKisi.Ad = Ad.Trim();
                        _mevcutKisi.Soyad = Soyad.Trim();
                        _mevcutKisi.Unvan = string.IsNullOrWhiteSpace(Unvan) ? null : Unvan.Trim();
                        _mevcutKisi.Sirket = string.IsNullOrWhiteSpace(Sirket) ? null : Sirket.Trim();
                        _mevcutKisi.Telefon = string.IsNullOrWhiteSpace(Telefon) ? null : Telefon.Trim();
                        _mevcutKisi.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
                        _mevcutKisi.Adres = string.IsNullOrWhiteSpace(Adres) ? null : Adres.Trim();
                        _mevcutKisi.Notlar = string.IsNullOrWhiteSpace(Notlar) ? null : Notlar.Trim();
                        _mevcutKisi.GuncellenmeTarihi = DateTime.Now;

                        var guncellemeSonucu = await _kisiRepository.GuncelleAsync(_mevcutKisi);

                        if (guncellemeSonucu)
                        {
                            await Application.Current.MainPage.DisplayAlert("Başarılı", 
                                "Kişi başarıyla güncellendi!", "Tamam");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Hata", 
                                "Kişi güncellenirken bir hata oluştu.", "Tamam");
                            return;
                        }
                    }
                    else
                    {
                        // Yeni kişi ekleme
                        var yeniKisi = new Kisi
                        {
                            Ad = Ad.Trim(),
                            Soyad = Soyad.Trim(),
                            Unvan = string.IsNullOrWhiteSpace(Unvan) ? null : Unvan.Trim(),
                            Sirket = string.IsNullOrWhiteSpace(Sirket) ? null : Sirket.Trim(),
                            Telefon = string.IsNullOrWhiteSpace(Telefon) ? null : Telefon.Trim(),
                            Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                            Adres = string.IsNullOrWhiteSpace(Adres) ? null : Adres.Trim(),
                            Notlar = string.IsNullOrWhiteSpace(Notlar) ? null : Notlar.Trim(),
                            OlusturmaTarihi = DateTime.Now,
                            GuncellenmeTarihi = DateTime.Now,
                            Aktif = true
                        };

                        var eklemeSonucu = await _kisiRepository.EkleAsync(yeniKisi);

                        if (eklemeSonucu)
                        {
                            await Application.Current.MainPage.DisplayAlert("Başarılı", 
                                "Kişi başarıyla eklendi!", "Tamam");

                            // Alanları temizle
                            if (!DuzenlemeModu)
                            {
                                AlanlariTemizle();
                            }
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Hata", 
                                "Kişi eklenirken bir hata oluştu.", "Tamam");
                            return;
                        }
                    }

                    // Kişiler sayfasına dön
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", 
                        $"Kişi kaydedilirken bir hata oluştu: {ex.Message}", "Tamam");
                }
            });
        }

        private async Task IptalAsync()
        {
            bool formDolumu = !string.IsNullOrWhiteSpace(Ad) || 
                              !string.IsNullOrWhiteSpace(Soyad) || 
                              !string.IsNullOrWhiteSpace(Telefon) ||
                              !string.IsNullOrWhiteSpace(Email);

            if (formDolumu)
            {
                var result = await Application.Current.MainPage.DisplayAlert("Onay", 
                    "Değişiklikler kaydedilmeyecek. Çıkmak istediğinize emin misiniz?", 
                    "Evet", "Hayır");
                
                if (!result) return;
            }

            await Shell.Current.GoToAsync("..");
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

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

        #endregion
    }
} 