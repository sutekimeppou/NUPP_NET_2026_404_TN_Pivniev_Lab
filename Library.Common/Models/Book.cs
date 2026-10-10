using System.Text.Json.Serialization;
using Library.Common.Interfaces;

namespace Library.Common.Models;

// Делегат
public delegate void BookTitleChangedHandler(Book book, string previousTitle);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(PrintedBook), "printed")]
[JsonDerivedType(typeof(EBook), "electronic")]
public class Book : IEntity
{
    // Статичне поле
    private static int _createdCount;

    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }

    // Подія
    public event BookTitleChangedHandler? TitleChanged;

    // Статичний конструктор
    static Book()
    {
        _createdCount = 0;
    }

    public Book() : this(string.Empty, string.Empty)
    {
    }

    // Конструктор
    public Book(string title, string author)
    {
        Id = Guid.NewGuid();
        Title = title;
        Author = author;
        Interlocked.Increment(ref _createdCount);
    }

    // Метод
    public void ChangeTitle(string newTitle)
    {
        string previousTitle = Title;
        Title = newTitle;
        TitleChanged?.Invoke(this, previousTitle);
    }

    // Статичний метод
    public static int GetCreatedCount()
    {
        return Volatile.Read(ref _createdCount);
    }

    public static Book CreateNew()
    {
        return new Book($"Книга {Guid.NewGuid():N}", "Невідомий автор");
    }
}
