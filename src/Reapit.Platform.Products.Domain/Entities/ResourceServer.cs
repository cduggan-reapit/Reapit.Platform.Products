using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Representation of a product.</summary>
/// <param name="name">The name of the product.</param>
public class ResourceServer(string externalId, string audience, string name) : EntityBase
{
    /// <summary>Update the properties of the product.</summary>
    /// <param name="name">The name of the product.</param>
     /// <param name="tokenLifetime">Expiration value (in seconds) for access tokens.</param>
    public void Update(string? name, int? tokenLifetime = null)
    {
        Name = GetUpdateValue(Name, name);
        TokenLifetime = GetUpdateValue(TokenLifetime, tokenLifetime) ?? TokenLifetime;
    }

    /// <summary>The unique identifier of the resource server in the IdP system.</summary>
    public string ExternalId { get; init; } = externalId;
    
    /// <summary>The name of the product.</summary>
    public string Name { get; private set; } = name;

    /// <summary>Unique identifier for the API used as the audience parameter on authorization calls. Can not be changed once set.</summary>
    public string Audience { get; init; } = audience;
    
    /// <summary>Expiration value (in seconds) for access tokens.</summary>
    public int TokenLifetime { get; private set; }
    
    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, DateCreated, DateModified };
    
}