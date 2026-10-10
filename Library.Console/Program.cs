using System.Globalization;
using System.Text;
using Library.Common.Models;
using Library.Common.Services;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");

Console.WriteLine("Лабораторна робота №2. Асинхронність і потоки");

using var books = new CrudServiceAsync<Book>("library-data.json");

await Parallel.ForEachAsync(Enumerable.Range(0, 2000), async (index, _) =>
{
    Book book = index % 2 == 0 ? PrintedBook.CreateNew() : EBook.CreateNew();

    if (!await books.CreateAsync(book))
    {
        throw new InvalidOperationException("Повторюваний Id.");
    }
});

Book[] all = (await books.ReadAllAsync()).ToArray();
PrintedBook[] printed = all.OfType<PrintedBook>().ToArray();
EBook[] electronic = all.OfType<EBook>().ToArray();

Console.WriteLine($"\nParallel: створено {all.Length} книг, Id унікальні: {all.Select(b => b.Id).Distinct().Count()}");
Console.WriteLine($"Друковані: {printed.Length}; електронні: {electronic.Length}");
Console.WriteLine($"Сторінки: min={printed.Min(b => b.PageCount)}, max={printed.Max(b => b.PageCount)}, avg={printed.Average(b => b.PageCount):F2}");
Console.WriteLine($"Розмір МБ: min={electronic.Min(b => b.FileSizeMb):F2}, max={electronic.Max(b => b.FileSizeMb):F2}, avg={electronic.Average(b => b.FileSizeMb):F2}");
Console.WriteLine($"Статичний лічильник Book: {Book.GetCreatedCount()}");
Console.WriteLine($"IEnumerable: foreach бачить {books.Count()} книг");

Book[] firstPage = (await books.ReadAllAsync(1, 10)).ToArray();
Book[] secondPage = (await books.ReadAllAsync(2, 10)).ToArray();

Console.WriteLine($"\nПагінація: сторінка 1 — {firstPage.Length}, сторінка 2 — {secondPage.Length}");
Console.WriteLine($"Спільних Id на сторінках: {firstPage.Select(b => b.Id).Intersect(secondPage.Select(b => b.Id)).Count()}");

Book selected = firstPage[0];
Console.WriteLine($"ReadAsync: знайдено {((await books.ReadAsync(selected.Id)).Id == selected.Id)}");

var updated = new Book("Оновлена книга", selected.Author) { Id = selected.Id };
Console.WriteLine($"UpdateAsync: {await books.UpdateAsync(updated)}");

Console.WriteLine($"RemoveAsync: {await books.RemoveAsync(updated)}");

await Task.WhenAll(books.SaveAsync(), books.SaveAsync());
Console.WriteLine($"SaveAsync: JSON збережено, записів: {books.Count()}");

object gate = new();
int counter = 0;

Parallel.For(0, 1000, _ =>
{
    lock (gate)
    {
        counter++;
    }
});

Console.WriteLine($"\nlock: лічильник після 1000 збільшень = {counter}");

using var semaphore = new Semaphore(2, 2);
int active = 0;
int peak = 0;

await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => Task.Run(() =>
{
    semaphore.WaitOne();

    try
    {
        lock (gate)
        {
            active++;
            peak = Math.Max(peak, active);
        }

        Thread.Sleep(30);

        lock (gate)
        {
            active--;
        }
    }
    finally
    {
        semaphore.Release();
    }
})));

Console.WriteLine($"Semaphore: одночасних виконавців максимум {peak}, ліміт 2");

using var signal = new AutoResetEvent(false);
Task worker = Task.Run(() => signal.WaitOne());

signal.Set();
await worker;

Console.WriteLine("AutoResetEvent: очікування завершено після Set()");

Console.WriteLine("\nДемонстрацію завершено.");
