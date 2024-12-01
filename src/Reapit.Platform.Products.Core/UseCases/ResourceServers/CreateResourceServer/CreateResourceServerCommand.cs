using Reapit.Platform.Products.Core.UseCases.ResourceServers.Shared;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;

/// <summary>Request to create a new resource server.</summary>
/// <param name="Name">The name of the resource server.</param>
/// <param name="Audience">The audience parameter on authorization calls.</param>
/// <param name="TokenLifetime">The expiration value (in seconds) for access tokens.</param>
/// <param name="Scopes">List of scopes that this resource server provides.</param>
public record CreateResourceServerCommand(
    string Name,
    string Audience,
    int TokenLifetime,
    ICollection<ResourceServerRequestScopeModel> Scopes) 
    : IRequest<Entities.ResourceServer>;