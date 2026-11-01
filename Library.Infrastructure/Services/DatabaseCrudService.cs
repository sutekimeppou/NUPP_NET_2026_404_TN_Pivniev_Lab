using Library.Common.Interfaces;
using Library.Infrastructure.Mapping;
using Library.Infrastructure.Models;
using Library.Infrastructure.Repositories;

namespace Library.Infrastructure.Services;

public class DatabaseCrudService<T, TModel>(
    IRepository<TModel> repository, IEntityMapper<T, TModel> mapper) : ICrudServiceAsync<T>
    where T : class, IEntity where TModel : class, IEntityModel
{
    public async Task<bool> CreateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        if (await repository.FindByPublicIdAsync(element.Id) is not null) return false;
        await repository.AddAsync(mapper.ToModel(element));
        return true;
    }

    public async Task<T> ReadAsync(Guid id)
    {
        TModel model = await repository.FindByPublicIdAsync(id)
            ?? throw new KeyNotFoundException("Запис не знайдено.");
        return mapper.ToDomain(model);
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return (await repository.GetAllAsync()).Select(mapper.ToDomain).ToArray();
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        return (await repository.GetPageAsync(page, amount)).Select(mapper.ToDomain).ToArray();
    }

    public async Task<bool> UpdateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        TModel? model = await repository.FindByPublicIdAsync(element.Id);
        if (model is null) return false;
        mapper.UpdateModel(element, model);
        await repository.Update(model);
        return true;
    }

    public async Task<bool> RemoveAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        TModel? model = await repository.FindByPublicIdAsync(element.Id);
        if (model is null) return false;
        await repository.Delete(model);
        return true;
    }

    public Task<bool> SaveAsync() => repository.SaveChangesAsync();
}
