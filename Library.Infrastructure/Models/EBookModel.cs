namespace Library.Infrastructure.Models;

public class EBookModel : BookModel
{
    public string FileFormat { get; set; } = string.Empty;
    public double FileSizeMb { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
}
