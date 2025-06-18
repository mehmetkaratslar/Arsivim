using System.Collections.ObjectModel;
using System.Windows.Input;
using Arsivim.Core.Models;

namespace Arsivim.ViewModels
{
    public class EtiketYonetimVM : BaseViewModel
    {
        public ObservableCollection<Etiket> Etiketler { get; } = new();

        private string _aramaMetni = string.Empty;

        public EtiketYonetimVM()
        {
            Title = "Etiket Yönetimi";

            // Commands
            AraCommand = new Command(async () => await AraAsync());
            YenileCommand = new Command(async () => await YenileAsync());

            _ = Task.Run(InitializeAsync);
        }

        #region Properties

        public string AramaMetni
        {
            get => _aramaMetni;
            set => SetProperty(ref _aramaMetni, value);
        }

        public int ToplamEtiketSayisi => Etiketler.Count;
        public string ToplamEtiketSayisiMetni => $"{ToplamEtiketSayisi} etiket";
        public bool EtiketYok => Etiketler.Count == 0;
        public string Metin => EtiketYok ? "Henüz etiket eklenmemiş." : string.Empty;

        #endregion

        #region Commands

        public ICommand AraCommand { get; }
        public ICommand YenileCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Global etiket listesinden etiketleri yükle
                await GlobalEtiketleriYukleAsync();
                OnPropertyChanged(nameof(ToplamEtiketSayisi), nameof(ToplamEtiketSayisiMetni), nameof(EtiketYok), nameof(Metin));
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

                // Tüm etiketleri al ve filtrele
                var tumEtiketler = await GetTumEtiketlerAsync();
                
                var filtrelenmisEtiketler = tumEtiketler
                    .Where(e => e.EtiketAdi.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ||
                               (e.Aciklama?.ToLowerInvariant().Contains(AramaMetni.ToLowerInvariant()) ?? false))
                    .ToList();

                Etiketler.Clear();
                foreach (var etiket in filtrelenmisEtiketler)
                {
                    Etiketler.Add(etiket);
                }

                OnPropertyChanged(nameof(ToplamEtiketSayisi), nameof(ToplamEtiketSayisiMetni), nameof(EtiketYok), nameof(Metin));
                await Task.Delay(300); // Arama simülasyonu
            });
        }

        private async Task YenileAsync()
        {
            await ExecuteAsync(async () =>
            {
                Etiketler.Clear();
                
                // Global etiket listesinden etiketleri yükle
                await GlobalEtiketleriYukleAsync();

                OnPropertyChanged(nameof(ToplamEtiketSayisi), nameof(ToplamEtiketSayisiMetni), nameof(EtiketYok), nameof(Metin));
                await Task.Delay(100); // Simüle edilen yükleme
            });
        }

        private async Task GlobalEtiketleriYukleAsync()
        {
            try
            {
                Etiketler.Clear();
                var tumEtiketler = await GetTumEtiketlerAsync();
                
                foreach (var etiket in tumEtiketler)
                {
                    Etiketler.Add(etiket);
                }
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Global etiket yükleme hatası: {ex.Message}");
                await VarsayilanEtiketleriEkleAsync();
            }
        }

        private async Task<List<Etiket>> GetTumEtiketlerAsync()
        {
            try
            {
                var etiketListesi = new List<Etiket>();
                
                // Preferences'tan etiketleri al
                var globalEtiketlerStr = Preferences.Get("GlobalEtiketler", string.Empty);
                
                if (!string.IsNullOrEmpty(globalEtiketlerStr))
                {
                    var etiketArray = globalEtiketlerStr.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    var etiketId = 1;

                    foreach (var etiketStr in etiketArray)
                    {
                        var parcalar = etiketStr.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        if (parcalar.Length >= 2)
                        {
                            var etiketAdi = parcalar[0];
                            var renk = parcalar[1];
                            var tarih = parcalar.Length > 2 ? parcalar[2] : DateTime.Now.ToString("yyyy-MM-dd");

                            var etiket = new Etiket
                            {
                                EtiketID = etiketId++,
                                EtiketAdi = etiketAdi,
                                Renk = renk,
                                Aciklama = $"Belge yüklerken otomatik eklendi - {tarih}"
                            };

                            etiketListesi.Add(etiket);
                        }
                    }
                }
                
                await Task.CompletedTask;
                return etiketListesi;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Etiket listesi alma hatası: {ex.Message}");
                return new List<Etiket>();
            }
        }

        private async Task VarsayilanEtiketleriEkleAsync()
        {
            // Eğer hiç etiket yoksa bilgi mesajı göster
            await Task.CompletedTask;
        }

        #endregion
    }
} 