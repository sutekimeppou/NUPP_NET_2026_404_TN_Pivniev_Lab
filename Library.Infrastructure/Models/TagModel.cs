namespace Library.Infrastructure.Models;

public class TagModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<BookModel> Books { get; set; } = new List<BookModel>();
}
