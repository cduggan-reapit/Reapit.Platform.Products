using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Representation of a product.</summary>
/// <param name="name">The name of the product.</param>
/// <param name="description">An optional description of the product.</param>
public class Product(string name, string? description) : EntityBase
{
    /// <summary>Update the properties of the product.</summary>
    /// <param name="name">The name of the product.</param>
    /// <param name="description">An optional description of the product.</param>
    public void Update(string? name, string? description)
    {
        Name = GetUpdateValue(Name, name);
        Description = GetUpdateValue(Description, description);
    }
    
    /// <summary>The name of the product.</summary>
    public string Name { get; private set; } = name;

    /// <summary>A description of the product.</summary>
    public string? Description { get; private set; } = description;
    
    /// <summary>The clients associated with this product.</summary>
    public ICollection<ProductClient> Clients { get; init; } = new List<ProductClient>();
    
    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id = Id, Name, DateCreated, DateModified };
}