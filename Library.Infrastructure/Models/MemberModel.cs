using System.ComponentModel.DataAnnotations;

namespace Library.Infrastructure.Models;

public class MemberModel : IEntityModel
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;
    public MemberCardModel? Card { get; set; }
    public ICollection<LoanModel> Loans { get; set; } = new List<LoanModel>();
}
