using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Configuration;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;

/// <inheritdoc />
public class TokenCache (IHttpClientFactory httpClientFactory, IOptions<IdentityProviderOptions> options, ILogger<TokenCache> logger) 
    : ITokenCache
{
    private IdentityProviderOptions Configuration { get; } = options.Value;
    
    private string? _token;
    private DateTimeOffset _nextTokenRefresh = DateTimeOffset.MinValue;
    
    /// <inheritdoc/>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        // If we've already got a token and the cache window hasn't expired, just send that back.
        var now = DateTimeOffsetProvider.Now;
        if (now < _nextTokenRefresh && _token is not null)
        {
            logger.LogDebug("Re-using cached access token. Next refresh at: {next:HH:mm:ss} UTC", _nextTokenRefresh.UtcDateTime);
            return _token;
        }

        // Set the next refresh timestamp
        _nextTokenRefresh = now.AddSeconds(Configuration.TokenCacheSeconds);
        logger.LogInformation("Refreshing access token. Next refresh at: {next:HH:mm:ss} UTC", _nextTokenRefresh.UtcDateTime);
        
        // Request a new token
        var tokenRequest = new ClientCredentialsTokenRequest
        {
            ClientId = Configuration.ClientId,
            ClientSecret = Configuration.ClientSecret,
            Audience = $"https://{Configuration.Domain}/api/v2/"
        };
        var httpClient = httpClientFactory.CreateClient("auth0-auth-api-client");
        var authClient = new AuthenticationApiClient(
            baseUri: new Uri($"https://{Configuration.Domain}"), 
            connection: new HttpClientAuthenticationConnection(httpClient));
        var response = await authClient.GetTokenAsync(tokenRequest, cancellationToken);
        
        // Tidy up - this is a singleton, and we've instantiated these, so we need to make sure they get disposed.
        authClient.Dispose();
        httpClient.Dispose();
        
        return _token = response.AccessToken;
    }
}