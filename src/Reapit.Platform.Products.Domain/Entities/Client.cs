using Reapit.Platform.Products.Domain.Entities.Abstract;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents a client.</summary>
/// <remarks>Also known as an "application" in auth0 parlance.</remarks>
/// <param name="appId">The unique identifier of the application with which this client is associated.</param>
/// <param name="externalId">The unique identifier of the client in the IdP service.</param>
/// <param name="type">The type of the client.</param>
/// <param name="name">The name of the client.</param>
/// <param name="description">An optional description of the client/</param>
/// <param name="loginUrl">The default login initiation endpoint. Only applicable where type = `ClientType.AuthCode`.</param>
/// <param name="callbackUrls">The URLs that are valid to call back when authenticating users. Only applicable where type = `ClientType.AuthCode`.</param>
/// <param name="signOutUrls">The URLs that can be redirected to after logout. Only applicable where type = `ClientType.AuthCode`.</param>
public class Client(
    string appId,
    string externalId,
    ClientType type,
    string name,
    string? description,
    string? loginUrl,
    ICollection<string>? callbackUrls,
    ICollection<string>? signOutUrls)
    : EntityBase
{
    /// <summary>The unique identifier of the application with which this client is associated.</summary>
    public string AppId { get; } = appId;
    
    /// <summary>The unique identifier of the client in the IdP service.</summary>
    public string ExternalId { get; } = externalId;
    
    /// <summary>The type of the client.</summary>
    public ClientType Type { get; } = type;
    
    /// <summary>The name of the client. Required, does not support <c>&lt;</c> or <c>&gt;</c>.</summary>
    public string Name { get; protected  set; } = name;

    /// <summary>A description of the client. Maximum 140 characters.</summary>
    public string? Description { get; protected set; } = description;
    
    /// <summary>The application with which this client is associated.</summary>
    public App? App { get; init; }
    
    /// <summary>The default login initiation endpoint.</summary>
    /// <remarks>Must be a https:// url when configured.</remarks>
    public string? LoginUrl { get; private set; } = loginUrl;
    
    /// <summary>The URLs that are valid to call back when authenticating users.</summary>
    public ICollection<string>? CallbackUrls { get; private set; } = callbackUrls;
    
    /// <summary>The URLs that can be redirected to after logout.</summary>
    public ICollection<string>? SignOutUrls { get; private set; } = signOutUrls;
    
    /// <summary>The grants associated with this client.</summary>
    public ICollection<Grant> Grants { get; } = new List<Grant>();

    /// <summary>Update a client.</summary>
    /// <param name="name">The name of the client.</param>
    /// <param name="description">A description of the client.</param>
    /// <param name="loginUrl">The default login initiation endpoint.</param>
    /// <param name="callbackUrls">The URLs that are valid to call back when authenticating users.</param>
    /// <param name="signOutUrls">The URLs that can be redirected to after logout.</param>
    public void Update(
        string? name = null, 
        string? description = null, 
        string? loginUrl = null, 
        ICollection<string>? callbackUrls = null, 
        ICollection<string>? signOutUrls = null)
    {
        Name = GetUpdateValue(Name, name);
        Description = GetUpdateValue(Description, description);
        LoginUrl = GetUpdateValue(LoginUrl, loginUrl);
        CallbackUrls = GetCollectionUpdateValue(CallbackUrls, callbackUrls);
        SignOutUrls = GetCollectionUpdateValue(SignOutUrls, signOutUrls);
    }

    /// <inheritdoc/>
    public override void SoftDelete()
    {
        base.SoftDelete();
        foreach(var grant in Grants)
            grant.SoftDelete();
    }
    
    /// <inheritdoc />
    public override object AsSerializable()
        => new { Id, Name, Type = Type.Name, DateCreated, DateModified };
}