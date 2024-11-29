using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Products.Data.Context.Configuration.Helpers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="ResourceServer"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ResourceServerConfiguration : IEntityTypeConfiguration<ResourceServer>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ResourceServer> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("resource_servers");
        
        builder.HasIndex(entity => new { entity.Name, entity.DateDeleted }).IsUnique();
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(200);
        
        builder.Property(entity => entity.Audience)
            .HasColumnName("audience")
            .HasMaxLength(600);

        builder.Property(entity => entity.ExternalId)
            .HasColumnName("externalId")
            .HasMaxLength(100);

        builder.Property(entity => entity.TokenLifetime)
            .HasColumnName("tokenLifetime");

        builder.HasMany(entity => entity.Scopes)
            .WithOne()
            .HasForeignKey(scope => scope.ResourceServerId);
    }
}