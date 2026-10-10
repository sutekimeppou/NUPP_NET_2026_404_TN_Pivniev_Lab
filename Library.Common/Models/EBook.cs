namespace Library.Common.Models;

public class EBook : Book
{
    public string FileFormat { get; set; }
    public double FileSizeMb { get; set; }
    public string DownloadUrl { get; set; }

    public EBook() : this(string.Empty, string.Empty, string.Empty, 0, string.Empty)
    {
    }

    public EBook(string title, string author, string fileFormat,
        double fileSizeMb, string downloadUrl) : base(title, author)
    {
        FileFormat = fileFormat;
        FileSizeMb = fileSizeMb;
        DownloadUrl = downloadUrl;
    }

    public new static EBook CreateNew()
    {
        return new EBook($"Електронна книга {Guid.NewGuid():N}", "Автор", "EPUB",
            Math.Round(Random.Shared.NextDouble() * 9 + 1, 2), "https://example.com/book.epub");
    }
}
