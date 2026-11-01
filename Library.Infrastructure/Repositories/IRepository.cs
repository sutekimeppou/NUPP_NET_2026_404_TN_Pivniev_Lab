namespace Library.Infrastructure.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task Update(T entity);
    Task Delete(T entity);
    Task<T?> FindByPublicIdAsync(Guid id);
    Task<IEnumerable<T>> GetPageAsync(int page, int amount);
    Task<bool> SaveChangesAsync();
}
