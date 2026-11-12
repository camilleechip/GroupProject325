using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupProject325.Models;
using GroupProject325.Data;

namespace GroupProject325.Controllers;

//items are listed on ledger page, no need for index
//every action finishes by redirecting to ledger page
public class MinorItemController : Controller
{
    private readonly LedgerContext _context;

    public MinorItemController(LedgerContext context)
    {
        _context = context;
    }

    // GET: MinorItems/Details
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.MinorItem == null)
        {
            return NotFound();
        }

        var minorItem = await _context.MinorItem
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
    public async Task<IActionResult> Create([Bind("CharacterId,ResourceType,Quantity,TransactionType,Description")] MinorItem minorItem)
    {
        if (ModelState.IsValid)
        {
            _context.MinorItem.Add(minorItem);
            await _context.SaveChangesAsync();
            return BackToLedger();
        }
        return View(minorItem);
    }

    // GET: MinorItems/Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.MinorItem == null)
        {
            return NotFound();
        }

        var minorItem = await _context.MinorItem.FindAsync(id);
        if (minorItem == null)
        {
            return NotFound();
        }
        return View(minorItem);
    }

    // POST: MinorItems/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ResourceType,Quantity,TransactionType,Description")] MinorItem minorItem)
    {
        if (id != minorItem.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingItem = await _context.MinorItem.FindAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.ResourceType = minorItem.ResourceType;
            existingItem.Quantity = minorItem.Quantity;
            existingItem.TransactionType = minorItem.TransactionType;

            await _context.SaveChangesAsync();

            return BackToLedger();
        }
        return View(minorItem);
    }

    // GET: MinorItems/Delete
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.MinorItem == null)
        {
            return NotFound();
        }

        var minorItem = await _context.MinorItem
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
        var minorItem = await _context.MinorItem.FindAsync(id);

        if (minorItem != null)
        {
            _context.MinorItem.Remove(minorItem);
            await _context.SaveChangesAsync();
        }

        return BackToLedger();
    }

    private IActionResult BackToLedger()
    {
        return RedirectToAction("Index", "Home");
    }
}


