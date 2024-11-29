using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Representation of a resource server.</summary>
public class ResourceServer(string externalId, string audience, string name, int tokenLifetime) : EntityBase
{
    /// <summary>Update the properties of the resource server.</summary>
    /// <param name="name">The name of the resource server.</param>
     /// <param name="tokenLifetime">Expiration value (in seconds) for access tokens.</param>
    public void Update(string? name = null, int? tokenLifetime = null)
    {
        Name = GetUpdateValue(Name, name);
        TokenLifetime = GetUpdateValue(TokenLifetime, tokenLifetime) ?? TokenLifetime;
    }

    /// <summary>The unique identifier of the resource server in the IdP system.</summary>
    public string ExternalId { get; } = externalId;
    
    /// <summary>The name of the resource server.</summary>
    public string Name { get; private set; } = name;

    /// <summary>Unique identifier for the API used as the audience parameter on authorization calls. Can not be changed once set.</summary>
    public string Audience { get; } = audience;

    /// <summary>Expiration value (in seconds) for access tokens.</summary>
    public int TokenLifetime { get; private set; } = tokenLifetime;
    
    /// <summary>The scopes available for this resource server.</summary>
    public ICollection<Scope> Scopes { get; init; } = new List<Scope>();
    
    /// <summary>The grants associated with this resource server.</summary>
    public ICollection<Grant> Grants { get; init; } = new List<Grant>();
    
    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, DateCreated, DateModified };
}