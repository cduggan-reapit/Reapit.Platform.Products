using Reapit.Platform.Products.Domain.Entities.Abstract;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents an IDP client associated with a <see cref="Product"/>.</summary>
public class ProductClient : ClientEntityBase
{
    /// <summary>Initializes a new instance of the <see cref="ProductClient"/> class.</summary>
    /// <param name="productId">The unique identifier of the product with which the client is associated.</param>
    /// <param name="clientId">The unique identifier of the client in the IdP system.</param>
    /// <param name="grantId">The unique identifier of the client grant in the IdP system.</param>
    /// <param name="name">The name of the client. Does not support <c>&lt;</c> or <c>&gt;</c>.</param>
    /// <param name="description">A description of the client. Maximum 140 characters.</param>
    /// <param name="type">The type of client.</param>
    /// <param name="callbackUrls">Comma-separated list of URLs whitelisted for use as a callback to the client after authentication.</param>
    /// <param name="signOutUrls">Comma-separated list of URLs that are valid to redirect to after logout. Wildcards are allowed for subdomains.</param>
    public ProductClient(
        string productId, 
        string clientId, 
        string grantId, 
        string name, 
        string? description, 
        ClientType type, 
        IEnumerable<string>? callbackUrls, 
        IEnumerable<string>? signOutUrls)
        : base(clientId, grantId, name, description, type, callbackUrls, signOutUrls)
    {
        ProductId = productId;
    }
    
    /// <summary>The unique identifier of the product with which the client is associated.</summary>
    public string ProductId { get; }
    
    /// <inheritdoc />
    public override object AsSerializable()
        => new { Id, ProductId, Type = Type.Name, Name, DateCreated, DateModified };
}