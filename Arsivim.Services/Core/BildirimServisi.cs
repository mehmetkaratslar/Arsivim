using Microsoft.Extensions.Logging;

namespace Arsivim.Services.Core
{
    public class BildirimServisi
    {
        private readonly ILogger<BildirimServisi> _logger;

        public BildirimServisi(ILogger<BildirimServisi> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Belge ekleme bildirimi gönderir
        /// </summary>
        public async Task BelgeEklendiBildirimiGonderAsync(string belgeAdi)
        {
            try
            {
                // Bildirim ayarı kontrol et - basit simülasyon
                var bildirimAktif = true; // Preferences.Get("BildirimlerAktif", true);
                if (!bildirimAktif)
                    return;

                // Platform-specific bildirim gönderimi
                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = 1001,
                    Title = "✅ Belge Eklendi",
                    Subtitle = "Arsivim",
                    Description = $"'{belgeAdi}' başarıyla arşivinize eklendi.",
                    BadgeNumber = 1,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1)
                    }
                });

                _logger.LogInformation($"Belge eklendi bildirimi gönderildi: {belgeAdi}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Belge eklendi bildirimi gönderilirken hata oluştu");
            }
        }

        /// <summary>
        /// Yedekleme bildirimi gönderir
        /// </summary>
        public async Task YedeklemeTamamlandiBildirimiGonderAsync()
        {
            try
            {
                var bildirimAktif = true; // Preferences.Get("BildirimlerAktif", true);
                if (!bildirimAktif)
                    return;

                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = 1002,
                    Title = "💾 Yedekleme Tamamlandı",
                    Subtitle = "Arsivim",
                    Description = "Belgelerinizin yedeklemesi başarıyla tamamlandı.",
                    BadgeNumber = 1,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1)
                    }
                });

                _logger.LogInformation("Yedekleme tamamlandı bildirimi gönderildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yedekleme bildirimi gönderilirken hata oluştu");
            }
        }

        /// <summary>
        /// OCR tamamlandı bildirimi gönderir
        /// </summary>
        public async Task OCRTamamlandiBildirimiGonderAsync(string belgeAdi)
        {
            try
            {
                var bildirimAktif = true; // Preferences.Get("BildirimlerAktif", true);
                if (!bildirimAktif)
                    return;

                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = 1003,
                    Title = "🔍 OCR İşlemi Tamamlandı",
                    Subtitle = "Arsivim",
                    Description = $"'{belgeAdi}' belgesi için metin çıkarma işlemi tamamlandı.",
                    BadgeNumber = 1,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1)
                    }
                });

                _logger.LogInformation($"OCR tamamlandı bildirimi gönderildi: {belgeAdi}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OCR bildirimi gönderilirken hata oluştu");
            }
        }

        /// <summary>
        /// Hatırlatma bildirimi gönderir
        /// </summary>
        public async Task HatirlatmaBildirimiGonderAsync(string mesaj)
        {
            try
            {
                var bildirimAktif = true; // Preferences.Get("BildirimlerAktif", true);
                if (!bildirimAktif)
                    return;

                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = 1004,
                    Title = "🔔 Hatırlatma",
                    Subtitle = "Arsivim",
                    Description = mesaj,
                    BadgeNumber = 1,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1)
                    }
                });

                _logger.LogInformation($"Hatırlatma bildirimi gönderildi: {mesaj}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hatırlatma bildirimi gönderilirken hata oluştu");
            }
        }

        /// <summary>
        /// Bildirim izni isteme
        /// </summary>
        public async Task<bool> BildirimIzniIsteAsync()
        {
            try
            {
                var result = await LocalNotificationCenter.Current.RequestNotificationPermission();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bildirim izni istenirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Tüm bildirimleri temizle
        /// </summary>
        public async Task TumBildirimleriTemizleAsync()
        {
            try
            {
                await LocalNotificationCenter.Current.Clear();
                _logger.LogInformation("Tüm bildirimler temizlendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bildirimler temizlenirken hata oluştu");
            }
        }
    }

    // Plugin.LocalNotification için basit wrapper
    public class LocalNotificationCenter
    {
        public static LocalNotificationCenter Current { get; } = new LocalNotificationCenter();

        public async Task<bool> Show(NotificationRequest request)
        {
            // Basit bildirim simülasyonu - gerçek implementasyon için Plugin.LocalNotification kullanılabilir
            await Task.Delay(100);
            
            // Debug için konsola yazdır
            System.Diagnostics.Debug.WriteLine($"📱 Bildirim: {request.Title} - {request.Description}");
            
            return true;
        }

        public async Task<bool> RequestNotificationPermission()
        {
            await Task.Delay(100);
            return true; // Simülasyon
        }

        public async Task Clear()
        {
            await Task.Delay(100);
            System.Diagnostics.Debug.WriteLine("🧹 Tüm bildirimler temizlendi");
        }
    }

    // Basit bildirim modelleri
    public class NotificationRequest
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int BadgeNumber { get; set; }
        public NotificationRequestSchedule? Schedule { get; set; }
    }

    public class NotificationRequestSchedule
    {
        public DateTime NotifyTime { get; set; }
    }
} 