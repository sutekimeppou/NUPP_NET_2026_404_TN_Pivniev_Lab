using System.ComponentModel.DataAnnotations;
using Library.Common.Models;

namespace Library.REST.Models;

public class BookRequest
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
    [Required, StringLength(120, MinimumLength = 1)]
    public string Author { get; set; } = string.Empty;
    [Required, RegularExpression("^(printed|electronic)$")]
    public string Kind { get; set; } = "printed";
    [Range(1, 100000)]
    public int PageCount { get; set; } = 1;
    [StringLength(120)]
    public string Publisher { get; set; } = string.Empty;
    [StringLength(30)]
    public string ShelfNumber { get; set; } = string.Empty;
    [StringLength(20)]
    public string FileFormat { get; set; } = "EPUB";
    [Range(0.01, 100000)]
    public double FileSizeMb { get; set; } = 1;
    [Url]
    public string? DownloadUrl { get; set; }

    public Book ToDomain()
    {
        return Kind == "printed"
            ? new PrintedBook(Title, Author, PageCount, Publisher, ShelfNumber)
            : new EBook(Title, Author, FileFormat, FileSizeMb, DownloadUrl ?? string.Empty);
    }
}
