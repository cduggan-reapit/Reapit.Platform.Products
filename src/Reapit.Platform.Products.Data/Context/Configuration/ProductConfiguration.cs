using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Products.Data.Context.Configuration.Helpers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="Product"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("products");
        
        builder.HasIndex(entity => entity.Name)
            .IsUnique();
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);
    }
}