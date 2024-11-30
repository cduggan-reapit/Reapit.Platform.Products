using Auth0.ManagementApi;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;

/// <summary>Auth0 management API factory.</summary>
public interface IIdentityProviderClientFactory
{
    /// <summary>Get an instance of the Auth0 management API client.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IManagementApiClient> GetClientAsync(CancellationToken cancellationToken);
}