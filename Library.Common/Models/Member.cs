using Library.Common.Interfaces;

namespace Library.Common.Models;

public class Member : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public Member() : this(string.Empty, string.Empty)
    {
    }

    public Member(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }

    public static Member CreateNew()
    {
        string number = Guid.NewGuid().ToString("N");
        return new Member($"Читач {number}", $"{number}@example.com");
    }
}
