using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Library.Infrastructure;

public static class LibraryDatabase
{
    public static string GetPath()
    {
        DirectoryInfo? directory = new(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Library.sln")))
            {
                return Path.Combine(directory.FullName, "library.db");
            }
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Запустіть застосунок із папки рішення Library.");
    }
}

public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
{
    public LibraryContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlite($"Data Source={LibraryDatabase.GetPath()}").Options;
        return new LibraryContext(options);
    }
}
