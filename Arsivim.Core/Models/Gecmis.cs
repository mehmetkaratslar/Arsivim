using System.ComponentModel.DataAnnotations;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Sistem işlem geçmişini temsil eden model sınıfı
    /// </summary>
    public class Gecmis
    {
        /// <summary>
        /// Geçmiş kaydı kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// İşlem türü (Ekleme, Silme, Güncelleme, Görüntüleme vb.)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string IslemTuru { get; set; } = string.Empty;

        /// <summary>
        /// İlişkili belge kimlik numarası (opsiyonel)
        /// </summary>
        public int? BelgeID { get; set; }

        /// <summary>
        /// İşlem zamanı
        /// </summary>
        public DateTime Zaman { get; set; } = DateTime.Now;

        /// <summary>
        /// İşlem açıklaması
        /// </summary>
        [StringLength(1000)]
        public string? Aciklama { get; set; }

        /// <summary>
        /// İşlemi yapan kullanıcı
        /// </summary>
        [StringLength(50)]
        public string? Kullanici { get; set; }

        /// <summary>
        /// IP adresi
        /// </summary>
        [StringLength(45)]
        public string? IPAdresi { get; set; }

        /// <summary>
        /// İşlem öncesi değer (JSON)
        /// </summary>
        public string? OncekiDeger { get; set; }

        /// <summary>
        /// İşlem sonrası değer (JSON)
        /// </summary>
        public string? SonrakiDeger { get; set; }

        /// <summary>
        /// İşlem kategorisi
        /// </summary>
        [StringLength(100)]
        public string? Kategori { get; set; }

        /// <summary>
        /// İşlem durumu (Başarılı, Başarısız, Beklemede)
        /// </summary>
        [StringLength(20)]
        public string Durum { get; set; } = "Başarılı";

        /// <summary>
        /// Hata mesajı (varsa)
        /// </summary>
        [StringLength(1000)]
        public string? HataMesaji { get; set; }

        /// <summary>
        /// İlişkili Belge nesnesi
        /// </summary>
        public virtual Belge? Belge { get; set; }
    }
} 