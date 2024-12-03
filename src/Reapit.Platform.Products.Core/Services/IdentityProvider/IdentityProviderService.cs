using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.Extensions;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;

namespace Reapit.Platform.Products.Core.Services.IdentityProvider;

/// <inheritdoc/> 
public class IdentityProviderService(
    IIdentityProviderClientFactory clientFactory,
    IOptions<IdentityProviderOptions> optionsProvider,
    ILogger<IdentityProviderService> logger) 
    : IIdentityProviderService
{
    private readonly IdentityProviderOptions _options = optionsProvider.Value;
    
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

        var scopes = entity.Scopes
            .Select(scope => new ResourceServerScope { Value = scope.Value, Description = scope.Description })
            .ToList();
        
        var requestModel = new ResourceServerUpdateRequest
        {
            Name = entity.Name,
            Scopes = scopes,
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
    
    #region Clients
    
    /// <inheritdoc/>
    public async Task<string> CreateAuthCodeClientAsync(CreateClientCommand command, bool isFirstParty, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new authorization code client: {command}", command.ToJson());
        
        // The AllowedLogoutUrls collection should always contain the auth0 login url. We add it into the collection here,
        // then deduplicate the list so it's not added twice:
        var requiredLogoutUrls = new[] { $"https://{_options.Domain}/login" };
        var allowedLogoutUrls = (command.SignOutUrls ?? []).Concat(requiredLogoutUrls).Distinct().ToArray();
        
        var requestModel = new ClientCreateRequest
        {
            Name = command.Name,
            Description = command.Description,
            IsFirstParty = isFirstParty,
            Callbacks = command.CallbackUrls?.ToArray(),
            AllowedLogoutUrls = allowedLogoutUrls,
            InitiateLoginUri = command.LoginUrl,
            IsCustomLoginPageOn = false,
            JwtConfiguration = new JwtConfiguration { SigningAlgorithm = "RS256" },
            ApplicationType = ClientApplicationType.RegularWeb,
            GrantTypes = ["authorization_code", "refresh_token"],
            OidcConformant = true,
            RefreshToken = new RefreshToken(),
            TokenEndpointAuthMethod = TokenEndpointAuthMethod.None
        };

        using var client = await clientFactory.GetClientAsync(cancellationToken);
        var response = await client.Clients.CreateAsync(requestModel, cancellationToken);
        if (response == null)
            throw IdentityProviderException.NullResponse;

        return response.ClientId;
    }
    
    /// <inheritdoc/>
    public async Task<string> CreateMachineClientAsync(CreateClientCommand command, bool isFirstParty, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new machine to machine client: {command}", command.ToJson());
        using var client = await clientFactory.GetClientAsync(cancellationToken);

        var requestModel = new ClientCreateRequest
        {
            Name = command.Name,
            Description = command.Description,
            IsFirstParty = isFirstParty,
            IsCustomLoginPageOn = false,
            JwtConfiguration = new JwtConfiguration { SigningAlgorithm = "RS256" },
            ApplicationType = ClientApplicationType.NonInteractive,
            GrantTypes = ["client_credentials"],
            TokenEndpointAuthMethod = TokenEndpointAuthMethod.ClientSecretBasic
        };

        var response = await client.Clients.CreateAsync(requestModel, cancellationToken);
        if (response == null)
            throw IdentityProviderException.NullResponse;

        return response.ClientId;
    }
    
    /// <inheritdoc/>
    public async Task<bool> UpdateAuthCodeClientAsync(Entities.Client entity, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating authorization code client: {command}", entity.ToJson());
        
        // The AllowedLogoutUrls collection should always contain the auth0 login url. We add it into the collection here,
        // then deduplicate the list so it's not added twice:
        var requiredLogoutUrls = new[] { $"https://{_options.Domain}/login" };
        var allowedLogoutUrls = (entity.SignOutUrls ?? []).Concat(requiredLogoutUrls).Distinct().ToArray();
        
        var requestModel = new ClientUpdateRequest
        {
            Name = entity.Name,
            Description = entity.Description,
            AllowedLogoutUrls = allowedLogoutUrls,
            InitiateLoginUri = entity.LoginUrl,
            Callbacks = entity.CallbackUrls?.ToArray()
        };

        using var client = await clientFactory.GetClientAsync(cancellationToken);
        var response = await client.Clients.UpdateAsync(entity.ExternalId, requestModel, cancellationToken);
        if (response == null)
            throw IdentityProviderException.NullResponse;

        return true;
    }
    
    /// <inheritdoc/>
    public async Task<bool> UpdateMachineClientAsync(Entities.Client entity, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating machine to machine client: {command}", entity.ToJson());
        using var client = await clientFactory.GetClientAsync(cancellationToken);

        var requestModel = new ClientUpdateRequest
        {
            Name = entity.Name,
            Description = entity.Description
        };

        var response = await client.Clients.UpdateAsync(entity.ExternalId, requestModel, cancellationToken);
        if (response == null)
            throw IdentityProviderException.NullResponse;

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteClientAsync(Entities.Client entity, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting client: {id} ({externalId})", entity.Id, entity.ExternalId);
        using var client = await clientFactory.GetClientAsync(cancellationToken);
        await client.Clients.DeleteAsync(entity.ExternalId, cancellationToken);
        return true;
    }

    #endregion
}