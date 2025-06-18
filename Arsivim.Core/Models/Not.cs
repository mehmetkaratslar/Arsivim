using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Not bilgilerini temsil eden model sınıfı
    /// </summary>
    public class Not
    {
        /// <summary>
        /// Not kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int NotID { get; set; }

        /// <summary>
        /// Not başlığı
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Baslik { get; set; } = string.Empty;

        /// <summary>
        /// Not içeriği
        /// </summary>
        [Required]
        public string Icerik { get; set; } = string.Empty;

        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime Tarih { get; set; } = DateTime.Now;

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime SonGuncelleme { get; set; } = DateTime.Now;

        /// <summary>
        /// İlişkili belge kimlik numarası (opsiyonel)
        /// </summary>
        public int? BelgeID { get; set; }

        /// <summary>
        /// Not kategorisi
        /// </summary>
        [StringLength(100)]
        public string? Kategori { get; set; }

        /// <summary>
        /// Öncelik seviyesi (1-5)
        /// </summary>
        public int Oncelik { get; set; } = 3;

        /// <summary>
        /// Aktif durumu
        /// </summary>
        public bool Aktif { get; set; } = true;

        /// <summary>
        /// Etiketler (virgülle ayrılmış)
        /// </summary>
        [StringLength(500)]
        public string? Etiketler { get; set; }

        /// <summary>
        /// İlişkili Belge nesnesi
        /// </summary>
        public virtual Belge? Belge { get; set; }
    }
} 