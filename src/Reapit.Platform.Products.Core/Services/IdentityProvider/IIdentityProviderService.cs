using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider;

/// <summary>Service exposing methods for interacting with the IdP.</summary>
public interface IIdentityProviderService
{
    /// <summary>Create a new resource server.</summary>
    /// <param name="command">The creation command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The system-generated identifier of the created resource server.</returns>
    Task<string> CreateResourceServerAsync(CreateResourceServerCommand command, CancellationToken cancellationToken);

    /// <summary>Update a resource server.</summary>
    /// <param name="entity">The domain representation of the resource server.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the update was successful.</returns>
    Task<bool> UpdateResourceServerAsync(Entities.ResourceServer entity, CancellationToken cancellationToken);
    
    /// <summary>Delete a resource server.</summary>
    /// <param name="entity">The domain representation of the resource server.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the deletion was successful.</returns>
    Task<bool> DeleteResourceServerAsync(Entities.ResourceServer entity, CancellationToken cancellationToken);

}