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
    public DbSet<Campaign> Campaigns => Set<Campaign>();

    public DbSet<User> Users => Set<User>();
    // COMMENTING THIS OUT UNTIL PARTY IS DEFINED
    // public DbSet<Party> Parties => Set<Party>();
    public DbSet<UserInventory> UserInventories => Set<UserInventory>();
    // COMMENTING THIS OUT UNTIL ENCOUNTER IS DEFINED
    // public DbSet<Encounter> Encounters => Set<Encounter>();

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
    }
}