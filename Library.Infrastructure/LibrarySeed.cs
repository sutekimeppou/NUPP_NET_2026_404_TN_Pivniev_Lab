using Library.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure;

public static class LibrarySeed
{
    public static async Task InitializeAsync(LibraryContext context)
    {
        if (await context.Books.AnyAsync() || await context.Members.AnyAsync()) return;
        var printed = new PrintedBookModel
        {
            PublicId = Guid.NewGuid(), Title = "Кобзар", Author = "Тарас Шевченко",
            PageCount = 320, Publisher = "Дніпро", ShelfNumber = "A-01"
        };
        var electronic = new EBookModel
        {
            PublicId = Guid.NewGuid(), Title = "Лісова пісня", Author = "Леся Українка",
            FileFormat = "EPUB", FileSizeMb = 1.5, DownloadUrl = "https://example.com/book.epub"
        };
        var literature = new TagModel { Name = "Українська література" };
        printed.Tags.Add(literature);
        electronic.Tags.Add(literature);
        var member = new MemberModel
        {
            PublicId = Guid.NewGuid(), Name = "Олена Коваль", Email = "olena@example.com",
            Card = new MemberCardModel { Number = "CARD-001", IssuedAt = DateTime.UtcNow }
        };
        context.AddRange(printed, electronic, member);
        context.Loans.Add(new LoanModel
        {
            Book = printed, Member = member, BorrowedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}
