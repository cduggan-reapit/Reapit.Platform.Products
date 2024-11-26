using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Products.Data.Context.Configuration.Helpers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="ProductClient"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ProductClientConfiguration : IEntityTypeConfiguration<ProductClient>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ProductClient> builder)
    {
        builder.ConfigureClientEntityBase()
            .ToTable("product_clients");
        
        builder.Property(entity => entity.ProductId)
            .HasColumnName("productId")
            .HasMaxLength(36);

        builder.HasOne(client => client.Product)
            .WithMany(product => product.Clients)
            .HasForeignKey(client => client.ProductId);
    }
}