using Arsivim.Core.Interfaces;
using Arsivim.Core.Models;
using Arsivim.Core.Enums;
using Arsivim.Data.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Arsivim.Services.Core
{
    public class BelgeYonetimi
    {
        private readonly BelgeRepository _belgeRepository;
        private readonly KisiRepository _kisiRepository;

        public BelgeYonetimi(BelgeRepository belgeRepository, KisiRepository kisiRepository)
        {
            _belgeRepository = belgeRepository;
            _kisiRepository = kisiRepository;
        }

        public async Task<IEnumerable<Belge>> TumBelgeleriGetirAsync()
        {
            return await _belgeRepository.GetAllAsync();
        }

        public async Task<Belge?> BelgeGetirAsync(int belgeId)
        {
            var belge = await _belgeRepository.GetByIdAsync(belgeId);
            if (belge != null)
            {
                await _belgeRepository.UpdateViewCountAsync(belgeId);
            }
            return belge;
        }

        public async Task<bool> BelgeEkleAsync(Belge belge, byte[] dosyaVerisi)
        {
            // Dosya hash'ini hesapla
            belge.DosyaHash = HashHesapla(dosyaVerisi);
            belge.Dosya = dosyaVerisi;
            belge.DosyaBoyutu = dosyaVerisi.Length;
            belge.YuklemeTarihi = DateTime.Now;
            belge.SonGuncelleme = DateTime.Now;

            // Belge türünü dosya uzantısından belirle
            belge.BelgeTipi = DosyaTipiniBelirle(belge.DosyaTipi);

            return await _belgeRepository.AddAsync(belge);
        }

        public async Task<bool> BelgeGuncelleAsync(Belge belge)
        {
            belge.SonGuncelleme = DateTime.Now;
            return await _belgeRepository.UpdateAsync(belge);
        }

        public async Task<bool> BelgeSilAsync(int belgeId)
        {
            var belge = await _belgeRepository.GetByIdAsync(belgeId);
            if (belge != null)
            {
                belge.Aktif = false;
                return await _belgeRepository.UpdateAsync(belge);
            }
            return false;
        }

        public async Task<IEnumerable<Belge>> BelgeAraAsync(string aramaMetni)
        {
            return await _belgeRepository.SearchAsync(aramaMetni);
        }

        public async Task<IEnumerable<Belge>> TipineGoreBelgeleriGetirAsync(BelgeTipi tip)
        {
            return await _belgeRepository.GetByTypeAsync(tip);
        }

        public async Task<IEnumerable<Belge>> SonEklenenBelgeleriGetirAsync(int adet = 10)
        {
            return await _belgeRepository.GetRecentAsync(adet);
        }

        public async Task<IEnumerable<Belge>> EnCokGoruntulenenBelgeleriGetirAsync(int adet = 10)
        {
            return await _belgeRepository.GetMostViewedAsync(adet);
        }

        public async Task<long> ToplamDosyaBoyutuAsync()
        {
            return await _belgeRepository.GetTotalSizeAsync();
        }

        public async Task<bool> KisiyeBelgeAtaAsync(int kisiId, int belgeId, string? iliskiTuru = null)
        {
            // Bu işlem için KisiBelge repository'si gerekli, şimdilik basit bir yaklaşım
            var kisi = await _kisiRepository.GetByIdAsync(kisiId);
            var belge = await _belgeRepository.GetByIdAsync(belgeId);

            if (kisi != null && belge != null)
            {
                // KisiBelge ilişkisi oluştur (bu kısım tam implementasyon gerektirir)
                return true;
            }
            return false;
        }

        private string HashHesapla(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(data);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

        private BelgeTipi DosyaTipiniBelirle(string? dosyaUzantisi)
        {
            if (string.IsNullOrEmpty(dosyaUzantisi))
                return BelgeTipi.Bilinmeyen;

            var uzanti = dosyaUzantisi.ToLower().TrimStart('.');

            return uzanti switch
            {
                "pdf" => BelgeTipi.PDF,
                "jpg" or "jpeg" or "png" or "bmp" or "gif" or "tiff" => BelgeTipi.Resim,
                "doc" or "docx" => BelgeTipi.Word,
                "xls" or "xlsx" => BelgeTipi.Excel,
                "ppt" or "pptx" => BelgeTipi.PowerPoint,
                "txt" => BelgeTipi.Metin,
                "zip" or "rar" or "7z" => BelgeTipi.Arsiv,
                "mp3" or "wav" or "flac" => BelgeTipi.Ses,
                "mp4" or "avi" or "mkv" or "mov" => BelgeTipi.Video,
                "xml" => BelgeTipi.XML,
                "json" => BelgeTipi.JSON,
                _ => BelgeTipi.Bilinmeyen
            };
        }

        public string DosyaBoyutuFormatla(long boyut)
        {
            string[] boyutBirimleri = { "B", "KB", "MB", "GB", "TB" };
            double boyutDouble = boyut;
            int birimIndex = 0;

            while (boyutDouble >= 1024 && birimIndex < boyutBirimleri.Length - 1)
            {
                boyutDouble /= 1024;
                birimIndex++;
            }

            return $"{boyutDouble:F2} {boyutBirimleri[birimIndex]}";
        }
    }
} 