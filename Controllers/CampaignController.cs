using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupProject325.Data;
using GroupProject325.Models;

namespace GroupProject325.Controllers;

public class CampaignController : Controller
{
    private readonly LedgerContext _context;

    public CampaignController(LedgerContext context)
    {
        _context = context;
    }

    // GET: /Campaign or /Campaign/Index
    public async Task<IActionResult> Index()
    {
        var campaigns = await _context.Campaigns
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return View(campaigns);
    }

    // GET: /Campaign/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var campaign = await _context.Campaigns
            .FirstOrDefaultAsync(c => c.Id == id);
        if (campaign == null)
        {
            return NotFound();
        }

        return View(campaign);
    }

    // GET: /Campaign/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Campaign/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,GameMaster,System")] Campaign campaign)
    {
        if (ModelState.IsValid)
        {
            campaign.CreatedAt = DateTime.UtcNow;
            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(campaign);
    }

    // GET: /Campaign/Edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var campaign = await _context.Campaigns.FindAsync(id);
        if (campaign == null)
        {
            return NotFound();
        }

        return View(campaign);
    }

    // POST: /Campaign/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,GameMaster,System")] Campaign campaign)
    {
        if (id != campaign.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existing = await _context.Campaigns.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = campaign.Title;
            existing.Description = campaign.Description;
            existing.GameMaster = campaign.GameMaster;
            existing.System = campaign.System;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(campaign);
    }

    // GET: /Campaign/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var campaign = await _context.Campaigns
            .FirstOrDefaultAsync(c => c.Id == id);
        if (campaign == null)
        {
            return NotFound();
        }

        return View(campaign);
    }

    // POST: /Campaign/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var campaign = await _context.Campaigns.FindAsync(id);
        if (campaign != null)
        {
            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
