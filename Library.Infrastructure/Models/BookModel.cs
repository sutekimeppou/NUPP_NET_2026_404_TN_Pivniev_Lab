namespace Library.Infrastructure.Models;

public class BookModel : IEntityModel
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public ICollection<LoanModel> Loans { get; set; } = new List<LoanModel>();
    public ICollection<TagModel> Tags { get; set; } = new List<TagModel>();
}
