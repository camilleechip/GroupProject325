using GroupProject325.Authorization;
using GroupProject325.Data;
using GroupProject325.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupProject325.Controllers;

// Manages custom property definitions (like "Rarity") and their options
// (like "Common", "Rare"). Setting a property's value on an item lives in
// ItemController.SetProperty.
// The permission checks are commented out until properties belong to a campaign (see docs/ERD.md).
public class CustomPropertyController : Controller
{
    private readonly LedgerContext _context;
    private readonly IAuthorizationService _authorization;

    public CustomPropertyController(LedgerContext context, IAuthorizationService authorization)
    {
        _context = context;
        _authorization = authorization;
    }

    // GET: /CustomProperty/Create
    public IActionResult Create()
    {
        // if (!await HasPermissionAsync(campaignId, CampaignPermission.ManageProperties)) return Forbid();
        return View();
    }

    // POST: /CustomProperty/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,ValueType,AllowMultiple")] CustomProperty property)
    {
        // if (!await HasPermissionAsync(campaignId, CampaignPermission.ManageProperties)) return Forbid();
        // TODO: scope names to a party once parties exist, so groups don't clash
        if (await NameTaken(property.Name, null))
        {
            ModelState.AddModelError(nameof(property.Name), $"A property named {property.Name} already exists.");
        }
        if (!ModelState.IsValid) return View(property);

        // AllowMultiple only means something for List properties.
        if (property.ValueType != PropertyValueType.List) property.AllowMultiple = false;

        _context.CustomProperties.Add(property);
        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // GET: /CustomProperty/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var property = await _context.CustomProperties
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (property == null) return NotFound();
        // if (!await HasPermissionAsync(property.CampaignId, CampaignPermission.ManageProperties)) return Forbid();
        return View(property);
    }

    // POST: /CustomProperty/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,ValueType,AllowMultiple")] CustomProperty form)
    {
        var property = await _context.CustomProperties.FindAsync(id);
        if (property == null) return NotFound();
        // if (!await HasPermissionAsync(property.CampaignId, CampaignPermission.ManageProperties)) return Forbid();

        if (await NameTaken(form.Name, id))
        {
            ModelState.AddModelError(nameof(form.Name), $"A property named {form.Name} already exists.");
        }

        if (form.ValueType != PropertyValueType.List) form.AllowMultiple = false;

        // Changes that would make existing item values invalid are blocked while items use the property.
        bool inUse = await _context.ItemPropertyValues.AnyAsync(v => v.CustomPropertyId == id);
        if (inUse && form.ValueType != property.ValueType)
        {
            ModelState.AddModelError(nameof(form.ValueType), "The type can't change while items use this property.");
        }
        if (inUse && property.AllowMultiple && !form.AllowMultiple && await HasItemWithMultipleOptions(id))
        {
            ModelState.AddModelError(nameof(form.AllowMultiple), "Some items have more than one option set. Fix those first.");
        }

        if (!ModelState.IsValid) return View(form);

        property.Name = form.Name;
        property.Description = form.Description;
        property.ValueType = form.ValueType;
        property.AllowMultiple = form.AllowMultiple;

        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // POST: /CustomProperty/Delete/{id}
    // Blocked while any item uses the property. The Restrict rule in LedgerContext
    // is the safety net; this check gives the user a readable message instead.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var property = await _context.CustomProperties.FindAsync(id);
        if (property == null) return NotFound();
        // if (!await HasPermissionAsync(property.CampaignId, CampaignPermission.ManageProperties)) return Forbid();

        int usedBy = await _context.ItemPropertyValues
            .Where(v => v.CustomPropertyId == id)
            .Select(v => v.ItemId)
            .Distinct()
            .CountAsync();
        if (usedBy > 0)
        {
            return LedgerError($"{property.Name} is used by {usedBy} item(s) and can't be deleted.");
        }

        // The property's options are cascade-deleted with it.
        _context.CustomProperties.Remove(property);
        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // POST: /CustomProperty/AddOption
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOption(Guid propertyId, string? value)
    {
        var property = await _context.CustomProperties
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == propertyId);
        if (property == null) return NotFound();
        // if (!await HasPermissionAsync(property.CampaignId, CampaignPermission.ManageProperties)) return Forbid();

        if (property.ValueType != PropertyValueType.List)
        {
            return LedgerError($"{property.Name} isn't a List property, so it can't have options.");
        }
        if (string.IsNullOrWhiteSpace(value))
        {
            return LedgerError("An option needs a value.");
        }

        value = value.Trim();
        if (value.Length > 50)
        {
            return LedgerError("Options can be at most 50 characters.");
        }
        // Keeping options unique is the whole point of predefined choices.
        if (property.Options.Any(o => string.Equals(o.Value, value, StringComparison.OrdinalIgnoreCase)))
        {
            return LedgerError($"{property.Name} already has an option called {value}.");
        }

        property.Options.Add(new PropertyOption { Value = value });
        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // POST: /CustomProperty/DeleteOption/{id}
    // Blocked while any item has this option selected, the same as Delete.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOption(Guid id)
    {
        var option = await _context.PropertyOptions.FindAsync(id);
        if (option == null) return NotFound();
        // var property = await _context.CustomProperties.FindAsync(option.CustomPropertyId);
        // if (!await HasPermissionAsync(property!.CampaignId, CampaignPermission.ManageProperties)) return Forbid();

        int usedBy = await _context.ItemPropertyValues.CountAsync(v => v.PropertyOptionId == id);
        if (usedBy > 0)
        {
            return LedgerError($"{option.Value} is used by {usedBy} item(s) and can't be deleted.");
        }

        _context.PropertyOptions.Remove(option);
        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // Case-insensitive, so "Rarity" and "rarity" count as the same name.
    // ignoreId lets Edit skip the property being edited.
    private Task<bool> NameTaken(string name, Guid? ignoreId)
    {
        string lowered = name.Trim().ToLower();
        return _context.CustomProperties.AnyAsync(p => p.Name.ToLower() == lowered && p.Id != ignoreId);
    }

    private Task<bool> HasItemWithMultipleOptions(Guid propertyId)
    {
        return _context.ItemPropertyValues
            .Where(v => v.CustomPropertyId == propertyId)
            .GroupBy(v => v.ItemId)
            .AnyAsync(g => g.Count() > 1);
    }

    private async Task<bool> HasPermissionAsync(int campaignId, CampaignPermission permission)
    {
        var campaign = await _context.Campaigns.FindAsync(campaignId);
        if (campaign == null) return false;
        var result = await _authorization.AuthorizeAsync(User, campaign, new CampaignPermissionRequirement(permission));
        return result.Succeeded;
    }

    // There's no ledger page yet, so go home for now. Change this when it exists.
    private IActionResult BackToLedger() => RedirectToAction("Index", "Home");

    // TempData survives one redirect, so the ledger page can display the message.
    private IActionResult LedgerError(string message)
    {
        TempData["Error"] = message;
        return BackToLedger();
    }
}
