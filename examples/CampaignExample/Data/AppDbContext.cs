using Microsoft.EntityFrameworkCore;
using CampaignExample.Models;

namespace CampaignExample.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Campaign> Campaigns => Set<Campaign>();
}
