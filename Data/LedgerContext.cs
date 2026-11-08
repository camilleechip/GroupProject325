using GroupProject325.Models;
using Microsoft.EntityFrameworkCore;

namespace GroupProject325.Data;

public class LedgerContext : DbContext
{
    public LedgerContext(DbContextOptions<LedgerContext> options) : base(options) {}

    public DbSet<Item> Items => Set<Item>();
    public DbSet<MinorItem> MinorItem => Set<MinorItem>();
    public DbSet<CustomProperty> CustomProperties => Set<CustomProperty>();
    public DbSet<PropertyOption> PropertyOptions => Set<PropertyOption>();
    public DbSet<ItemPropertyValue> ItemPropertyValues => Set<ItemPropertyValue>();

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
    }
}