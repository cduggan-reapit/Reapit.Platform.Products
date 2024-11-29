using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Products.Data.Context.Configuration.Helpers;
using Reapit.Platform.Products.Data.Context.Converters;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context.Configuration;

/// <summary>Entity framework configuration for the <see cref="Client"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ProductClientConfiguration : IEntityTypeConfiguration<Client>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ConfigureEntityBase()
            .ToTable("clients");
        
        // Make names unique - makes it a bit easier for us to manage in auth0
        builder.HasIndex(entity => new { entity.Name, entity.DateDeleted }).IsUnique();
        
        builder.Property(entity => entity.AppId)
            .HasColumnName("appId")
            .HasMaxLength(36);
        
        builder.Property(entity => entity.ClientId)
            .HasColumnName("clientId")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.Type)
            .HasColumnName("type")
            .HasConversion(TypeConverters.ClientTypeConverter);
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasMaxLength(140);

        builder.Property(entity => entity.LoginUrl)
            .HasColumnName("loginUrl")
            .HasMaxLength(1000);
        
        builder.Property(entity => entity.CallbackUrls)
            .HasColumnName("callbackUrls")
            .HasConversion(converter: TypeConverters.StringArrayConverter, valueComparer: TypeComparers.StringArrayComparer);

        builder.Property(entity => entity.SignOutUrls)
            .HasColumnName("signOutUrls")
            .HasConversion(converter: TypeConverters.StringArrayConverter, valueComparer: TypeComparers.StringArrayComparer);

        // Client 1-N App
        builder.HasOne(client => client.App)
            .WithMany(app => app.Clients)
            .HasForeignKey(client => client.AppId);
    }
}