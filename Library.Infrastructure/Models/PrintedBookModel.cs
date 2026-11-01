namespace Library.Infrastructure.Models;

public class PrintedBookModel : BookModel
{
    public int PageCount { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public string ShelfNumber { get; set; } = string.Empty;
}
