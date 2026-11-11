namespace GroupProject325.Models;

/// <summary>
/// A user's personal inventory. Each user has exactly one, 
// created when the user registers and deleted together with the user.
/// </summary>
public class UserInventory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign key to the owning user. Non-nullable, so deleting the user 
    // cascades and deletes the inventory too.
    public Guid UserId { get; set; }
    public User? User { get; set; }
}