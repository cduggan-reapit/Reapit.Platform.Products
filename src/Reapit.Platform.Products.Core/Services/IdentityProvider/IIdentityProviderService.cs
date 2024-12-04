using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider;

/// <summary>Service exposing methods for interacting with the IdP.</summary>
public interface IIdentityProviderService
{
    #region Resource Servers
    
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

    #endregion
    
    #region Clients
    
    /// <summary>Create a new authentication code client.</summary>
    /// <param name="command">The creation command.</param>
    /// <param name="isFirstParty">Flag indicating whether the client is associated with a first party application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<string> CreateAuthCodeClientAsync(CreateClientCommand command, bool isFirstParty, CancellationToken cancellationToken);
    
    /// <summary>Create a new machine-to-machine client.</summary>
    /// <param name="command">The creation command.</param>
    /// <param name="isFirstParty">Flag indicating whether the client is associated with a first party application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<string> CreateMachineClientAsync(CreateClientCommand command, bool isFirstParty, CancellationToken cancellationToken);

    /// <summary>Update an authentication code client.</summary>
    /// <param name="entity">The domain representation of the client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<bool> UpdateAuthCodeClientAsync(Entities.Client entity, CancellationToken cancellationToken);

    /// <summary>Update a machine code client.</summary>
    /// <param name="entity">The domain representation of the client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<bool> UpdateMachineClientAsync(Entities.Client entity, CancellationToken cancellationToken);
    
    /// <summary>Delete a client.</summary>
    /// <param name="entity">The domain representation of the client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the deletion was successful.</returns>
    Task<bool> DeleteClientAsync(Entities.Client entity, CancellationToken cancellationToken);
    
    #endregion
    
    #region Grants

    Task<string> CreateGrantAsync(CreateGrantCommand command, Entities.Client client, Entities.ResourceServer resourceServer, CancellationToken cancellationToken);

    Task<bool> UpdateGrantAsync(Entities.Grant grant, CancellationToken cancellationToken);
    
    Task<bool> DeleteGrantAsync(Entities.Grant grant, CancellationToken cancellationToken);

    #endregion
}