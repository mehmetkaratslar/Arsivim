using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Kişi ve Belge arasındaki çoktan çoğa ilişkiyi temsil eden model
    /// </summary>
    public class KisiBelge
    {
        /// <summary>
        /// Kişi kimlik numarası (Foreign Key)
        /// </summary>
        [Required]
        public int KisiID { get; set; }

        /// <summary>
        /// Belge kimlik numarası (Foreign Key)
        /// </summary>
        [Required]
        public int BelgeID { get; set; }

        /// <summary>
        /// İlişki kurulma tarihi
        /// </summary>
        public DateTime IliskiTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// İlişki türü (Sahibi, Yetkili, Görüntüleyici vb.)
        /// </summary>
        [StringLength(50)]
        public string? IliskiTuru { get; set; }

        /// <summary>
        /// İlişkili Kişi nesnesi
        /// </summary>
        public virtual Kisi Kisi { get; set; } = null!;

        /// <summary>
        /// İlişkili Belge nesnesi
        /// </summary>
        public virtual Belge Belge { get; set; } = null!;
    }
} 