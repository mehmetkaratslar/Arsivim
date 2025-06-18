using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Etiket bilgilerini temsil eden model sınıfı
    /// </summary>
    public class Etiket
    {
        /// <summary>
        /// Etiket kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int EtiketID { get; set; }

        /// <summary>
        /// Etiket adı
        /// </summary>
        [Required]
        [StringLength(100)]
        public string EtiketAdi { get; set; } = string.Empty;

        /// <summary>
        /// Etiket rengi (hex kod)
        /// </summary>
        [StringLength(7)]
        public string? Renk { get; set; }

        /// <summary>
        /// Etiket açıklaması
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// Aktif durumu
        /// </summary>
        public bool Aktif { get; set; } = true;

        /// <summary>
        /// Kullanım sayısı
        /// </summary>
        public int KullanimSayisi { get; set; } = 0;

        /// <summary>
        /// Etikete ait belgeler
        /// </summary>
        public virtual ICollection<BelgeEtiket> BelgeEtiketleri { get; set; } = new List<BelgeEtiket>();
    }
} 