using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Uygulama ayarlarını temsil eden model sınıfı
    /// </summary>
    public class Ayarlar
    {
        /// <summary>
        /// Ayar kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int AyarID { get; set; }

        /// <summary>
        /// Ayar anahtarı
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Anahtar { get; set; } = string.Empty;

        /// <summary>
        /// Ayar değeri
        /// </summary>
        [StringLength(1000)]
        public string? Deger { get; set; }

        /// <summary>
        /// Ayar açıklaması
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        /// <summary>
        /// Ayar kategorisi
        /// </summary>
        [StringLength(50)]
        public string? Kategori { get; set; }

        /// <summary>
        /// Varsayılan değer
        /// </summary>
        [StringLength(1000)]
        public string? VarsayilanDeger { get; set; }

        /// <summary>
        /// Ayar türü (String, Boolean, Integer, Json vb.)
        /// </summary>
        [StringLength(20)]
        public string AyarTuru { get; set; } = "String";

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime SonGuncelleme { get; set; } = DateTime.Now;

        /// <summary>
        /// Kullanıcı tarafından değiştirilebilir mi?
        /// </summary>
        public bool KullaniciDegistirebilir { get; set; } = true;

        /// <summary>
        /// Sistem yeniden başlatma gerektirir mi?
        /// </summary>
        public bool YenidenBaslatmaGerektirir { get; set; } = false;

        /// <summary>
        /// Aktif durumu
        /// </summary>
        public bool Aktif { get; set; } = true;
    }
} 