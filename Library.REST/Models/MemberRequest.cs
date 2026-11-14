using System.ComponentModel.DataAnnotations;

namespace Library.REST.Models;

public class MemberRequest
{
    [Required, StringLength(120, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;
}
