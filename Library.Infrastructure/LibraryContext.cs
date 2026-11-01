using Library.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure;

public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public DbSet<BookModel> Books => Set<BookModel>();
    public DbSet<MemberModel> Members => Set<MemberModel>();
    public DbSet<MemberCardModel> MemberCards => Set<MemberCardModel>();
    public DbSet<LoanModel> Loans => Set<LoanModel>();
    public DbSet<TagModel> Tags => Set<TagModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BookModel>().UseTptMappingStrategy().ToTable("Books");
        modelBuilder.Entity<PrintedBookModel>().ToTable("PrintedBooks");
        modelBuilder.Entity<EBookModel>().ToTable("EBooks");
        modelBuilder.Entity<MemberModel>().ToTable("Members");
        modelBuilder.Entity<MemberCardModel>().ToTable("MemberCards");
        modelBuilder.Entity<LoanModel>().ToTable("Loans");
        modelBuilder.Entity<TagModel>().ToTable("Tags");

        modelBuilder.Entity<BookModel>().HasIndex(x => x.PublicId).IsUnique();
        modelBuilder.Entity<MemberModel>().HasIndex(x => x.PublicId).IsUnique();
        modelBuilder.Entity<MemberModel>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<BookModel>().Property(x => x.Title).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<BookModel>().Property(x => x.Author).IsRequired().HasMaxLength(120);
        modelBuilder.Entity<MemberModel>().Property(x => x.Name).IsRequired().HasMaxLength(120);
        modelBuilder.Entity<MemberModel>().Property(x => x.Email).IsRequired().HasMaxLength(200);

        modelBuilder.Entity<MemberModel>()
            .HasOne(x => x.Card).WithOne(x => x.Member)
            .HasForeignKey<MemberCardModel>(x => x.MemberId);
        modelBuilder.Entity<LoanModel>()
            .HasOne(x => x.Member).WithMany(x => x.Loans).HasForeignKey(x => x.MemberId);
        modelBuilder.Entity<LoanModel>()
            .HasOne(x => x.Book).WithMany(x => x.Loans).HasForeignKey(x => x.BookId);
        modelBuilder.Entity<BookModel>()
            .HasMany(x => x.Tags).WithMany(x => x.Books)
            .UsingEntity<Dictionary<string, object>>(
                "BookTags",
                right => right.HasOne<TagModel>().WithMany().HasForeignKey("TagId"),
                left => left.HasOne<BookModel>().WithMany().HasForeignKey("BookId"));
    }
}
