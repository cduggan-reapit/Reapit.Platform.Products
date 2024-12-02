namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServerById;

/// <summary>Request to get a resource server.</summary>
/// <param name="Id">The unique identifier of the resource server.</param>
public record GetResourceServerByIdQuery(string Id) : IRequest<Entities.ResourceServer>;