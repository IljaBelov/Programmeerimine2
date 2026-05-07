using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data.Repositories
{
    public class BaseRepository<T> where T : Entity
    {
        protected readonly TodoContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(TodoContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<PagedResult<T>> GetAllPaged(int page, int pageSize)
        {
            var totalItems = await _dbSet.CountAsync();
            var items = await _dbSet
                .Skip((page - 1) * pageSize) // Пропускаем записи предыдущих страниц
                .Take(pageSize)             // Берем только нужное количество
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<List<T>> GetAll() => await _dbSet.ToListAsync();
        public async Task<T?> GetById(int id) => await _dbSet.FindAsync(id);
        public async Task Add(T entity) => await _dbSet.AddAsync(entity);
        public async Task Save() => await _context.SaveChangesAsync();
    }
}