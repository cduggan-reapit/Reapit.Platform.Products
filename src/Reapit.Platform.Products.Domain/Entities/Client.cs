using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Reapit.Platform.Products.Domain.Entities.Abstract;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents an IDP client.</summary>
public class Client : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Client"/> class.</summary>
    /// <param name="productId"></param>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="callbackUrls"></param>
    /// <param name="signOutUrls"></param>
    public Client(string productId, ClientType type, string name, string? description, string[]? callbackUrls, string[]? signOutUrls)
    {
        ProductId = productId;
        Type = type;
        Name = name;
        Description = description;
        CallbackUrls = callbackUrls;
        SignOutUrls = signOutUrls;
    }
    
    /// <summary>The name of the client. Required, does not support <c>&lt;</c> or <c>&gt;</c>.</summary>
    public string Name { get; private set; }
    
    /// <summary>A description of the client. Maximum 140 characters.</summary>
    public string? Description { get; private set; }
    
    /// <summary>Type of client used to determine which settings are applicable.</summary>
    public ClientType Type { get; private set; }
    
    /// <summary>Comma-separated list of URLs whitelisted for use as a callback to the client after authentication.</summary>
    /// <remarks>Only applicable to non-authCode clients.</remarks>
    public IEnumerable<string>? CallbackUrls { get; private set; }
    
    /// <summary>Comma-separated list of URLs that are valid to redirect to after logout. Wildcards are allowed for subdomains.</summary>
    /// <remarks>Only applicable to non-authCode clients.</remarks>
    public IEnumerable<string>? SignOutUrls { get; private set; }
    
    /// <summary>The unique identifier of the product with which the client is associated.</summary>
    public string ProductId { get; private init; }
    
    /// <summary>The product with which the client is associated.</summary>
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; private set; }
    
    /// <inheritdoc />
    public override object AsSerializable()
        => new { ProductId, Id, Type = Type.Name, Name, DateCreated, DateModified };
}