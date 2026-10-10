using Library.Common.Models;

namespace Library.Common.Extensions;

public static class BookExtensions
{
    // Метод розширення
    public static string ToDisplayString(this Book book)
    {
        string description = $"«{book.Title}», {book.Author}";

        if (book is PrintedBook printedBook)
        {
            return $"{description}; {printedBook.PageCount} с.; полиця {printedBook.ShelfNumber}";
        }

        if (book is EBook eBook)
        {
            return $"{description}; {eBook.FileFormat}; {eBook.FileSizeMb:F1} МБ";
        }

        return description;
    }
}
