using System.ComponentModel.DataAnnotations;

namespace GroupProject325.Models;

/// <summary>
/// Represents a Tabletop Role-Playing Game (TTRPG) campaign session/group.
/// Used to associate user accounts, inventory ledgers, and encounters.
/// </summary>
public class Campaign
{
    /// <summary>
    /// Unique identifier for the campaign (Primary Key).
    /// Examples: 1, 2, 42
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The display name or title of the campaign.
    /// Examples: "Curse of Strahd", "Lost Mine of Phandelver", "Starfinder: Dead Suns", "Wednesday Night Homebrew"
    /// </summary>
    [Required(ErrorMessage = "Campaign title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A summary, backstory, house rules, or overview of the campaign setting.
    /// Examples: 
    /// - "A gothic horror adventure set in the mist-shrouded valley of Barovia."
    /// - "Sci-fi space opera tracking salvage runs in the Diaspora asteroid belt."
    /// - "Low-magic survival campaign using gritty realism rest rules."
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The user handle, username, or name of the primary Game Master / Admin who manages the campaign (FR-6).
    /// Examples: "DungeonMasterDrew", "TroyB", "camille_gm", "SierraE"
    /// </summary>
    [Required(ErrorMessage = "Game Master (GM) name is required.")]
    [StringLength(100, ErrorMessage = "Game Master name cannot exceed 100 characters.")]
    public string GameMaster { get; set; } = string.Empty;

    /// <summary>
    /// The ruleset or tabletop system being used. Supports system-agnostic ledger usage.
    /// Examples: "D&D 5e", "Pathfinder 2e", "Call of Cthulhu 7e", "Cyberpunk RED", "Daggerheart", "System-Agnostic"
    /// </summary>
    [StringLength(100, ErrorMessage = "TTRPG System cannot exceed 100 characters.")]
    public string System { get; set; } = "System-Agnostic";

    /// <summary>
    /// UTC timestamp of when the campaign was created (FR-8 timestamping requirement).
    /// Examples: 2026-11-02T18:00:00Z
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp of the last update to the campaign details, or null if never modified.
    /// Examples: 2026-11-03T10:15:30Z, null
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// What Members of this campaign are allowed to do. Set by the GM.
    /// </summary>
    public CampaignPermission MemberPermissions { get; set; } = CampaignPermission.None;

    public List<CampaignMember> Members { get; set; } = new();
}
