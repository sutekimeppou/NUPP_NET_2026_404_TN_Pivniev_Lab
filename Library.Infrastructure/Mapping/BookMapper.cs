using Library.Common.Models;
using Library.Infrastructure.Models;

namespace Library.Infrastructure.Mapping;

public class BookMapper : IEntityMapper<Book, BookModel>
{
    public BookModel ToModel(Book book)
    {
        BookModel model = book switch
        {
            PrintedBook => new PrintedBookModel(),
            EBook => new EBookModel(),
            _ => new BookModel()
        };
        model.PublicId = book.Id;
        UpdateModel(book, model);
        return model;
    }

    public Book ToDomain(BookModel model)
    {
        Book book = model switch
        {
            PrintedBookModel p => new PrintedBook(p.Title, p.Author, p.PageCount, p.Publisher, p.ShelfNumber),
            EBookModel e => new EBook(e.Title, e.Author, e.FileFormat, e.FileSizeMb, e.DownloadUrl),
            _ => new Book(model.Title, model.Author)
        };
        book.Id = model.PublicId;
        return book;
    }

    public void UpdateModel(Book book, BookModel model)
    {
        if ((book is PrintedBook) != (model is PrintedBookModel)
            || (book is EBook) != (model is EBookModel))
        {
            throw new ArgumentException("Тип книги при оновленні має залишатися незмінним.");
        }
        model.Title = book.Title;
        model.Author = book.Author;
        if (book is PrintedBook p && model is PrintedBookModel pm)
        {
            pm.PageCount = p.PageCount;
            pm.Publisher = p.Publisher;
            pm.ShelfNumber = p.ShelfNumber;
        }
        if (book is EBook e && model is EBookModel em)
        {
            em.FileFormat = e.FileFormat;
            em.FileSizeMb = e.FileSizeMb;
            em.DownloadUrl = e.DownloadUrl;
        }
    }
}
