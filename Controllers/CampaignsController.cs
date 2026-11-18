using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupProject325.Data;
using GroupProject325.Models;

namespace GroupProject325.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly LedgerContext _context;

    public CampaignsController(LedgerContext context)
    {
        _context = context;
    }

    // GET: api/campaigns
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns()
    {
        return await _context.Campaigns.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    // GET: api/campaigns/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Campaign>> GetCampaign(int id)
    {
        var campaign = await _context.Campaigns.FindAsync(id);
        if (campaign == null)
        {
            return NotFound();
        }
        return campaign;
    }

    // POST: api/campaigns
    [HttpPost]
    public async Task<ActionResult<Campaign>> CreateCampaign(Campaign campaign)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        campaign.CreatedAt = DateTime.UtcNow;

        // Members come from the request body too, so clear them. Otherwise anyone could add themselves as GM.
        campaign.Members.Clear();

        // Make the creator the GM. Enable once OAuth sign-in exists.
        // var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        // var creator = await _context.Users.FirstOrDefaultAsync(u =>
        //     u.OAuthProvider == claim!.Issuer && u.OAuthSubject == claim.Value);
        // if (creator == null)
        // {
        //     return Unauthorized();
        // }
        // campaign.Members.Add(new CampaignMember { UserId = creator.Id, Role = CampaignRole.GM });

        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCampaign), new { id = campaign.Id }, campaign);
    }

    // PUT: api/campaigns/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCampaign(int id, Campaign campaign)
    {
        if (id != campaign.Id)
        {
            return BadRequest("Campaign ID mismatch.");
        }

        var existing = await _context.Campaigns.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        // Update the 3 editable properties
        existing.Title = campaign.Title;
        existing.Description = campaign.Description;
        existing.System = campaign.System;

        // Auto-update timestamp
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/campaigns/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCampaign(int id)
    {
        var campaign = await _context.Campaigns.FindAsync(id);
        if (campaign == null)
        {
            return NotFound();
        }

        _context.Campaigns.Remove(campaign);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
