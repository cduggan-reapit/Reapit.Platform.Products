using Auth0.ManagementApi;
using Microsoft.Extensions.Options;
using Reapit.Platform.Products.Core.Configuration;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;

/// <inheritdoc/>
public class IdentityProviderClientFactory(ITokenCache tokenFactory, IOptions<IdentityProviderOptions> options) 
    : IIdentityProviderClientFactory
{
    /// <summary>The configuration object provided by `option`</summary>
    private IdentityProviderOptions Configuration { get; } = options.Value;
    
    /// <inheritdoc/>
    public async Task<IManagementApiClient> GetClientAsync(CancellationToken cancellationToken)
        => new ManagementApiClient(
            token: await tokenFactory.GetAccessTokenAsync(cancellationToken), 
            baseUri: new Uri($"https://{Configuration.Domain}/api/v2/"));
}