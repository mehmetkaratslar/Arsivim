using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Favori belgeler bilgilerini temsil eden model sınıfı
    /// </summary>
    public class Favori
    {
        /// <summary>
        /// Favori kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int FavoriID { get; set; }

        /// <summary>
        /// İlişkili belge kimlik numarası (Foreign Key)
        /// </summary>
        [Required]
        public int BelgeID { get; set; }

        /// <summary>
        /// Favorilere eklenme tarihi
        /// </summary>
        public DateTime Tarih { get; set; } = DateTime.Now;

        /// <summary>
        /// Favori notu
        /// </summary>
        [StringLength(500)]
        public string? Not { get; set; }

        /// <summary>
        /// Sıralama değeri
        /// </summary>
        public int Sira { get; set; } = 0;

        /// <summary>
        /// İlişkili Belge nesnesi
        /// </summary>
        public virtual Belge Belge { get; set; } = null!;
    }
} 