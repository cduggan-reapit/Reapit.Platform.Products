namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;

/// <summary>Definition of the grant to create.</summary>
/// <param name="ClientId">The unique identifier of the client to which access will be granted.</param>
/// <param name="ResourceServerId">The unique identifier of the resource server to which access will be granted.</param>
/// <param name="Scopes">The scopes allowed for this grant.</param>
public record CreateGrantRequestModel(
    string ClientId, 
    string ResourceServerId, 
    ICollection<string> Scopes);