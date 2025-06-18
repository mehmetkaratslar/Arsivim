using Microsoft.EntityFrameworkCore;
using Arsivim.Core.Models;
using Arsivim.Core.Enums;
using Arsivim.Data.Context;
using Arsivim.Shared.Helpers;

namespace Arsivim.Data.Repositories
{
    public class BelgeRepository : BaseRepository<Belge>
    {
        public BelgeRepository(ArsivimContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Belge>> GetByTypeAsync(BelgeTipi tip)
        {
            return await _dbSet
                .Where(b => b.BelgeTipi == tip && b.Aktif)
                .OrderByDescending(b => b.YuklemeTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Belge>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await TumunuGetirAsync();

            // Arama terimini temizle ve normalleştir
            var cleanedSearchTerm = AramaYardimcisi.CleanSearchTerm(searchTerm);
            if (string.IsNullOrWhiteSpace(cleanedSearchTerm))
                return await TumunuGetirAsync();

            var normalizedSearchTerm = AramaYardimcisi.NormalizeSearchTerm(cleanedSearchTerm);
            
            // Tüm belgeleri notları ile birlikte getir
            var allBelgeler = await _dbSet
                .Include(b => b.Notlar.Where(n => n.Aktif))
                .Include(b => b.BelgeEtiketleri)
                    .ThenInclude(be => be.Etiket)
                .Where(b => b.Aktif)
                .ToListAsync();
            
            var matchedBelgeler = allBelgeler.Where(b =>
                // Belge temel alanlarında arama
                AramaYardimcisi.IsMatch(b.BelgeAdi, searchTerm) ||
                (b.DosyaAciklamasi != null && AramaYardimcisi.IsMatch(b.DosyaAciklamasi, searchTerm)) ||
                (b.OCRMetni != null && AramaYardimcisi.IsMatch(b.OCRMetni, searchTerm)) ||
                
                // Belge notlarında arama
                b.Notlar.Any(n => n.Aktif && (
                    AramaYardimcisi.IsMatch(n.Baslik, searchTerm) ||
                    AramaYardimcisi.IsMatch(n.Icerik, searchTerm) ||
                    (n.Kategori != null && AramaYardimcisi.IsMatch(n.Kategori, searchTerm)) ||
                    (n.Etiketler != null && AramaYardimcisi.IsMatch(n.Etiketler, searchTerm))
                )) ||
                
                // Belge etiketlerinde arama
                b.BelgeEtiketleri.Any(be => 
                    AramaYardimcisi.IsMatch(be.Etiket.EtiketAdi, searchTerm) ||
                    (be.Etiket.Aciklama != null && AramaYardimcisi.IsMatch(be.Etiket.Aciklama, searchTerm))
                ) ||
                
                // Basit SQL arama da ekleyelim yedek olarak
                b.BelgeAdi.ToLower().Contains(normalizedSearchTerm) ||
                (b.DosyaAciklamasi != null && b.DosyaAciklamasi.ToLower().Contains(normalizedSearchTerm)) ||
                (b.OCRMetni != null && b.OCRMetni.ToLower().Contains(normalizedSearchTerm)) ||
                b.Notlar.Any(n => n.Aktif && (
                    n.Baslik.ToLower().Contains(normalizedSearchTerm) ||
                    n.Icerik.ToLower().Contains(normalizedSearchTerm) ||
                    (n.Kategori != null && n.Kategori.ToLower().Contains(normalizedSearchTerm)) ||
                    (n.Etiketler != null && n.Etiketler.ToLower().Contains(normalizedSearchTerm))
                ))
            )
            .OrderByDescending(b => b.YuklemeTarihi);

            return matchedBelgeler;
        }

        public async Task<IEnumerable<Belge>> GetRecentAsync(int count = 10)
        {
            return await _dbSet
                .Where(b => b.Aktif)
                .OrderByDescending(b => b.YuklemeTarihi)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Belge>> GetMostViewedAsync(int count = 10)
        {
            return await _dbSet
                .Where(b => b.Aktif)
                .OrderByDescending(b => b.GoruntulenmeSayisi)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> UpdateViewCountAsync(int belgeId)
        {
            var belge = await GetByIdAsync(belgeId);
            if (belge != null)
            {
                belge.GoruntulenmeSayisi++;
                belge.SonGoruntulenmeTarihi = DateTime.Now;
                return await UpdateAsync(belge);
            }
            return false;
        }

        public async Task<long> GetTotalSizeAsync()
        {
            return await _dbSet
                .Where(b => b.Aktif)
                .SumAsync(b => b.DosyaBoyutu);
        }

        public async Task<IEnumerable<Belge>> GetByPersonAsync(int kisiId)
        {
            return await _context.KisiBelgeler
                .Where(kb => kb.KisiID == kisiId)
                .Select(kb => kb.Belge)
                .Where(b => b.Aktif)
                .OrderByDescending(b => b.YuklemeTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Belge>> GetByTagAsync(int etiketId)
        {
            return await _context.BelgeEtiketler
                .Where(be => be.EtiketID == etiketId)
                .Select(be => be.Belge)
                .Where(b => b.Aktif)
                .OrderByDescending(b => b.YuklemeTarihi)
                .ToListAsync();
        }

        // BelgeYonetimi için uyumlu metodlar
        public async Task<Belge?> EkleAsync(Belge belge)
        {
            try
            {
                await _dbSet.AddAsync(belge);
                var result = await _context.SaveChangesAsync() > 0;
                return result ? belge : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> GuncelleAsync(Belge belge)
        {
            return await UpdateAsync(belge);
        }

        public async Task<bool> SilAsync(int belgeId)
        {
            var belge = await GetByIdAsync(belgeId);
            if (belge != null)
            {
                return await DeleteAsync(belge);
            }
            return false;
        }

        public async Task<Belge?> GetirAsync(int belgeId)
        {
            return await GetByIdAsync(belgeId);
        }

        public async Task<IEnumerable<Belge>> TumunuGetirAsync()
        {
            return await _dbSet
                .Where(b => b.Aktif)
                .OrderByDescending(b => b.YuklemeTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Belge>> TipineGoreGetirAsync(BelgeTipi tip)
        {
            return await GetByTypeAsync(tip);
        }

        public async Task<IEnumerable<Belge>> AraAsync(string aramaMetni)
        {
            return await SearchAsync(aramaMetni);
        }
    }
} 