using System.ComponentModel.DataAnnotations;

namespace GroupProject325.Models;

/// <summary>
/// A character played by a user. A user can have many characters.
/// </summary>
public class Character
{
    public int Id { get; set; }

    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    // The owner. Set on the server from the signed-in user, 
    // never bound from a form.
    public Guid UserId { get; set; }
    public User? User { get; set; }
}