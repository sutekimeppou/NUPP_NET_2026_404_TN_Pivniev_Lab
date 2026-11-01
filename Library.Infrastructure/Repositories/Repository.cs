using Library.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class Repository<T>(LibraryContext context) : IRepository<T> where T : class, IEntityModel
{
    private readonly DbSet<T> _set = context.Set<T>();

    public async Task<T> GetByIdAsync(int id)
    {
        return await _set.FindAsync(id) ?? throw new KeyNotFoundException("Запис не знайдено.");
    }

    public async Task<T?> FindByPublicIdAsync(Guid id)
    {
        return _set.Local.FirstOrDefault(x => x.PublicId == id)
            ?? await _set.SingleOrDefaultAsync(x => x.PublicId == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _set.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetPageAsync(int page, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1);
        long offset = ((long)page - 1) * amount;
        if (offset > int.MaxValue) return Array.Empty<T>();
        return await _set.AsNoTracking().OrderBy(x => x.Id)
            .Skip((int)offset).Take(amount).ToListAsync();
    }

    public async Task AddAsync(T entity) => await _set.AddAsync(entity);
    public Task Update(T entity)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(T entity)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() > 0;
}
