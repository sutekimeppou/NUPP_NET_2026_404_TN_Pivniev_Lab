using System.Text.Json;
using Library.Common.Interfaces;

namespace Library.Common.Services;

public class CrudService<T> : ICrudService<T> where T : class, IEntity
{
    private Dictionary<Guid, T> _elements = new();

    public void Create(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (!_elements.TryAdd(element.Id, element))
        {
            throw new InvalidOperationException("Об'єкт із таким Id уже існує.");
        }
    }

    public T Read(Guid id)
    {
        if (!_elements.TryGetValue(id, out T? element))
        {
            throw new KeyNotFoundException("Об'єкт із таким Id не знайдено.");
        }

        return element;
    }

    public IEnumerable<T> ReadAll()
    {
        return _elements.Values.ToArray();
    }

    public void Update(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (!_elements.ContainsKey(element.Id))
        {
            throw new KeyNotFoundException("Об'єкт для оновлення не знайдено.");
        }

        _elements[element.Id] = element;
    }

    public void Remove(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (!_elements.Remove(element.Id))
        {
            throw new KeyNotFoundException("Об'єкт для видалення не знайдено.");
        }
    }

    public void Save(string FilePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(_elements.Values.ToList(), options);
        File.WriteAllText(FilePath, json);
    }

    public void Load(string FilePath)
    {
        string json = File.ReadAllText(FilePath);
        List<T> elements = JsonSerializer.Deserialize<List<T>>(json)
            ?? throw new InvalidDataException("Файл не містить колекції об'єктів.");

        var loadedElements = new Dictionary<Guid, T>();

        foreach (T element in elements)
        {
            if (element is null || !loadedElements.TryAdd(element.Id, element))
            {
                throw new InvalidDataException("Файл містить null або повторювані Id.");
            }
        }

        _elements = loadedElements;
    }
}
