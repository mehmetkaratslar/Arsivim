using System.ComponentModel.DataAnnotations;
using Arsivim.Core.Enums;

namespace Arsivim.Core.Models
{
    /// <summary>
    /// Belge bilgilerini temsil eden model sınıfı
    /// </summary>
    public class Belge
    {
        /// <summary>
        /// Belge kimlik numarası (Primary Key)
        /// </summary>
        [Key]
        public int BelgeID { get; set; }

        /// <summary>
        /// Belge türü
        /// </summary>
        public BelgeTipi BelgeTipi { get; set; }

        /// <summary>
        /// Belge adı
        /// </summary>
        [Required]
        [StringLength(200)]
        public string BelgeAdi { get; set; } = string.Empty;

        /// <summary>
        /// Dosya içeriği (BLOB)
        /// </summary>
        public byte[]? Dosya { get; set; }

        /// <summary>
        /// Dosya uzantısı (örn: .pdf, .jpg)
        /// </summary>
        [StringLength(20)]
        public string? DosyaTipi { get; set; }

        /// <summary>
        /// Dosya boyutu (byte)
        /// </summary>
        public long DosyaBoyutu { get; set; }

        /// <summary>
        /// Yükleme tarihi
        /// </summary>
        public DateTime YuklemeTarihi { get; set; } = DateTime.Now;

        /// <summary>
        /// Dosya açıklaması
        /// </summary>
        [StringLength(1000)]
        public string? DosyaAciklamasi { get; set; }

        /// <summary>
        /// Belge versiyonu
        /// </summary>
        public int Versiyon { get; set; } = 1;

        /// <summary>
        /// Görüntülenme sayısı
        /// </summary>
        public int GoruntulenmeSayisi { get; set; } = 0;

        /// <summary>
        /// Son görüntülenme tarihi
        /// </summary>
        public DateTime? SonGoruntulenmeTarihi { get; set; }

        /// <summary>
        /// Belgenin aktif olup olmadığı
        /// </summary>
        public bool Aktif { get; set; } = true;

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime SonGuncelleme { get; set; } = DateTime.Now;

        /// <summary>
        /// Dosya hash değeri (değişiklik tespiti için)
        /// </summary>
        [StringLength(64)]
        public string? DosyaHash { get; set; }

        /// <summary>
        /// OCR ile çıkarılan metin
        /// </summary>
        public string? OCRMetni { get; set; }

        /// <summary>
        /// Belgeye bağlı kişiler
        /// </summary>
        public virtual ICollection<KisiBelge> KisiBelgeleri { get; set; } = new List<KisiBelge>();

        /// <summary>
        /// Belgeye ait etiketler
        /// </summary>
        public virtual ICollection<BelgeEtiket> BelgeEtiketleri { get; set; } = new List<BelgeEtiket>();

        /// <summary>
        /// Belge versiyonları
        /// </summary>
        public virtual ICollection<BelgeVersiyon> Versiyonlar { get; set; } = new List<BelgeVersiyon>();

        /// <summary>
        /// Belgeye ait notlar
        /// </summary>
        public virtual ICollection<Not> Notlar { get; set; } = new List<Not>();

        /// <summary>
        /// Favori kaydı
        /// </summary>
        public virtual Favori? Favori { get; set; }
    }
} 