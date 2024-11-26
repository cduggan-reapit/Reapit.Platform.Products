using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.Entities.Abstract;

/// <summary>Base entity type for IdP clients.</summary>
public abstract class ClientEntityBase : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="ClientEntityBase"/> class.</summary>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="grantId">The unique identifier of the client grant in the IdP service.</param>
    /// <param name="name">The name of the client. Does not support <c>&lt;</c> or <c>&gt;</c>.</param>
    /// <param name="description">A description of the client. Maximum 140 characters.</param>
    /// <param name="type">The type of client.</param>
    /// <param name="callbackUrls">Comma-separated list of URLs whitelisted for use as a callback to the client after authentication.</param>
    /// <param name="signOutUrls">Comma-separated list of URLs that are valid to redirect to after logout. Wildcards are allowed for subdomains.</param>
    protected ClientEntityBase(
        string clientId,
        string? grantId,
        string name, 
        string? description,
        ClientType type,
        ICollection<string>? callbackUrls, 
        ICollection<string>? signOutUrls)
    {
        ClientId = clientId;
        GrantId = grantId;
        Name = name;
        Description = description;
        Type = type;
        CallbackUrls = callbackUrls;
        SignOutUrls = signOutUrls;
    }
    
    /// <summary>Update an instance of the <see cref="ProductClient"/> class.</summary>
    /// <param name="name">The name of the client. Does not support <c>&lt;</c> or <c>&gt;</c>.</param>
    /// <param name="description">A description of the client. Maximum 140 characters.</param>
    /// <param name="callbackUrls">Comma-separated list of URLs whitelisted for use as a callback to the client after authentication.</param>
    /// <param name="signOutUrls">Comma-separated list of URLs that are valid to redirect to after logout. Wildcards are allowed for subdomains.</param>
    public void Update(
        string? name = null, 
        string? description = null, 
        ICollection<string>? callbackUrls = null, 
        ICollection<string>? signOutUrls = null)
    {
        Name = GetUpdateValue(Name, name);
        Description = GetUpdateValue(Description, description);
        CallbackUrls = GetUpdateValue(CallbackUrls, callbackUrls);
        SignOutUrls = GetUpdateValue(SignOutUrls, signOutUrls);
    }
    
    /// <summary>The unique identifier of the client in the IdP service.</summary>
    public string ClientId { get; init; }
    
    /// <summary>The unique identifier of the client grant in the IdP service.</summary>
    /// <remarks>Only applicable to client_credentials clients.</remarks>
    public string? GrantId { get; init; }
    
    /// <summary>The name of the client. Required, does not support <c>&lt;</c> or <c>&gt;</c>.</summary>
    public string Name { get; private set; }

    /// <summary>A description of the client. Maximum 140 characters.</summary>
    public string? Description { get; private set; }

    /// <summary>The type of client.</summary>
    public ClientType Type { get; init; }
    
    /// <summary>Comma-separated list of URLs whitelisted for use as a callback to the client after authentication.</summary>
    public ICollection<string>? CallbackUrls { get; private set; }
    
    /// <summary>Comma-separated list of URLs that are valid to redirect to after logout. Wildcards are allowed for subdomains.</summary>
    public ICollection<string>? SignOutUrls { get; private set; }
}