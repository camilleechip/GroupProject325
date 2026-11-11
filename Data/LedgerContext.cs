using GroupProject325.Models;
using Microsoft.EntityFrameworkCore;

namespace GroupProject325.Data;

public class LedgerContext : DbContext
{
    public LedgerContext(DbContextOptions<LedgerContext> options) : base(options) { }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<MinorItem> MinorItem => Set<MinorItem>();
    public DbSet<CustomProperty> CustomProperties => Set<CustomProperty>();
    public DbSet<PropertyOption> PropertyOptions => Set<PropertyOption>();
    public DbSet<ItemPropertyValue> ItemPropertyValues => Set<ItemPropertyValue>();

    public DbSet<User> Users => Set<User>();
    // COMMENTING THIS OUT UNTIL PARTY IS DEFINED
    // public DbSet<Party> Parties => Set<Party>();
    public DbSet<UserInventory> UserInventories => Set<UserInventory>();
    public DbSet<Encounter> Encounters => Set<Encounter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemPropertyValue>()
            .HasOne(v => v.CustomProperty)
            .WithMany()
            .HasForeignKey(v => v.CustomPropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ItemPropertyValue>()
            .HasOne(v => v.PropertyOption)
            .WithMany()
            .HasForeignKey(v => v.PropertyOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.OAuthProvider, u.OAuthSubject })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Encounter 1 -> Item
        // Item has no EncounterId property, so EF Core makes a hidden EncounterId column on Items.
        // If an encounter is deleted, its items stay but lose the link.
        modelBuilder.Entity<Encounter>()
            .HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey("EncounterId")
            .OnDelete(DeleteBehavior.SetNull);

        // User 1 -> Encounter (the GM who created it)
        // Tells EF Core to use CreatedByUserId instead of guessing a column called UserId.
        // A user who created encounters can't be deleted, which protects the ledger history.
        modelBuilder.Entity<User>()
            .HasMany(u => u.Encounters)
            .WithOne()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}