using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context.Configuration;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context;

public class DemoDbContext : DbContext
{
    public DbSet<Dummy> Dummies { get; set; }

    public DemoDbContext(DbContextOptions<DemoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfiguration(new DummyConfiguration());
}