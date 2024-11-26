using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Auth0.Factories;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Auth0.Models;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Auth0;

/// <inheritdoc/>
public class IdentityProviderService : IIdentityProviderService
{
    private readonly IManagementApiClientFactory _clientFactory;
    private readonly Auth0ConfigurationOptions _options;
    private readonly ILogger<IdentityProviderService> _logger;
    
    /// <summary>Initialize a new instance of the <see cref="IdentityProviderService"/> class.</summary>
    /// <param name="clientFactory">The auth0 management API client factory.</param>
    /// <param name="options">The identity provider options provider.</param>
    /// <param name="logger">The logging service.</param>
    public IdentityProviderService(
        IManagementApiClientFactory clientFactory,
        IOptions<Auth0ConfigurationOptions> options,
        ILogger<IdentityProviderService> logger)
    {
        _clientFactory = clientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CreateClientResult> AddClientAsync(
        AccessClient clientEntity,
        CancellationToken cancellationToken)
    {
        using var api = await _clientFactory.GetClientAsync(cancellationToken);

        var request = ClientRequestFactory.GetClientCreateRequest(clientEntity);
        var client = await api.Clients.CreateAsync(request, cancellationToken);

        if(clientEntity.Type != ClientType.ClientCredentials)
            return new CreateClientResult(client.ClientId, null);
        
        var grantRequest = new ClientGrantCreateRequest
        {
            ClientId = client.ClientId,
            Audience = _options.ClientGrantAudience,
            Scope = []
        };
        
        var grant = await api.ClientGrants.CreateAsync(grantRequest, cancellationToken);
        
        return new CreateClientResult(client.ClientId, grant.Id);
    }
}