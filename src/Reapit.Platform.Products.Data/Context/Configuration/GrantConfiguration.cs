using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Products.Data.Context.Configuration.Helpers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context.Configuration;

public class GrantConfiguration : IEntityTypeConfiguration<Grant>
{
    public void Configure(EntityTypeBuilder<Grant> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("grants");
        
        builder.HasIndex(entity => new { entity.ClientId, entity.ResourceServerId, entity.DateDeleted }).IsUnique();
        
        builder.Property(entity => entity.ClientId)
            .HasColumnName("clientId")
            .HasMaxLength(36);
        
        builder.Property(entity => entity.ResourceServerId)
            .HasColumnName("resourceServerId")
            .HasMaxLength(36);
        
        builder.HasOne(grant => grant.Client)
            .WithMany(client => client.Grants)
            .HasForeignKey(grant => grant.ClientId);
        
        builder.HasOne(grant => grant.ResourceServer)
            .WithMany()
            .HasForeignKey(grant => grant.ResourceServerId);
        
        builder.HasMany(grant => grant.Scopes)
            .WithMany()
            .UsingEntity(joinEntityName: "grant_scopes",
                configureLeft: l => l.HasOne(typeof(Grant)).WithMany().HasForeignKey("grantId").HasPrincipalKey(nameof(Grant.Id)),
                configureRight: r => r.HasOne(typeof(Scope)).WithMany().HasForeignKey("scopeId").HasPrincipalKey(nameof(Scope.Id)),
                configureJoinEntityType: j => j.HasKey("grantId", "scopeId"));
    }
}