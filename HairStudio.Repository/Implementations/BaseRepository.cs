using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;

namespace HairStudio.Repository.Implementations
{
    public class BaseRepository<T> : IRepositoryBase<T> where T : class
    {
        protected readonly HairStudioContext _context;

        public BaseRepository(HairStudioContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll() => _context.Set<T>();
        public async Task<T?> GetByIdAsync<TKey>(TKey id) where TKey : notnull => await _context.Set<T>().FindAsync(id);
        public void Add(T entity) => _context.Set<T>().Add(entity);
        public void Update(T entity) => _context.Set<T>().Update(entity);
        public void Remove(T entity) => _context.Set<T>().Remove(entity);
        public void RemoveRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);
        public async Task AddAndSaveAsync(T entity)
        {
            Add(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAndSaveAsync(T entity)
        {
            Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAndSaveAsync(T entity)
        {
            Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveRangeAndSaveAsync(IEnumerable<T> entities)
        {
            RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
