using Library.Common.Interfaces;
using Library.Common.Models;
using Library.REST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.REST.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController(ICrudServiceAsync<Member> members) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<MemberResponse>>> GetAll(int page = 1, int amount = 10)
    {
        if (page < 1 || amount < 1 || amount > 100) return BadRequest();
        return Ok((await members.ReadAllAsync(page, amount)).Select(MemberResponse.FromDomain));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberResponse>> GetById(Guid id)
    {
        try { return Ok(MemberResponse.FromDomain(await members.ReadAsync(id))); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MemberResponse>> Create(MemberRequest request)
    {
        var member = new Member(request.Name, request.Email);
        await members.CreateAsync(member);
        try { await members.SaveAsync(); }
        catch (DbUpdateException) { return Conflict("Email уже використовується."); }
        return CreatedAtAction(nameof(GetById), new { id = member.Id }, MemberResponse.FromDomain(member));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, MemberRequest request)
    {
        var member = new Member(request.Name, request.Email) { Id = id };
        if (!await members.UpdateAsync(member)) return NotFound();
        try { await members.SaveAsync(); }
        catch (DbUpdateException) { return Conflict("Email уже використовується."); }
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await members.RemoveAsync(new Member { Id = id })) return NotFound();
        await members.SaveAsync();
        return NoContent();
    }
}
