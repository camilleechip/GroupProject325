// Gives validation rules like [Required], [StringLength], and [Range]
using System.ComponentModel.DataAnnotations;
// Gives us [Column], which controls how a value is stored in the database
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupProject325.Models;

// An encounter is made by the GM and gives the campaign gold, xp, and items
public class Encounter
{
    // Unique id for each encounter (the primary key).
    // EF Core sees a property named "Id" and fills it in automatically when the encounter is saved.
    public Guid Id { get; set; }

    // The name of the encounter, like "Goblin Ambush" or "Dragon's Hoard".
    // The GM must fill this in. It can't be longer than 100 characters.
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    // Optional notes about the encounter, like what happened or where it took place.
    // The ? after string means it's allowed to be empty (null), so the GM can skip it.
    public string? Description { get; set; }

    // How much gold the campaign gets from this encounter. It can't be negative (you can't win -50 gold).
    // Store it with 2 decimal places, same as Item's Value.
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal GoldReward { get; set; }

    // How much experience (XP) the campaign gets from this encounter.
    // XP is always a whole number, so it's an int. It can't be negative.
    [Range(0, int.MaxValue)]
    public int XpReward { get; set; }

    // Starts as false. ApplyRewards sets it to true after giving out the rewards.
    // Once it's true, the encounter can't be deleted, applied again, or have items changed.
    // The GM can still fix the gold and xp through Edit. Only the difference
    // is added to (or taken from) the campaign, so nothing is ever given out twice.
    public bool RewardsApplied { get; set; }

    // The campaign this encounter belongs to.
    // Campaign uses an int for its id, so this has to be an int too.
    // TODO: add a Campaign navigation property once Campaign's namespace is fixed.
    public int CampaignId { get; set; }

    // The id of the GM who created this encounter.
    // User uses a Guid for its id, so this is a Guid.
    // TODO: add a User navigation property once User is in LedgerContext
    public Guid CreatedByUserId { get; set; }

    // The items this encounter gives out, like a sword or a bag of gems.
    // One encounter can give out many items.
    // EF Core adds an EncounterId column to the Items table to link each item back here.
    public ICollection<Item> Items { get; set; } = new List<Item>();

    // When the encounter was created and when it was last changed.
    // The current time in UTC one standard time zone for everyone.
    // Same names as Item uses, so it matches.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}