namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;

/// <summary>Management API token caching service.</summary>
public interface ITokenCache
{
    /// <summary>Get an auth0 access token.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
}