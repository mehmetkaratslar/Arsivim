using Microsoft.EntityFrameworkCore;
using Arsivim.Core.Models;
using Arsivim.Data.Context;

namespace Arsivim.Data.Repositories
{
    public class KisiRepository : BaseRepository<Kisi>
    {
        public KisiRepository(ArsivimContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Kisi>> SearchAsync(string searchTerm)
        {
            return await _dbSet
                .Where(k => k.Ad.Contains(searchTerm) || 
                           k.Soyad.Contains(searchTerm) ||
                           k.Email!.Contains(searchTerm))
                .OrderBy(k => k.Ad)
                .ThenBy(k => k.Soyad)
                .ToListAsync();
        }

        public async Task<Kisi?> GetByFullNameAsync(string ad, string soyad)
        {
            return await _dbSet
                .FirstOrDefaultAsync(k => k.Ad == ad && k.Soyad == soyad);
        }

        public async Task<IEnumerable<Kisi>> GetWithDocumentCountAsync()
        {
            return await _dbSet
                .Include(k => k.KisiBelgeleri)
                .ThenInclude(kb => kb.Belge)
                .OrderBy(k => k.Ad)
                .ToListAsync();
        }

        public async Task<int> GetDocumentCountAsync(int kisiId)
        {
            return await _context.KisiBelgeler
                .CountAsync(kb => kb.KisiID == kisiId);
        }
    }
} 