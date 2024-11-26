using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Representation of a product.</summary>
/// <param name="name">The name of the product.</param>
/// <param name="description">An optional description of the product.</param>
/// <param name="reference">An optional, external reference to identify the product.</param>
public class Product(string name, string? description, string? reference) : EntityBase
{
    /// <summary>Update the properties of the product.</summary>
    /// <param name="name">The name of the product.</param>
    /// <param name="description">An optional description of the product.</param>
    /// <param name="reference">An optional, external reference to identify the product.</param>
    public void Update(string? name, string? description, string? reference)
    {
        Name = GetUpdateValue(Name, name);
        Description = GetUpdateValue(Description, description);
        Reference = GetUpdateValue(Reference, reference);
    }
    
    /// <summary>The name of the product.</summary>
    public string Name { get; private set; } = name;

    /// <summary>A description of the product.</summary>
    public string? Description { get; private set; } = description;

    /// <summary>An external reference to identify the product (e.g. agencyCloud).</summary>
    public string? Reference { get; private set; } = reference;

    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, Reference, DateCreated, DateModified };
}