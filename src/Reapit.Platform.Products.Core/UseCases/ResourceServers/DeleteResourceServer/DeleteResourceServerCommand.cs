namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.DeleteResourceServer;

/// <summary>Request to delete a resource server.</summary>
/// <param name="Id">The unique identifier of the resource server.</param>
public record DeleteResourceServerCommand(string Id) : IRequest<Entities.ResourceServer>;