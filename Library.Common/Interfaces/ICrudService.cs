namespace Library.Common.Interfaces;

public interface ICrudService<T>
{
    void Create(T element);
    T Read(Guid id);
    IEnumerable<T> ReadAll();
    void Update(T element);
    void Remove(T element);
}
