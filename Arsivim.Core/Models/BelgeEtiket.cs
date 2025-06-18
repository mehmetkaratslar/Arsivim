using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Belge ve Etiket arasındaki çoktan çoğa ilişkiyi temsil eden model
    /// </summary>
    public class BelgeEtiket
    {
        /// <summary>
        /// Belge kimlik numarası (Foreign Key)
        /// </summary>
        [Required]
        public int BelgeID { get; set; }

        /// <summary>
        /// Etiket kimlik numarası (Foreign Key)
        /// </summary>
        [Required]
        public int EtiketID { get; set; }

        /// <summary>
        /// Etiketleme tarihi
        /// </summary>
        public DateTime EtiketlemeTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// İlişkili Belge nesnesi
        /// </summary>
        public virtual Belge Belge { get; set; } = null!;

        /// <summary>
        /// İlişkili Etiket nesnesi
        /// </summary>
        public virtual Etiket Etiket { get; set; } = null!;
    }
} 