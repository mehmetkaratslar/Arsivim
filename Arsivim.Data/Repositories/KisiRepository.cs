using Microsoft.EntityFrameworkCore;
using Arsivim.Core.Models;
using Arsivim.Data.Context;
using Arsivim.Shared.Helpers;

namespace Arsivim.Data.Repositories
{
    public class KisiRepository : BaseRepository<Kisi>
    {
        public KisiRepository(ArsivimContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Kisi>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await TumunuGetirAsync();

            // Arama terimini temizle ve normalleştir
            var cleanedSearchTerm = AramaYardimcisi.CleanSearchTerm(searchTerm);
            if (string.IsNullOrWhiteSpace(cleanedSearchTerm))
                return await TumunuGetirAsync();

            var normalizedSearchTerm = AramaYardimcisi.NormalizeSearchTerm(cleanedSearchTerm);
            
            // Tüm kişileri getir ve memory'de filtrele
            var allKisiler = await _dbSet.Where(k => k.Aktif).ToListAsync();
            
            var matchedKisiler = allKisiler.Where(k =>
                AramaYardimcisi.IsMatch(k.Ad, searchTerm) ||
                AramaYardimcisi.IsMatch(k.Soyad, searchTerm) ||
                AramaYardimcisi.IsMatch(k.TamAd, searchTerm) ||
                (k.Email != null && AramaYardimcisi.IsMatch(k.Email, searchTerm)) ||
                (k.Telefon != null && AramaYardimcisi.IsMatch(k.Telefon, searchTerm)) ||
                (k.Sirket != null && AramaYardimcisi.IsMatch(k.Sirket, searchTerm)) ||
                (k.Unvan != null && AramaYardimcisi.IsMatch(k.Unvan, searchTerm)) ||
                // Basit SQL arama da ekleyelim yedek olarak
                k.Ad.ToLower().Contains(normalizedSearchTerm) ||
                k.Soyad.ToLower().Contains(normalizedSearchTerm) ||
                (k.Email != null && k.Email.ToLower().Contains(normalizedSearchTerm)) ||
                (k.Telefon != null && k.Telefon.Contains(searchTerm)) ||
                (k.Sirket != null && k.Sirket.ToLower().Contains(normalizedSearchTerm)) ||
                (k.Unvan != null && k.Unvan.ToLower().Contains(normalizedSearchTerm))
            )
            .OrderBy(k => k.Ad)
            .ThenBy(k => k.Soyad);

            return matchedKisiler;
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

        public async Task<IEnumerable<Kisi>> TumunuGetirAsync()
        {
            return await _dbSet.OrderBy(k => k.Ad).ThenBy(k => k.Soyad).ToListAsync();
        }

        public async Task SilAsync(int id)
        {
            var kisi = await GetByIdAsync(id);
            if (kisi != null)
            {
                await DeleteAsync(kisi);
            }
        }

        public async Task<Kisi?> GetirAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<bool> EkleAsync(Kisi kisi)
        {
            return await AddAsync(kisi);
        }

        public async Task<bool> GuncelleAsync(Kisi kisi)
        {
            return await UpdateAsync(kisi);
        }
    }
} 