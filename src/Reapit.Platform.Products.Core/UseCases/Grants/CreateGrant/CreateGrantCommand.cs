namespace Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;

/// <summary>Request to create a new grant.</summary>
/// <param name="ClientId">The unique identifier of the client to which access will be granted.</param>
/// <param name="ResourceServerId">The unique identifier of the resource server to which access will be granted.</param>
/// <param name="Scopes">The scopes allowed for this grant.</param>
public record CreateGrantCommand(string ClientId, string ResourceServerId, ICollection<string> Scopes)
    : IRequest<Entities.Grant>;