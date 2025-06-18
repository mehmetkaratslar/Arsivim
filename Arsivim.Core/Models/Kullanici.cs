using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Kullanıcı bilgilerini temsil eden model sınıfı
    /// </summary>
    public class Kullanici
    {
        /// <summary>
        /// Kullanıcı kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int KullaniciID { get; set; }

        /// <summary>
        /// Kullanıcı adı
        /// </summary>
        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        /// <summary>
        /// Şifreli parola (Hash)
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Sifre { get; set; } = string.Empty;

        /// <summary>
        /// Şifre salt değeri
        /// </summary>
        [StringLength(128)]
        public string? Salt { get; set; }

        /// <summary>
        /// E-posta adresi
        /// </summary>
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Tam adı
        /// </summary>
        [StringLength(100)]
        public string? TamAd { get; set; }

        /// <summary>
        /// Son giriş zamanı
        /// </summary>
        public DateTime? SonGirisZamani { get; set; }

        /// <summary>
        /// Hesap oluşturulma tarihi
        /// </summary>
        public DateTime HesapOlusturmaTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// Aktif durumu
        /// </summary>
        public bool Aktif { get; set; } = true;

        /// <summary>
        /// Yönetici yetkisi
        /// </summary>
        public bool Yonetici { get; set; } = false;

        /// <summary>
        /// Başarısız giriş sayısı
        /// </summary>
        public int BasarisizGirisSayisi { get; set; } = 0;

        /// <summary>
        /// Hesap kilitli mi?
        /// </summary>
        public bool HesapKilitli { get; set; } = false;

        /// <summary>
        /// Hesap kilit süresi
        /// </summary>
        public DateTime? KilitSuresi { get; set; }

        /// <summary>
        /// Son parola değiştirme tarihi
        /// </summary>
        public DateTime? SonParolaDegisikligi { get; set; }

        /// <summary>
        /// İki faktörlü kimlik doğrulama aktif mi?
        /// </summary>
        public bool IkiFaktorAktif { get; set; } = false;

        /// <summary>
        /// Profil resmi
        /// </summary>
        public byte[]? ProfilResmi { get; set; }
    }
} 