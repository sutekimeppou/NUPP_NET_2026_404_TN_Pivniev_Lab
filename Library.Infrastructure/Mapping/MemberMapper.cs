using Library.Common.Models;
using Library.Infrastructure.Models;

namespace Library.Infrastructure.Mapping;

public class MemberMapper : IEntityMapper<Member, MemberModel>
{
    public MemberModel ToModel(Member member)
    {
        return new MemberModel { PublicId = member.Id, Name = member.Name, Email = member.Email };
    }

    public Member ToDomain(MemberModel model)
    {
        return new Member(model.Name, model.Email) { Id = model.PublicId };
    }

    public void UpdateModel(Member member, MemberModel model)
    {
        model.Name = member.Name;
        model.Email = member.Email;
    }
}
