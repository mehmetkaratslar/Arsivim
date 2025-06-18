using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Belge versiyonlarını temsil eden model sınıfı
    /// </summary>
    public class BelgeVersiyon
    {
        /// <summary>
        /// Versiyon kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int VersiyonID { get; set; }

        /// <summary>
        /// İlişkili belge kimlik numarası (Foreign Key)
        /// </summary>
        [Required]
        public int BelgeID { get; set; }

        /// <summary>
        /// Versiyon numarası
        /// </summary>
        public int VersiyonNumarasi { get; set; }

        /// <summary>
        /// Versiyon tarihi
        /// </summary>
        public DateTime Tarih { get; set; } = DateTime.Now;

        /// <summary>
        /// Versiyon açıklaması
        /// </summary>
        [StringLength(1000)]
        public string? Aciklama { get; set; }

        /// <summary>
        /// Dosya içeriği (BLOB)
        /// </summary>
        public byte[]? Dosya { get; set; }

        /// <summary>
        /// Dosya türü
        /// </summary>
        [StringLength(20)]
        public string? DosyaTipi { get; set; }

        /// <summary>
        /// Dosya boyutu
        /// </summary>
        public long DosyaBoyutu { get; set; }

        /// <summary>
        /// Dosya hash değeri
        /// </summary>
        [StringLength(64)]
        public string? DosyaHash { get; set; }

        /// <summary>
        /// Oluşturan kullanıcı
        /// </summary>
        [StringLength(100)]
        public string? OlusturanKullanici { get; set; }

        /// <summary>
        /// İlişkili Belge nesnesi
        /// </summary>
        public virtual Belge Belge { get; set; } = null!;
    }
} 