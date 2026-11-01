using System.Text;
using Library.Common.Extensions;
using Library.Common.Models;
using Library.Infrastructure;
using Library.Infrastructure.Mapping;
using Library.Infrastructure.Models;
using Library.Infrastructure.Repositories;
using Library.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Лабораторна робота №3. Entity Framework і SQLite");

var options = new DbContextOptionsBuilder<LibraryContext>()
    .UseSqlite($"Data Source={LibraryDatabase.GetPath()}")
    .Options;

await using var context = new LibraryContext(options);

await context.Database.MigrateAsync();
await LibrarySeed.InitializeAsync(context);

var repository = new Repository<BookModel>(context);
var books = new DatabaseCrudService<Book, BookModel>(repository, new BookMapper());
var members = new DatabaseCrudService<Member, MemberModel>(
    new Repository<MemberModel>(context), new MemberMapper());

Console.WriteLine("\nREAD ALL: книги з бази даних");

foreach (Book book in await books.ReadAllAsync())
    Console.WriteLine(book.ToDisplayString());

BookModel first = (await repository.GetAllAsync()).First();

Console.WriteLine($"\nДва ідентифікатори: SQL Id={first.Id}, PublicId={first.PublicId}");
Console.WriteLine($"Repository.GetByIdAsync(int): {(await repository.GetByIdAsync(first.Id)).Title}");
Console.WriteLine($"CRUD.ReadAsync(Guid): {(await books.ReadAsync(first.PublicId)).Title}");
Console.WriteLine($"Пагінація: сторінка 1 містить {(await books.ReadAllAsync(1, 1)).Count()} запис");

var demo = new PrintedBook("Тестова книга", "Автор", 200, "Дніпро", "T-01");

Console.WriteLine($"\nCREATE: {await books.CreateAsync(demo)}");
await books.SaveAsync();

demo.PageCount = 240;
Console.WriteLine($"UPDATE: {await books.UpdateAsync(demo)}");
await books.SaveAsync();
Console.WriteLine($"Після оновлення: {((PrintedBook)await books.ReadAsync(demo.Id)).PageCount} сторінок");

Console.WriteLine($"REMOVE: {await books.RemoveAsync(demo)}");
await books.SaveAsync();

Console.WriteLine($"Книг після видалення: {(await books.ReadAllAsync()).Count()}");
Console.WriteLine($"Читачів: {(await members.ReadAllAsync()).Count()}");

Console.WriteLine($"\n1:1 — читацьких карток: {await context.MemberCards.CountAsync()}");
Console.WriteLine($"1:N — видач книг: {await context.Loans.CountAsync()}");

int bookTags = await context.Set<Dictionary<string, object>>("BookTags").CountAsync();

Console.WriteLine($"M:N — зв'язків книга/тег: {bookTags}");
Console.WriteLine("TPT — Books, PrintedBooks, EBooks");
Console.WriteLine("База даних: library.db");

Console.WriteLine("\nДемонстрацію завершено.");
