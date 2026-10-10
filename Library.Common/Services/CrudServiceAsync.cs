using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using Library.Common.Interfaces;

namespace Library.Common.Services;

public class CrudServiceAsync<T> : ICrudServiceAsync<T>, IDisposable where T : class, IEntity
{
    private readonly ConcurrentDictionary<Guid, T> _elements = new();
    private readonly SemaphoreSlim _saveGate = new(1, 1);
    public string FilePath { get; }

    public CrudServiceAsync(string FilePath)
    {
        this.FilePath = FilePath;
    }

    public Task<bool> CreateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return Task.FromResult(_elements.TryAdd(element.Id, element));
    }

    public Task<T> ReadAsync(Guid id)
    {
        return _elements.TryGetValue(id, out T? element)
            ? Task.FromResult(element)
            : Task.FromException<T>(new KeyNotFoundException("Об'єкт не знайдено."));
    }

    public Task<IEnumerable<T>> ReadAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(Snapshot());
    }

    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1);
        long offset = ((long)page - 1) * amount;
        IEnumerable<T> result = offset > int.MaxValue
            ? Array.Empty<T>()
            : Snapshot().Skip((int)offset).Take(amount).ToArray();
        return Task.FromResult(result);
    }

    public Task<bool> UpdateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        while (_elements.TryGetValue(element.Id, out T? previous))
        {
            if (_elements.TryUpdate(element.Id, element, previous))
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public Task<bool> RemoveAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return Task.FromResult(_elements.TryRemove(element.Id, out _));
    }

    public async Task<bool> SaveAsync()
    {
        await _saveGate.WaitAsync();
        try
        {
            await using FileStream file = File.Create(FilePath);
            await JsonSerializer.SerializeAsync(file, Snapshot(),
                new JsonSerializerOptions { WriteIndented = true });
            return true;
        }
        finally
        {
            _saveGate.Release();
        }
    }

    private T[] Snapshot()
    {
        return _elements.ToArray().Select(pair => pair.Value).OrderBy(x => x.Id).ToArray();
    }

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Snapshot()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void Dispose() => _saveGate.Dispose();
}
