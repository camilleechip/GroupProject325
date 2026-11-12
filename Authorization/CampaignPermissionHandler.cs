using System.Security.Claims;
using GroupProject325.Data;
using GroupProject325.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GroupProject325.Authorization;

/// <summary>
/// A GM can do anything in their campaign. A Member can only do what the campaign's
/// MemberPermissions allow.
/// </summary>
public class CampaignPermissionHandler : AuthorizationHandler<CampaignPermissionRequirement, Campaign>
{
    private readonly LedgerContext _context;

    public CampaignPermissionHandler(LedgerContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CampaignPermissionRequirement requirement,
        Campaign campaign)
    {
        var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return;
        }

        var member = await _context.CampaignMembers.FirstOrDefaultAsync(m =>
            m.CampaignId == campaign.Id &&
            m.User!.OAuthProvider == claim.Issuer &&
            m.User.OAuthSubject == claim.Value);
        if (member == null)
        {
            return;
        }

        if (member.Role == CampaignRole.GM || campaign.MemberPermissions.HasFlag(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
