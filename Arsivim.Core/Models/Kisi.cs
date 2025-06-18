using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Kişi bilgilerini temsil eden model sınıfı
    /// </summary>
    public class Kisi
    {
        /// <summary>
        /// Kişi kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int KisiID { get; set; }

        /// <summary>
        /// Kişinin adı
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Ad { get; set; } = string.Empty;

        /// <summary>
        /// Kişinin soyadı
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Soyad { get; set; } = string.Empty;

        /// <summary>
        /// Kişinin unvanı
        /// </summary>
        [StringLength(100)]
        public string? Unvan { get; set; }

        /// <summary>
        /// Çalıştığı şirket
        /// </summary>
        [StringLength(100)]
        public string? Sirket { get; set; }

        /// <summary>
        /// Kişinin doğum tarihi
        /// </summary>
        public DateTime? DogumTarihi { get; set; }

        /// <summary>
        /// Kişinin cinsiyeti
        /// </summary>
        [StringLength(10)]
        public string? Cinsiyet { get; set; }

        /// <summary>
        /// Telefon numarası
        /// </summary>
        [StringLength(20)]
        public string? Telefon { get; set; }

        /// <summary>
        /// E-posta adresi
        /// </summary>
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Adres bilgisi
        /// </summary>
        [StringLength(500)]
        public string? Adres { get; set; }

        /// <summary>
        /// Kişi hakkında notlar
        /// </summary>
        [StringLength(1000)]
        public string? Notlar { get; set; }

        /// <summary>
        /// Kişinin aktif durumu
        /// </summary>
        public bool Aktif { get; set; } = true;

        /// <summary>
        /// Kayıt oluşturulma tarihi
        /// </summary>
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// Kayıt oluşturulma tarihi (Backward compatibility)
        /// </summary>
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime GuncellenmeTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// Son güncelleme tarihi (Backward compatibility)
        /// </summary>
        public DateTime SonGuncelleme { get; set; } = DateTime.Now;

        /// <summary>
        /// Kişinin tam adı
        /// </summary>
        public string TamAd => $"{Ad} {Soyad}";

        /// <summary>
        /// Kişiye ait belgeler
        /// </summary>
        public virtual ICollection<KisiBelge> KisiBelgeleri { get; set; } = new List<KisiBelge>();
    }
} 