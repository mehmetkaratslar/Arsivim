using Microsoft.EntityFrameworkCore;
using Arsivim.Data.Context;

namespace Arsivim.Data.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly ArsivimContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ArsivimContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
} 