using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Configuration;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Auth0.Factories;

/// <inheritdoc/>
public class TokenFactory : ITokenFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Auth0ConfigurationOptions _options;
    private readonly ILogger<TokenFactory> _logger;

    private string? _token;
    private DateTimeOffset _nextTokenRefresh = DateTimeOffset.MinValue;
    
    /// <summary>Initializes a new instance of the <see cref="TokenFactory"/> class.</summary>
    /// <param name="httpClientFactory">The http client factory.</param>
    /// <param name="optionsProvider">The auth0 options provider.</param>
    /// <param name="logger">The logging service.</param>
    public TokenFactory(
        IHttpClientFactory httpClientFactory,
        IOptions<Auth0ConfigurationOptions> optionsProvider,
        ILogger<TokenFactory> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = optionsProvider.Value;
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffsetProvider.Now;
        if (now < _nextTokenRefresh && _token is not null)
        {
            _logger.LogDebug("Re-using Auth0 access token. Next refresh at {next:hh:mm:ss} UTC", _nextTokenRefresh.UtcDateTime);
            return _token;
        }

        _nextTokenRefresh = now.AddSeconds(_options.TokenTimeToLive);
        _logger.LogInformation("Refreshing Auth0 access token. Next refresh at {next:hh:mm:ss} UTC", _nextTokenRefresh.UtcDateTime);
        var tokenRequest = new ClientCredentialsTokenRequest
        {
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            Audience = $"https://{_options.Domain}/api/v2/"
        };
        
        var httpClient = _httpClientFactory.CreateClient("auth0-client-factory");
        var authClient = new AuthenticationApiClient(new Uri($"https://{_options.Domain}"), new HttpClientAuthenticationConnection(httpClient));
        var response = await authClient.GetTokenAsync(tokenRequest, cancellationToken);
        
        // Tidy up.  This is a singleton and httpClient does some funny stuff if you don't dispose it.
        authClient.Dispose();
        httpClient.Dispose();
        
        return _token = response.AccessToken;
    }
}