namespace GroupProject325.Models;

/// <summary>
/// Actions a GM can allow Members to do in their campaign. A GM can always do all of them.
/// </summary>
[Flags]
public enum CampaignPermission
{
    None = 0,
    ViewPartyInventory = 1 << 0,
    AddItems = 1 << 1,
    EditItems = 1 << 2,
    DeleteItems = 1 << 3
}
