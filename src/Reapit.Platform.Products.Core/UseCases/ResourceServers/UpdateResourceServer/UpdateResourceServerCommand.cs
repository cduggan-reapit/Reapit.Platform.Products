using Reapit.Platform.Products.Core.UseCases.ResourceServers.Shared;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.UpdateResourceServer;

/// <summary>Request to delete a resource server.</summary>
/// <param name="Id">The unique identifier of the resource server.</param>
/// <param name="Name">The name of the resource server.</param>
/// <param name="TokenLifetime">The expiration value (in seconds) for access tokens.</param>
/// <param name="Scopes">List of scopes that this resource server provides.</param>
public record UpdateResourceServerCommand(string Id, 
    string? Name, 
    int? TokenLifetime, 
    ICollection<ResourceServerRequestScopeModel>? Scopes) : IRequest<Entities.ResourceServer>;