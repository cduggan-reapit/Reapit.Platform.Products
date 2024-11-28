using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Products.Data.Context.Converters;
using Reapit.Platform.Products.Domain.Entities.Abstract;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.Context.Configuration.Helpers;

/// <summary>Base configuration methods for entities inheriting <see cref="EntityBase"/>.</summary>
[ExcludeFromCodeCoverage]
public static class ClientEntityBaseConfigurationHelper
{
    /// <summary>Configures the entity of type <typeparamref name="T"/>.</summary>
    /// <param name="builder">The entity type builder.</param>
    /// <typeparam name="T">The type of EntityBase.</typeparam>
    public static EntityTypeBuilder<T> ConfigureClientEntityBase<T>(this EntityTypeBuilder<T> builder)
        where T: ClientEntityBase
    {
        builder.ConfigureEntityBase();
        
        // Make names unique - this should be a bit easier for us to manage in auth0
        builder.HasIndex(entity => new { entity.Name, entity.DateDeleted }).IsUnique();
        
        builder.Property(entity => entity.ClientId)
            .HasColumnName("clientId")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.GrantId)
            .HasColumnName("grantId")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);
        
        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasMaxLength(140);

        builder.Property(entity => entity.Type)
            .HasColumnName("type")
            .HasConversion(TypeConverters.ClientTypeConverter);
        
        builder.Property(entity => entity.Audience)
            .HasColumnName("audience")
            .HasMaxLength(1000);
        
        builder.Property(entity => entity.CallbackUrls)
            .HasColumnName("callbackUrls")
            .HasConversion(converter: TypeConverters.StringArrayConverter, valueComparer: TypeComparers.StringArrayComparer);

        builder.Property(entity => entity.SignOutUrls)
            .HasColumnName("signOutUrls")
            .HasConversion(converter: TypeConverters.StringArrayConverter, valueComparer: TypeComparers.StringArrayComparer);
        
        return builder;
    }
}