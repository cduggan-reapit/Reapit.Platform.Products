using System.ComponentModel.DataAnnotations.Schema;
using Reapit.Platform.Products.Domain.Entities.Abstract;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents an IDP client.</summary>
public class AccessClient : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="AccessClient"/> class.</summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="type"></param>
    /// <param name="callbackUrls"></param>
    /// <param name="signOutUrls"></param>
    public AccessClient(
        string name, 
        string? description,
        ClientType type,
        string[]? callbackUrls, 
        string[]? signOutUrls)
    {
        Name = name;
        Description = description;
        Type = type;
        CallbackUrls = callbackUrls;
        SignOutUrls = signOutUrls;
    }
    
    /// <summary>The unique identifier of the client in the IdP service.</summary>
    public string? IdpId { get; init; }
    
    /// <summary>The name of the client. Required, does not support <c>&lt;</c> or <c>&gt;</c>.</summary>
    public string Name { get; init; }

    /// <summary>A description of the client. Maximum 140 characters.</summary>
    public string? Description { get; init; }

    /// <summary>The type of client.</summary>
    public  ClientType Type { get; init; }
    
    /// <summary>Comma-separated list of URLs whitelisted for use as a callback to the client after authentication.</summary>
    /// <remarks>Only applicable to non-authCode clients.</remarks>
    public IEnumerable<string>? CallbackUrls { get; private set; }
    
    /// <summary>Comma-separated list of URLs that are valid to redirect to after logout. Wildcards are allowed for subdomains.</summary>
    /// <remarks>Only applicable to non-authCode clients.</remarks>
    public IEnumerable<string>? SignOutUrls { get; private set; }
    
    /// <inheritdoc />
    public override object AsSerializable()
        => new { Id, Type = Type.Name, Name, DateCreated, DateModified };
}