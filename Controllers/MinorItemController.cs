using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTL.Models;

namespace TTL.Controllers;

[Route("minoritems")]
public class MinorItemController : Controller
{
    private readonly TTLContext _db;

    public MinorItemController(TTLContext db)
    {
        _db = db;
    }

    // GET: MinorItems
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _db.MinorItems.ToListAsync());
    }

    // GET: MinorItems/Details
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _db.MinorItems == null)
        {
            return NotFound();
        }

        var minorItem = await _db.MinorItems
            .FirstOrDefaultAsync(m => m.Id == id);
        if (minorItem == null)
        {
            return NotFound();
        }

        return View(minorItem);
    }
    // GET: MinorItems/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: MinorItems/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CharacterId,ResourceType,Amount,TransactionType,Description")] MinorItem minorItem)
    {
        if (ModelState.IsValid)
        {
            _db.Add(minorItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(minorItem);
    }

    // GET: MinorItems/Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _db.MinorItems == null)
        {
            return NotFound();
        }

        var minorItem = await _db.MinorItems.FindAsync(id);
        if (minorItem == null)
        {
            return NotFound();
        }
        return View(minorItem);
    }

    // POST: MinorItems/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ResourceType,Amount,TransactionType")] MinorItem minorItem)
    {
        if (id != minorItem.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingItem = await _db.MinorItems.FindAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.ResourceType = minorItem.ResourceType;
            existingItem.Amount = minorItem.Amount;
            existingItem.TransactionType = minorItem.TransactionType;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        return View(minorItem);
    }

    // GET: MinorItems/Delete
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _db.MinorItems == null)
        {
            return NotFound();
        }

        var minorItem = await _db.MinorItems
            .FirstOrDefaultAsync(m => m.Id == id);
        if (minorItem == null)
        {
            return NotFound();
        }

        return View(minorItem);
    }

    // POST: MinorItems/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)    
    {
        var minorItem = await _db.MinorItems.FindAsync(id);

        if (minorItem != null)
        {
            _db.MinorItems.Remove(minorItem);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}


