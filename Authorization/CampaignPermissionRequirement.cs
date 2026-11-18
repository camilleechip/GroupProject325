using GroupProject325.Models;
using Microsoft.AspNetCore.Authorization;

namespace GroupProject325.Authorization;

public class CampaignPermissionRequirement : IAuthorizationRequirement
{
    public CampaignPermission Permission { get; }

    public CampaignPermissionRequirement(CampaignPermission permission)
    {
        Permission = permission;
    }
}
