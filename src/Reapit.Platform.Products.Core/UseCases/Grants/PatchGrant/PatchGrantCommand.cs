namespace Reapit.Platform.Products.Core.UseCases.Grants.PatchGrant;

/// <summary>Request to update a grant.</summary>
/// <param name="Id">The unique identifier of the grant.</param>
/// <param name="Scopes">The scopes allowed for this grant.</param>
public record PatchGrantCommand(string Id, ICollection<string> Scopes)
    : IRequest<Entities.Grant>;