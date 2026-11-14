using Library.Common.Interfaces;
using Library.Common.Models;
using Library.REST.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.REST.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController(ICrudServiceAsync<Book> books) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetAll(int page = 1, int amount = 10)
    {
        if (page < 1 || amount < 1 || amount > 100) return BadRequest();
        return Ok((await books.ReadAllAsync(page, amount)).Select(BookResponse.FromDomain));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponse>> GetById(Guid id)
    {
        try { return Ok(BookResponse.FromDomain(await books.ReadAsync(id))); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookResponse>> Create(BookRequest request)
    {
        Book book = request.ToDomain();
        await books.CreateAsync(book);
        await books.SaveAsync();
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, BookResponse.FromDomain(book));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, BookRequest request)
    {
        Book book = request.ToDomain();
        book.Id = id;
        try
        {
            if (!await books.UpdateAsync(book)) return NotFound();
        }
        catch (ArgumentException exception) { return BadRequest(exception.Message); }
        await books.SaveAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await books.RemoveAsync(new Book { Id = id })) return NotFound();
        await books.SaveAsync();
        return NoContent();
    }
}
