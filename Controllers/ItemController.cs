using System.Globalization;
using GroupProject325.Authorization;
using GroupProject325.Data;
using GroupProject325.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupProject325.Controllers;

// Items are listed on the ledger page, so there is no Index action here.
// Every action finishes by sending the user back to the ledger.
// The permission checks are commented out until items belong to a campaign (see docs/ERD.md).
public class ItemController : Controller
{
    private readonly LedgerContext _context;
    private readonly IAuthorizationService _authorization;

    public ItemController(LedgerContext context, IAuthorizationService authorization)
    {
        _context = context;
        _authorization = authorization;
    }

    // GET: /Item/Create
    public IActionResult Create()
    {
        // if (!await HasPermissionAsync(campaignId, CampaignPermission.AddItems)) return Forbid();
        return View();
    }

    // POST: /Item/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,Value,Weight,Quantity")] Item item)
    {
        // if (!await HasPermissionAsync(campaignId, CampaignPermission.AddItems)) return Forbid();
        // TODO: place the item in the party inventory once inventories exist
        if (!ModelState.IsValid) return View(item);

        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // GET: /Item/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();
        // if (!await HasPermissionAsync(item.CampaignId, CampaignPermission.EditItems)) return Forbid();
        return View(item);
    }

    // POST: /Item/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,Value,Weight,Quantity")] Item form)
    {
        if (!ModelState.IsValid) return View(form);

        // Copy onto the stored item instead of saving the form object, so fields
        // that aren't on the form (Id, CreatedAt) can't be overwritten.
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();
        // if (!await HasPermissionAsync(item.CampaignId, CampaignPermission.EditItems)) return Forbid();

        item.Name = form.Name;
        item.Description = form.Description;
        item.Value = form.Value;
        item.Weight = form.Weight;
        item.Quantity = form.Quantity;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // POST: /Item/Delete/{id}
    // A GM correction ("this item shouldn't exist"), not an in-story action. See UseUp.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();
        // if (!await HasPermissionAsync(item.CampaignId, CampaignPermission.DeleteItems)) return Forbid();

        // The item's property values are cascade-deleted with it.
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // POST: /Item/UseUp/{id}
    // Drinking a potion, firing arrows. Removes the item when the stack hits 0.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UseUp(Guid id, int amount)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();
        // if (!await HasPermissionAsync(item.CampaignId, CampaignPermission.UseItems)) return Forbid();

        if (amount <= 0 || amount > item.Quantity)
        {
            return LedgerError($"You can use between 1 and {item.Quantity} of {item.Name}.");
        }

        item.Quantity -= amount;
        if (item.Quantity == 0)
        {
            _context.Items.Remove(item);
        }
        else
        {
            item.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // POST: /Item/Split/{id}
    // "I'll take 10 of the 50 arrows." Creates a second stack with the same details.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Split(Guid id, int amount)
    {
        // TODO: put the new stack in the requesting player's inventory once inventories exist
        var item = await _context.Items
            .Include(i => i.PropertyValues)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (item == null) return NotFound();
        // if (!await HasPermissionAsync(item.CampaignId, CampaignPermission.UseItems)) return Forbid();

        // Splitting the whole stack (or more) isn't a split; at least 1 must stay behind.
        if (amount <= 0 || amount >= item.Quantity)
        {
            return LedgerError($"You can split off between 1 and {item.Quantity - 1} of {item.Name}.");
        }

        var newStack = new Item
        {
            Name = item.Name,
            Description = item.Description,
            Value = item.Value,
            Weight = item.Weight,
            Quantity = amount,
        };

        // New ItemPropertyValue rows, not the same objects: each row can only belong to one item.
        foreach (var propertyValue in item.PropertyValues)
        {
            newStack.PropertyValues.Add(new ItemPropertyValue
            {
                CustomPropertyId = propertyValue.CustomPropertyId,
                Value = propertyValue.Value,
                PropertyOptionId = propertyValue.PropertyOptionId,
            });
        }

        item.Quantity -= amount;
        item.UpdatedAt = DateTime.UtcNow;

        _context.Items.Add(newStack);
        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // POST: /Item/SetProperty
    // Sets one custom property on an item, replacing any value it already had.
    // Text and Number properties use `value`; List properties use `optionIds`.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetProperty(Guid itemId, Guid customPropertyId, string? value, List<Guid>? optionIds)
    {
        optionIds ??= new List<Guid>();

        var item = await _context.Items
            .Include(i => i.PropertyValues)
            .FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null) return NotFound();
        // if (!await HasPermissionAsync(item.CampaignId, CampaignPermission.EditItems)) return Forbid();

        var property = await _context.CustomProperties
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == customPropertyId);
        if (property == null) return NotFound();

        var newValues = new List<ItemPropertyValue>();

        switch (property.ValueType)
        {
            case PropertyValueType.Text:
                if (string.IsNullOrWhiteSpace(value))
                {
                    return LedgerError($"{property.Name} needs a value.");
                }
                newValues.Add(new ItemPropertyValue { CustomPropertyId = property.Id, Value = value.Trim() });
                break;

            case PropertyValueType.Number:
                // Stored as text, so check it parses. InvariantCulture means "." is always the decimal point.
                if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
                {
                    return LedgerError($"{property.Name} must be a number.");
                }
                newValues.Add(new ItemPropertyValue
                {
                    CustomPropertyId = property.Id,
                    Value = number.ToString(CultureInfo.InvariantCulture),
                });
                break;

            case PropertyValueType.List:
                var chosen = optionIds.Distinct().ToList();
                if (chosen.Count == 0)
                {
                    return LedgerError($"Choose an option for {property.Name}.");
                }
                // AllowMultiple can't be enforced by the database, so it's enforced here.
                if (!property.AllowMultiple && chosen.Count > 1)
                {
                    return LedgerError($"{property.Name} only allows one option.");
                }
                // Make sure every option actually belongs to this property.
                if (chosen.Any(optionId => property.Options.All(o => o.Id != optionId)))
                {
                    return BadRequest();
                }
                foreach (var optionId in chosen)
                {
                    newValues.Add(new ItemPropertyValue { CustomPropertyId = property.Id, PropertyOptionId = optionId });
                }
                break;
        }

        var oldValues = item.PropertyValues.Where(v => v.CustomPropertyId == property.Id).ToList();
        _context.ItemPropertyValues.RemoveRange(oldValues);
        foreach (var newValue in newValues)
        {
            item.PropertyValues.Add(newValue);
        }
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // POST: /Item/RemoveProperty
    // Removes a custom property (and all of its values) from one item.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProperty(Guid itemId, Guid customPropertyId)
    {
        var item = await _context.Items
            .Include(i => i.PropertyValues)
            .FirstOrDefaultAsync(i => i.Id == itemId);
        if (item == null) return NotFound();
        // if (!await HasPermissionAsync(item.CampaignId, CampaignPermission.EditItems)) return Forbid();

        var values = item.PropertyValues.Where(v => v.CustomPropertyId == customPropertyId).ToList();
        if (values.Count == 0) return NotFound();

        _context.ItemPropertyValues.RemoveRange(values);
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return BackToLedger();
    }

    // TODO: Claim, Return, Give once inventories exist.

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
