using GroupProject325.Data;
using GroupProject325.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupProject325.Controllers;

// Handles creating encounters and giving their rewards to the campaign
// The GM makes an encounter, adds items to it, then applies it.
// Once applied, items can't be changed and the encounter can't be deleted or applied again.
// The GM can still fix the gold and xp, and only the difference goes to the campaign.
public class EncounterController : Controller
{
    // Our connection to the database, set once in the constructor
    private readonly LedgerContext _context;

    // ASP.NET passes in the database for us
    public EncounterController(LedgerContext context)
    {
        // Save it so every action in this class can use it
        _context = context;
    }

    // GET: /Encounter
    // Shows a list of all the encounters
    public async Task<IActionResult> Index()
    {
        // TODO: permission check, only show encounters for the user's campaign
        // Get every encounter, newest first
        var encounters = await _context.Encounters
            // Sort by the date it was made, newest at the top
            .OrderByDescending(e => e.CreatedAt)
            // Run the query and turn the results into a list
            .ToListAsync();
        // Send the list to the Index page to show it
        return View(encounters);
    }
    // GET: /Encounter/Details/{id}
    // Shows one encounter and the items it gives out
    public async Task<IActionResult> Details(Guid id)
    {
        // TODO: permission check, make sure the user is in this encounter's campaign

        // Find the encounter with this id
        var encounter = await _context.Encounters
            // Also load the items attached to this encounter
            .Include(e => e.Items)
            // Get the first match, or null if there isn't one
            .FirstOrDefaultAsync(e => e.Id == id);

        // If no encounter has that id, show a 404 not found page
        if (encounter == null) return NotFound();

        // Send the encounter (with its items) to the Details page
        return View(encounter);

    }

    // GET: /Encounter/Create
    // Shows the empty form for making a new encounter
    public IActionResult Create()
    {
        // TODO: permission check, only the GM can create encounters

        // Show the blank Create page
        return View();
    }

    // POST: /Encounter/Create
    // Runs when the GM submits the Create form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,GoldReward,XpReward,CampaignId")] Encounter encounter)
    {
        // TODO: permission check, only the GM can create encounters
        // TODO: once login works, set CampaignId and CreatedByUserId from the
        // logged in user instead of the form, and remove CampaignId from [Bind]

        // Check the rules on the model ([Required], [Range], etc.)
        // If something is wrong, show the form again with the error messages
        if (!ModelState.IsValid) return View(encounter);

        // Add the new encounter to the database
        _context.Encounters.Add(encounter);

        // Actually save it (nothing is saved until this runs)
        await _context.SaveChangesAsync();

        // Go to the new encounter's Details page so the GM can add items to it
        return RedirectToAction("Details", new { id = encounter.Id });
    }

    // GET: /Encounter/Edit/{id}
    // Shows the Edit form filled in with the encounter's current values
    public async Task<IActionResult> Edit(Guid id)
    {
        // TODO: permission check, only the GM can edit encounters

        // Look up the encounter by its id
        var encounter = await _context.Encounters.FindAsync(id);

        // If it doesn't exist, show a 404 not found page
        if (encounter == null) return NotFound();

        // Show the Edit page with the current values filled in
        return View(encounter);
    }

    // POST: /Encounter/Edit/{id}
    // Runs when the GM submits the Edit form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Title,Description,GoldReward,XpReward")] Encounter form)
    {
        // TODO: permission check, only the GM can edit encounters

        // Check the rules on the model
        // If something is wrong, show the form again with the error messages
        if (!ModelState.IsValid) return View(form);

        // Load the saved version of the encounter from the database
        var encounter = await _context.Encounters.FindAsync(id);

        // If it doesn't exist, show a 404 not found page
        if (encounter == null) return NotFound();

        // If the rewards were already given out, the campaign needs to be corrected too
        if (encounter.RewardsApplied)
        {
            // Work out how much the rewards changed
            // positive = the GM gave more, negative = the GM gave less
            var goldDifference = form.GoldReward - encounter.GoldReward;
            var xpDifference = form.XpReward - encounter.XpReward;
            // TODO: once Campaign has Gold and Experience, apply the difference:
            // var campaign = await _context.Campaigns.FindAsync(encounter.CampaignId);
            // if (campaign == null) return NotFound();
            //
            // Don't let a correction take the campaign below zero
            // if (campaign.Gold + goldDifference < 0)
            // {
            //     TempData["Error"] = "The campaign doesn't have enough gold left to take that much back.";
            //     return RedirectToAction("Details", new { id });
            // }
            //
            // campaign.Gold += goldDifference;
            // campaign.Experience += xpDifference;
        }

        // Copy the form values onto the saved encounter
        // (fields not on the form, like CampaignId and CreatedAt, stay the same)
        encounter.Title = form.Title;
        encounter.Description = form.Description;
        encounter.GoldReward = form.GoldReward;
        encounter.XpReward = form.XpReward;

        // Record when it was changed
        encounter.UpdatedAt = DateTime.UtcNow;

        // Save the changes (and the campaign correction, once that's added)
        await _context.SaveChangesAsync();

        // Go back to the Details page to see the updated encounter
        return RedirectToAction("Details", new { id });
    }
}