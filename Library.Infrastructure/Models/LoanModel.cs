namespace Library.Infrastructure.Models;

public class LoanModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime BorrowedAt { get; set; }
    public BookModel Book { get; set; } = null!;
    public MemberModel Member { get; set; } = null!;
}
