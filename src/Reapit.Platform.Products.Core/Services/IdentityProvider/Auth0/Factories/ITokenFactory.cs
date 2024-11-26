namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Auth0.Factories;

/// <summary>Auth0 token generation factory with caching.</summary>
public interface ITokenFactory
{
    /// <summary>Get an auth0 access token.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
}