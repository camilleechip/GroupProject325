using System.ComponentModel.DataAnnotations;

namespace GroupProject325.Models;

/// <summary>
/// An application user. Identity comes from OAuth, so there is no password. The OAuth
/// email claim is used to match a sign-in to a User row.
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, StringLength(100, MinimumLength = 1)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    // Which provider issued the identity, e.g. "Google".
    [Required, StringLength(50)]
    public string OAuthProvider { get; set; } = string.Empty;

    // The provider's stable ID for this person (the "sub" claim). It never changes.
    [Required, StringLength(255)]
    public string OAuthSubject { get; set; } = string.Empty;

    public PartyRole Role { get; set; } = PartyRole.Member;

    // The ID of the party the user belongs to. It is null until the user joins a party.
    // Stored as a foreign key in the Users table.
    public Guid? PartyId { get; set; }

    // The party the user belongs to, so code can write user.Party.Name. 
    // It is null until the user joins a party. Many users can share one party.
    // public Party? Party { get; set; }

    // This user's personal inventory of items. Each user has one, and it is 
    // created and deleted together with the user. 
    // It is null only on a User object that was loaded without its inventory.
    public UserInventory? Inventory { get; set; }

    // The encounters this user has created (only a GM has that permission). 
    // It starts as an empty list and is never null.
    public List<Encounter> Encounters { get; set; } = new();
}