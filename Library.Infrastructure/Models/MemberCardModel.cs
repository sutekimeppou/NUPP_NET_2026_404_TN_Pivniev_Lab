namespace Library.Infrastructure.Models;

public class MemberCardModel
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public MemberModel Member { get; set; } = null!;
}
