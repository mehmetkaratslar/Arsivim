namespace Arsivim.Core.Enums
{
    /// <summary>
    /// Belge türlerini tanımlayan enum
    /// </summary>
    public enum BelgeTipi
    {
        /// <summary>
        /// Bilinmeyen dosya türü
        /// </summary>
        Bilinmeyen = 0,

        /// <summary>
        /// PDF belgesi
        /// </summary>
        PDF = 1,

        /// <summary>
        /// Resim dosyası (JPG, PNG, BMP, GIF)
        /// </summary>
        Resim = 2,

        /// <summary>
        /// Microsoft Word belgesi
        /// </summary>
        Word = 3,

        /// <summary>
        /// Microsoft Excel belgesi
        /// </summary>
        Excel = 4,

        /// <summary>
        /// Metin dosyası
        /// </summary>
        Metin = 5,

        /// <summary>
        /// Sıkıştırılmış arşiv dosyası
        /// </summary>
        Arsiv = 6,

        /// <summary>
        /// Ses dosyası
        /// </summary>
        Ses = 7,

        /// <summary>
        /// Video dosyası
        /// </summary>
        Video = 8,

        /// <summary>
        /// PowerPoint sunumu
        /// </summary>
        PowerPoint = 9,

        /// <summary>
        /// XML dosyası
        /// </summary>
        XML = 10,

        /// <summary>
        /// JSON dosyası
        /// </summary>
        JSON = 11
    }
} 