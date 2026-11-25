using GroupProject325.Data;
using GroupProject325.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupProject325.Controllers;

// Every action requires a signed-in user.
[Authorize]  // Re-enable once authentication is set up.
public class CharactersController : LedgerControllerBase
{
    public CharactersController(LedgerContext context) : base(context) { }

    // GET: Characters
    // Lists only the signed-in user's own characters.
    public async Task<IActionResult> Index()
    {
        var current = await GetCurrentUserAsync();
        if (current == null) return RedirectToAction("Create", "Users");

        var characters = await _context.Characters
            .Where(c => c.UserId == current.Id)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(characters);
    }

    // GET: Characters/Details/5
    // Visible to the owner and to anyone who shares a campaign with the owner.
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var character = await _context.Characters
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (character == null) return NotFound();

        var current = await GetCurrentUserAsync();
        if (current == null) return Forbid();

        if (character.UserId != current.Id)
        {
            var shares = await _context.CampaignMembers.AnyAsync(m =>
                m.UserId == character.UserId &&
                _context.CampaignMembers.Any(c => c.UserId == current.Id && c.CampaignId == m.CampaignId));
            if (!shares) return Forbid();
        }

        return View(character);
    }

    // GET: Characters/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Characters/Create
    // Only Name is bound from the form. The owner is set here from the signed-in user.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Character character)
    {
        var current = await GetCurrentUserAsync();
        if (current == null) return Forbid();

        character.UserId = current.Id;
        ModelState.Remove("User");
        ModelState.Remove("UserId");

        if (ModelState.IsValid)
        {
            character.Name = character.Name.Trim();
            _context.Add(character);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(character);
    }

    // GET: Characters/Edit/5
    // A user can only edit their own characters.
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var character = await _context.Characters.FindAsync(id);
        if (character == null) return NotFound();

        var current = await GetCurrentUserAsync();
        if (current == null || character.UserId != current.Id) return Forbid();

        return View(character);
    }

    // POST: Characters/Edit/5
    // Only Id and Name are bound. The existing row is loaded and updated so UserId
    // can't be overwritten.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Character character)
    {
        if (id != character.Id) return NotFound();

        var existing = await _context.Characters.FindAsync(id);
        if (existing == null) return NotFound();

        var current = await GetCurrentUserAsync();
        if (current == null || existing.UserId != current.Id) return Forbid();

        ModelState.Remove("User");
        ModelState.Remove("UserId");

        if (ModelState.IsValid)
        {
            existing.Name = character.Name.Trim();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(character);
    }

    // GET: Characters/Delete/5
    // A user can only delete their own characters.
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var character = await _context.Characters.FindAsync(id);
        if (character == null) return NotFound();

        var current = await GetCurrentUserAsync();
        if (current == null || character.UserId != current.Id) return Forbid();

        return View(character);
    }

    // POST: Characters/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var character = await _context.Characters.FindAsync(id);
        if (character == null) return NotFound();

        var current = await GetCurrentUserAsync();
        if (current == null || character.UserId != current.Id) return Forbid();

        _context.Characters.Remove(character);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}