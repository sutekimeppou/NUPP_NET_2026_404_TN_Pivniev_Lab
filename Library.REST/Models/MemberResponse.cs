using Library.Common.Models;

namespace Library.REST.Models;

public record MemberResponse(Guid Id, string Name, string Email)
{
    public static MemberResponse FromDomain(Member member) => new(member.Id, member.Name, member.Email);
}
