using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.Extensions;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider;

/// <inheritdoc/> 
public class IdentityProviderService(IIdentityProviderClientFactory clientFactory, ILogger<IdentityProviderService> logger) 
    : IIdentityProviderService
{
    #region Resource Servers
    
    /// <inheritdoc/>
    public async Task<string> CreateResourceServerAsync(
        CreateResourceServerCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new resource server: {command}", command.ToJson());
        using var client = await clientFactory.GetClientAsync(cancellationToken);

        var requestModel = new ResourceServerCreateRequest
        {
            Name = command.Name,
            Identifier = command.Audience,
            Scopes = command.Scopes.Select(model => model.ToResourceServerScope()).ToList(),
            AllowOfflineAccess = true,
            TokenLifetime = command.TokenLifetime,
            TokenDialect = TokenDialect.AccessToken,
            SkipConsentForVerifiableFirstPartyClients = true,
            SigningAlgorithm = SigningAlgorithm.RS256,
        };

        var response = await client.ResourceServers.CreateAsync(requestModel, cancellationToken);
        if (response == null)
            throw IdentityProviderException.NullResponse;

        return response.Id;
    }
    
    #endregion
}