namespace Library.Common.Models;

public class PrintedBook : Book
{
    public int PageCount { get; set; }
    public string Publisher { get; set; }
    public string ShelfNumber { get; set; }

    public PrintedBook() : this(string.Empty, string.Empty, 0, string.Empty, string.Empty)
    {
    }

    public PrintedBook(string title, string author, int pageCount,
        string publisher, string shelfNumber) : base(title, author)
    {
        PageCount = pageCount;
        Publisher = publisher;
        ShelfNumber = shelfNumber;
    }
}
