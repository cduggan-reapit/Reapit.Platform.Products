using Auth0.ManagementApi.Models;
using Microsoft.EntityFrameworkCore.Storage;
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
    
    /// <inheritdoc/>
    public async Task<bool> UpdateResourceServerAsync(
        Entities.ResourceServer entity,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating resource server: {id} ({externalId})", entity.Id, entity.ExternalId);
        using var client = await clientFactory.GetClientAsync(cancellationToken);

        var requestModel = new ResourceServerUpdateRequest
        {
            Name = entity.Name,
            Scopes = entity.Scopes.Select(model 
                => new ResourceServerScope { Value = model.Value, Description = model.Description }).ToList(),
            TokenLifetime = entity.TokenLifetime
        };

        var response = await client.ResourceServers.UpdateAsync(entity.ExternalId, requestModel, cancellationToken);
        if (response == null)
            throw IdentityProviderException.NullResponse;

        return true;
    }
    
    /// <inheritdoc/>
    public async Task<bool> DeleteResourceServerAsync(
        Entities.ResourceServer entity,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting resource server: {id} ({externalId})", entity.Id, entity.ExternalId);
        using var client = await clientFactory.GetClientAsync(cancellationToken);
        await client.ResourceServers.DeleteAsync(entity.ExternalId, cancellationToken);
        return true;
    }
    
    #endregion
}