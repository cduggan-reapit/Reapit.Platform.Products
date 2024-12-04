namespace Reapit.Platform.Products.Core.UseCases.Grants.DeleteGrant;

/// <summary>Request to delete a grant.</summary>
/// <param name="Id">The unique identifier of the grant.</param>
public record DeleteGrantCommand(string Id): IRequest<Entities.Grant>;