using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context.Configuration;

public class ScopeConfiguration : IEntityTypeConfiguration<Scope>
{
    public void Configure(EntityTypeBuilder<Scope> builder)
    {
        builder.ToTable("scopes");

        builder.HasIndex(entity => new { entity.Value, entity.ResourceServerId }).IsUnique();
        
        builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
        
        builder.Property(entity => entity.Value)
            .HasColumnName("value")
            .HasMaxLength(280);
        
        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(entity => entity.ResourceServerId)
            .HasColumnName("resourceServerId")
            .HasMaxLength(36);
    }
}