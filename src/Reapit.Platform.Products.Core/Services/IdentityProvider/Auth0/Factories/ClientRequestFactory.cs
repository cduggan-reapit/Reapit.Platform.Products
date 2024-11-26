using Auth0.ManagementApi.Models;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;


namespace Reapit.Platform.Products.Core.Services.IdentityProvider.Auth0.Factories;

public static class ClientRequestFactory
{

    /// <summary>Get a <see cref="ClientCreateRequest"/> representing an <see cref="ProductClient"/> entity.</summary>
    /// <param name="client">The access client</param>
    /// <exception cref="ArgumentException">when the access client type is not supported.</exception>
    public static ClientCreateRequest GetClientCreateRequest(ProductClient client)
    {
        if (client.Type == ClientType.AuthorizationCode)
            return GetUserClientCreateRequest(client);

        if (client.Type == ClientType.ClientCredentials)
            return GetMachineClientCreateRequest(client);
        
        // Metadata:
        //      Reapit.Services.Organisations   => { internalProductId = Id };
        //      Reapit.Marketplace              => { internalAppId = Id }
        throw new ArgumentException($"Unsupported client type: {client.Type.Name}");
    }

    /// <summary>Get the <see cref="ClientCreateRequest"/> for a code flow client.</summary>
    /// <param name="client">The access client.</param>
    private static ClientCreateRequest GetUserClientCreateRequest(ProductClient client) 
        => new()
        {
            IsFirstParty = true, 
            IsCustomLoginPageOn = false,
            Name = client.Name,
            Description = client.Description,
            TokenEndpointAuthMethod= TokenEndpointAuthMethod.None,
            ApplicationType = ClientApplicationType.RegularWeb,
            GrantTypes = ["authorization_code", "refresh_token"],
            JwtConfiguration = new JwtConfiguration
            {
                SigningAlgorithm = "RS256",
            },
            OidcConformant = true,
            RefreshToken = new RefreshToken(),
            Callbacks = (client.CallbackUrls ?? []).ToArray(),
            AllowedLogoutUrls = (client.SignOutUrls ?? []).ToArray()
        };

    /// <summary>Get the <see cref="ClientCreateRequest"/> for a client credentials client.</summary>
    /// <param name="client">The access client.</param>
    private static ClientCreateRequest GetMachineClientCreateRequest(ProductClient client) 
        => new()
        {
            IsFirstParty = true, 
            IsCustomLoginPageOn = false,
            Name = client.Name,
            Description = client.Description,
            TokenEndpointAuthMethod= TokenEndpointAuthMethod.ClientSecretBasic,
            ApplicationType = ClientApplicationType.NonInteractive,
            GrantTypes = ["client_credentials"]
        };
}