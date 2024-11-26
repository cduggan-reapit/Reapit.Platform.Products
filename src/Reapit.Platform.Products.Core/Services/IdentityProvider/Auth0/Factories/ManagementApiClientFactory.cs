using Auth0.ManagementApi;
using Microsoft.Extensions.Options;
using Reapit.Platform.Products.Core.Configuration;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Auth0.Factories;

/// <inheritdoc/>
public class ManagementApiClientFactory : IManagementApiClientFactory
{
    private readonly ITokenFactory _tokenFactory;
    private readonly Auth0ConfigurationOptions _options;

    /// <summary>Initializes a new instance of the <see cref="ManagementApiClientFactory"/> class.</summary>
    /// <param name="tokenFactory">The auth0 token factory.</param>
    /// <param name="optionsProvider">The auth0 options provider.</param>
    public ManagementApiClientFactory(ITokenFactory tokenFactory, IOptions<Auth0ConfigurationOptions> optionsProvider)
    {
        _tokenFactory = tokenFactory;
        _options = optionsProvider.Value;
    }

    /// <inheritdoc/>
    public async Task<IManagementApiClient> GetClientAsync(CancellationToken cancellationToken)
        => new ManagementApiClient(
            token: await _tokenFactory.GetAccessTokenAsync(cancellationToken), 
            baseUri: new Uri($"https://{_options.Domain}/api/v2/"));
}