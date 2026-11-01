using Library.Common.Models;
using Library.Infrastructure;
using Library.Infrastructure.Mapping;
using Library.Infrastructure.Models;
using Library.Infrastructure.Repositories;
using Library.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Library.Tests;

public class DatabaseCrudServiceTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private LibraryContext _context = null!;
    private DatabaseCrudService<Book, BookModel> _books = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        _context = new LibraryContext(new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlite(_connection).Options);
        await _context.Database.MigrateAsync();
        _books = new DatabaseCrudService<Book, BookModel>(
            new Repository<BookModel>(_context), new BookMapper());
    }

    [Fact]
    public async Task CrudPersistsDerivedTypesAndKeepsBothIds()
    {
        PrintedBook book = PrintedBook.CreateNew();
        Assert.True(await _books.CreateAsync(book));
        Assert.False(await _books.CreateAsync(book));
        Assert.True(await _books.SaveAsync());
        _context.ChangeTracker.Clear();
        PrintedBook read = Assert.IsType<PrintedBook>(await _books.ReadAsync(book.Id));
        Assert.Equal(book.PageCount, read.PageCount);
        BookModel model = await _context.Books.SingleAsync();
        Assert.True(model.Id > 0);
        Assert.Equal(book.Id, model.PublicId);
        book.PageCount++;
        Assert.True(await _books.UpdateAsync(book));
        await _books.SaveAsync();
        Assert.Equal(book.PageCount, ((PrintedBook)await _books.ReadAsync(book.Id)).PageCount);
        Assert.True(await _books.RemoveAsync(book));
        await _books.SaveAsync();
        Assert.Empty(await _books.ReadAllAsync());
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _books.ReadAsync(book.Id));
    }

    [Fact]
    public async Task MissingUpdateAndDeleteReturnFalse()
    {
        PrintedBook book = PrintedBook.CreateNew();
        Assert.False(await _books.UpdateAsync(book));
        Assert.False(await _books.RemoveAsync(book));
    }

    [Fact]
    public async Task SqlPaginationDoesNotOverlap()
    {
        for (int i = 0; i < 5; i++) await _books.CreateAsync(PrintedBook.CreateNew());
        await _books.SaveAsync();
        Book[] first = (await _books.ReadAllAsync(1, 2)).ToArray();
        Book[] second = (await _books.ReadAllAsync(2, 2)).ToArray();
        Assert.Equal(2, first.Length);
        Assert.Empty(first.Select(x => x.Id).Intersect(second.Select(x => x.Id)));
        Assert.Single(await _books.ReadAllAsync(3, 2));
        Assert.Empty(await _books.ReadAllAsync(int.MaxValue, int.MaxValue));
    }

    [Fact]
    public async Task SeedContainsAllRequiredRelationships()
    {
        await LibrarySeed.InitializeAsync(_context);
        Assert.Equal(2, await _context.Books.CountAsync());
        Assert.Single(await _context.MemberCards.Include(x => x.Member).ToListAsync());
        Assert.Single(await _context.Loans.Include(x => x.Book).Include(x => x.Member).ToListAsync());
        TagModel tag = await _context.Tags.Include(x => x.Books).SingleAsync();
        Assert.Equal(2, tag.Books.Count);
        await LibrarySeed.InitializeAsync(_context);
        Assert.Equal(2, await _context.Books.CountAsync());
    }

    [Fact]
    public async Task UniqueEmailAndOneToOneCardAreEnforced()
    {
        await LibrarySeed.InitializeAsync(_context);
        _context.Members.Add(new MemberModel
        {
            PublicId = Guid.NewGuid(), Name = "Duplicate", Email = "olena@example.com"
        });
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
        _context.ChangeTracker.Clear();
        MemberModel member = await _context.Members.SingleAsync();
        _context.MemberCards.Add(new MemberCardModel { MemberId = member.Id, Number = "Second" });
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
