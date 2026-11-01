namespace Library.Infrastructure.Models;

public interface IEntityModel
{
    int Id { get; set; }
    Guid PublicId { get; set; }
}
