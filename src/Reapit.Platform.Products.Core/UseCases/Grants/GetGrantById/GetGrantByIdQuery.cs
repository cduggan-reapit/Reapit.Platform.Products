namespace Reapit.Platform.Products.Core.UseCases.Grants.GetGrantById;

/// <summary>Request to get a grant.</summary>
/// <param name="Id">The unique identifier of the grant.</param>
public record GetGrantByIdQuery(string Id) : IRequest<Entities.Grant>;