using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupProject325.Authorization;
using GroupProject325.Models;
using GroupProject325.Data;

namespace GroupProject325.Controllers;

//items are listed on ledger page, no need for index
//every action finishes by redirecting to ledger page
// The permission checks are commented out until MinorItem.CampaignId is an int linked
// to Campaign (see docs/ERD.md).
public class MinorItemController : Controller
{
    private readonly LedgerContext _context;
    private readonly IAuthorizationService _authorization;

    public MinorItemController(LedgerContext context, IAuthorizationService authorization)
    {
        _context = context;
        _authorization = authorization;
    }
    //GET: MinorItems/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // if (!await HasPermissionAsync(campaignId, CampaignPermission.ViewPartyInventory)) return Forbid();
        var minorItems = await _context.MinorItem.ToListAsync();
        return View(minorItems);
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
        // if (!await HasPermissionAsync(minorItem.CampaignId, CampaignPermission.ViewPartyInventory)) return Forbid();

        return View(minorItem);
    }
    // GET: MinorItems/Create
    [HttpGet]
    public IActionResult Create()
    {
        // if (!await HasPermissionAsync(campaignId, CampaignPermission.AddItems)) return Forbid();
        return View();
    }

    // POST: MinorItems/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CharacterId,ResourceType,Quantity,TransactionType,Description")] MinorItem minorItem)
    {
        // if (!await HasPermissionAsync(campaignId, CampaignPermission.AddItems)) return Forbid();
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
        // if (!await HasPermissionAsync(minorItem.CampaignId, CampaignPermission.EditItems)) return Forbid();
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
            // if (!await HasPermissionAsync(existingItem.CampaignId, CampaignPermission.EditItems)) return Forbid();

            existingItem.ResourceType = minorItem.ResourceType;
            existingItem.Quantity = minorItem.Quantity;
            existingItem.TransactionType = minorItem.TransactionType;
            existingItem.Description = minorItem.Description;

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
        // if (!await HasPermissionAsync(minorItem.CampaignId, CampaignPermission.DeleteItems)) return Forbid();

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
            // if (!await HasPermissionAsync(minorItem.CampaignId, CampaignPermission.DeleteItems)) return Forbid();
            _context.MinorItem.Remove(minorItem);
            await _context.SaveChangesAsync();
        }

        return BackToLedger();
    }

    private IActionResult BackToLedger()
    {
        return RedirectToAction("Index", "Home");
    }

    private async Task<bool> HasPermissionAsync(int campaignId, CampaignPermission permission)
    {
        var campaign = await _context.Campaigns.FindAsync(campaignId);
        if (campaign == null)
        {
            return false;
        }
        var result = await _authorization.AuthorizeAsync(User, campaign, new CampaignPermissionRequirement(permission));
        return result.Succeeded;
    }
}


