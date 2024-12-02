using Reapit.Platform.Products.Core.UseCases.Common.Scopes;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.PatchResourceServer;

/// <summary>Request to delete a resource server.</summary>
/// <param name="Id">The unique identifier of the resource server.</param>
/// <param name="Name">The name of the resource server.</param>
/// <param name="TokenLifetime">The expiration value (in seconds) for access tokens.</param>
/// <param name="Scopes">List of scopes that this resource server provides.</param>
public record PatchResourceServerCommand(string Id, 
    string? Name, 
    int? TokenLifetime, 
    ICollection<RequestScopeModel>? Scopes) : IRequest<Entities.ResourceServer>;