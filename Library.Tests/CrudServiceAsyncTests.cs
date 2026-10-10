using System.Text.Json;
using Library.Common.Models;
using Library.Common.Services;

namespace Library.Tests;

public class CrudServiceAsyncTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"library-{Guid.NewGuid()}.json");
    private readonly CrudServiceAsync<Book> _service;

    public CrudServiceAsyncTests()
    {
        _service = new CrudServiceAsync<Book>(_path);
    }

    [Fact]
    public async Task CreateRejectsDuplicateAndReadFindsObject()
    {
        Book book = Book.CreateNew();
        Assert.True(await _service.CreateAsync(book));
        Assert.False(await _service.CreateAsync(book));
        Assert.Same(book, await _service.ReadAsync(book.Id));
    }

    [Fact]
    public async Task MissingRecordsHaveExpectedResults()
    {
        Book book = Book.CreateNew();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ReadAsync(book.Id));
        Assert.False(await _service.UpdateAsync(book));
        Assert.False(await _service.RemoveAsync(book));
    }

    [Fact]
    public async Task UpdateReplacesAndRemoveDeletesObject()
    {
        Book book = Book.CreateNew();
        await _service.CreateAsync(book);
        var replacement = new Book("Нова назва", "Автор") { Id = book.Id };
        Assert.True(await _service.UpdateAsync(replacement));
        Assert.Same(replacement, await _service.ReadAsync(book.Id));
        Assert.True(await _service.RemoveAsync(replacement));
        Assert.Empty(await _service.ReadAllAsync());
    }

    [Fact]
    public async Task ParallelCreationKeepsAllUniqueRecords()
    {
        await Parallel.ForEachAsync(Enumerable.Range(0, 2000), async (_, _) =>
        {
            Assert.True(await _service.CreateAsync(PrintedBook.CreateNew()));
        });
        Book[] books = (await _service.ReadAllAsync()).ToArray();
        Assert.Equal(2000, books.Length);
        Assert.Equal(2000, books.Select(b => b.Id).Distinct().Count());
    }

    [Fact]
    public async Task CompetingCreatesAcceptExactlyOneRecord()
    {
        Book book = Book.CreateNew();
        bool[] results = await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => _service.CreateAsync(book)));
        Assert.Single(results, x => x);
        Assert.Single(await _service.ReadAllAsync());
    }

    [Fact]
    public async Task PagesAreOrderedAndDoNotOverlap()
    {
        for (int i = 0; i < 25; i++) await _service.CreateAsync(Book.CreateNew());
        Book[] all = (await _service.ReadAllAsync()).ToArray();
        Assert.Equal(all.Take(10), await _service.ReadAllAsync(1, 10));
        Assert.Equal(all.Skip(10).Take(10), await _service.ReadAllAsync(2, 10));
        Assert.Equal(5, (await _service.ReadAllAsync(3, 10)).Count());
        Assert.Empty(await _service.ReadAllAsync(4, 10));
        Assert.Empty(await _service.ReadAllAsync(int.MaxValue, int.MaxValue));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(-1, 10)]
    public async Task InvalidPaginationIsRejected(int page, int amount)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.ReadAllAsync(page, amount));
    }

    [Fact]
    public async Task NullElementsAreRejected()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null!));
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemoveAsync(null!));
    }

    [Fact]
    public async Task EnumerationUsesAStableSnapshot()
    {
        Book book = Book.CreateNew();
        await _service.CreateAsync(book);
        using IEnumerator<Book> iterator = _service.GetEnumerator();
        await _service.CreateAsync(Book.CreateNew());
        Assert.True(iterator.MoveNext());
        Assert.Same(book, iterator.Current);
        Assert.False(iterator.MoveNext());
        Assert.Equal(2, _service.Count());
    }

    [Fact]
    public async Task ConcurrentSavesProduceValidPolymorphicJson()
    {
        await _service.CreateAsync(PrintedBook.CreateNew());
        await _service.CreateAsync(EBook.CreateNew());
        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => _service.SaveAsync()));
        await using FileStream file = File.OpenRead(_path);
        Book[] books = (await JsonSerializer.DeserializeAsync<Book[]>(file))!;
        Assert.Equal(2, books.Length);
        Assert.Single(books.OfType<PrintedBook>());
        Assert.Single(books.OfType<EBook>());
    }

    [Fact]
    public async Task SavePropagatesFileErrorsAndReleasesGate()
    {
        using var invalid = new CrudServiceAsync<Book>(
            Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "books.json"));
        await Assert.ThrowsAnyAsync<IOException>(() => invalid.SaveAsync());
        await Assert.ThrowsAnyAsync<IOException>(() => invalid.SaveAsync());
    }

    [Fact]
    public async Task SameServiceSupportsMembers()
    {
        using var members = new CrudServiceAsync<Member>(_path);
        Member member = Member.CreateNew();
        Assert.True(await members.CreateAsync(member));
        Assert.Same(member, await members.ReadAsync(member.Id));
        Assert.True(await members.SaveAsync());
    }

    public void Dispose()
    {
        _service.Dispose();
        File.Delete(_path);
    }
}
