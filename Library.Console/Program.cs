using System.Globalization;
using System.Text;
using Library.Common.Extensions;
using Library.Common.Models;
using Library.Common.Services;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");

Console.WriteLine("Лабораторна робота №1. Бібліотека");

var books = new CrudService<Book>();
var printedBook = new PrintedBook("Кобзар", "Тарас Шевченко", 320, "Дніпро", "A-01");
var eBook = new EBook("Лісова пісня", "Леся Українка", "EPUB", 1.5,
    "https://example.com/books/lisova-pisnia.epub");

Console.WriteLine("\n1. CREATE: додаємо друковану та електронну книги.");
books.Create(printedBook);
books.Create(eBook);
Console.WriteLine($"Створено об'єктів Book: {Book.GetCreatedCount()}");

Console.WriteLine("\n2. READ ALL: усі книги (метод розширення).");
foreach (Book book in books.ReadAll())
{
    Console.WriteLine(book.ToDisplayString());
}

Console.WriteLine("\n3. READ: пошук друкованої книги за Guid.");
Console.WriteLine($"Id: {printedBook.Id}");
Console.WriteLine(books.Read(printedBook.Id).ToDisplayString());

Console.WriteLine("\n4. UPDATE: замінюємо запис, зберігаючи його Id.");
var updatedBook = new PrintedBook(printedBook.Title, printedBook.Author,
    352, printedBook.Publisher, "B-02")
{
    Id = printedBook.Id
};
updatedBook.TitleChanged += (book, previousTitle) =>
    Console.WriteLine($"Подія TitleChanged: «{previousTitle}» → «{book.Title}»");
updatedBook.ChangeTitle("Кобзар. Повне видання");
books.Update(updatedBook);
Console.WriteLine(books.Read(updatedBook.Id).ToDisplayString());

Console.WriteLine("\n5. SAVE / LOAD: зберігаємо обидва типи книг у JSON.");
string filePath = Path.Combine(Environment.CurrentDirectory, "library-data.json");
books.Save(filePath);
var loadedBooks = new CrudService<Book>();
loadedBooks.Load(filePath);
foreach (Book book in loadedBooks.ReadAll())
{
    Console.WriteLine($"{book.GetType().Name}: {book.ToDisplayString()}");
}
Console.WriteLine("Id та властивості збережено; похідні типи відновлено.");

Console.WriteLine("\n6. REMOVE: видаляємо електронну книгу.");
books.Remove(eBook);
Console.WriteLine($"Залишилося книг: {books.ReadAll().Count()}");
Console.WriteLine(books.Read(updatedBook.Id).ToDisplayString());

Console.WriteLine("\n7. GENERIC: той самий CRUD для читачів.");
var members = new CrudService<Member>();
var member = new Member("Олена Коваль", "olena@example.com");
members.Create(member);
Member foundMember = members.Read(member.Id);
Console.WriteLine($"Читач: {foundMember.Name}; {foundMember.Email}");

Console.WriteLine("\nДемонстрацію завершено.");
