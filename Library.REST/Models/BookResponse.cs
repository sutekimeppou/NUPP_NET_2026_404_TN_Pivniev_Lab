using Library.Common.Models;

namespace Library.REST.Models;

public class BookResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public int? PageCount { get; set; }
    public string? Publisher { get; set; }
    public string? ShelfNumber { get; set; }
    public string? FileFormat { get; set; }
    public double? FileSizeMb { get; set; }
    public string? DownloadUrl { get; set; }

    public static BookResponse FromDomain(Book book)
    {
        return new BookResponse
        {
            Id = book.Id, Title = book.Title, Author = book.Author,
            Kind = book is PrintedBook ? "printed" : book is EBook ? "electronic" : "book",
            PageCount = (book as PrintedBook)?.PageCount,
            Publisher = (book as PrintedBook)?.Publisher,
            ShelfNumber = (book as PrintedBook)?.ShelfNumber,
            FileFormat = (book as EBook)?.FileFormat,
            FileSizeMb = (book as EBook)?.FileSizeMb,
            DownloadUrl = (book as EBook)?.DownloadUrl
        };
    }
}
