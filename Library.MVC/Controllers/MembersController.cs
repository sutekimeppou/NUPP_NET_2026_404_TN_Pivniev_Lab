using Library.Infrastructure;
using Library.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers;

public class MembersController(LibraryContext context) : Controller
{
    public async Task<IActionResult> Index() => View(await context.Members.ToListAsync());
    public IActionResult Create() => View();

    public async Task<IActionResult> Details(int id)
    {
        MemberModel? member = await context.Members.FindAsync(id);
        return member is null ? NotFound() : View(member);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email")] MemberModel member)
    {
        if (!ModelState.IsValid) return View(member);
        member.PublicId = Guid.NewGuid();
        context.Members.Add(member);
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Email", "Email уже використовується.");
            return View(member);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        MemberModel? member = await context.Members.FindAsync(id);
        return member is null ? NotFound() : View(member);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email")] MemberModel form)
    {
        if (id != form.Id) return BadRequest();
        if (!ModelState.IsValid) return View(form);
        MemberModel? member = await context.Members.FindAsync(id);
        if (member is null) return NotFound();
        member.Name = form.Name;
        member.Email = form.Email;
        try { await context.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Email", "Email уже використовується.");
            return View(form);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        MemberModel? member = await context.Members.FindAsync(id);
        return member is null ? NotFound() : View(member);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        MemberModel? member = await context.Members.FindAsync(id);
        if (member is null) return NotFound();
        context.Members.Remove(member);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
