using Microsoft.EntityFrameworkCore;
using Arsivim.Core.Models;
using Arsivim.Core.Enums;
using Arsivim.Data.Context;

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
            return await _dbSet
                .Where(b => b.Aktif && 
                           (b.BelgeAdi.Contains(searchTerm) || 
                            b.DosyaAciklamasi!.Contains(searchTerm) ||
                            b.OCRMetni!.Contains(searchTerm)))
                .OrderByDescending(b => b.YuklemeTarihi)
                .ToListAsync();
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
    }
} 