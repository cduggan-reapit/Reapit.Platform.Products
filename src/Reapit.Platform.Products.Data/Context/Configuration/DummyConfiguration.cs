using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Context.Configuration;

[ExcludeFromCodeCoverage]
public class DummyConfiguration : IEntityTypeConfiguration<Dummy>
{
    public void Configure(EntityTypeBuilder<Dummy> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.ToTable("dummies");

        builder.HasIndex(entity => entity.DateModified);
        
        builder.HasIndex(entity => entity.DateCreated);

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasMaxLength(36);
        
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasMaxLength(100);

        builder.Property(entity => entity.DateCreated)
            .HasColumnName("date_created");
        
        builder.Property(entity => entity.DateModified)
            .HasColumnName("date_modified");
    }
}