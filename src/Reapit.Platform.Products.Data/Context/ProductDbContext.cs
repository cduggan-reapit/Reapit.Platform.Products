using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context;

/// <summary>Initializes a new instance of the <see cref="ProductDbContext"/> class.</summary>
/// <param name="options">The database context options.</param>
public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    // V2.1!
    public DbSet<App> Apps { get; init; }
    
    public DbSet<Client> Clients { get; init; }
    
    
    
    
    /// <summary>The collection of products.</summary>
    public DbSet<ResourceServer> Products { get; init; }
    
    /// <inheritdoc /> 
    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
}