namespace HairStudio.Repository.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T?> GetByIdAsync<TKey>(TKey id) where TKey : notnull;
        public IQueryable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task AddAndSaveAsync(T entity);
        Task UpdateAndSaveAsync(T entity);
        Task DeleteAndSaveAsync(T entity);
        Task RemoveRangeAndSaveAsync(IEnumerable<T> entities);
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
