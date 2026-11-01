namespace Library.Infrastructure.Mapping;

public interface IEntityMapper<T, TModel>
{
    TModel ToModel(T entity);
    T ToDomain(TModel model);
    void UpdateModel(T entity, TModel model);
}
