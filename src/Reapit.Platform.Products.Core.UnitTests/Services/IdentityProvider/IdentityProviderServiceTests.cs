using Auth0.ManagementApi;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Options;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UnitTests.Services.IdentityProvider;

public class IdentityProviderServiceTests
{
    private readonly IIdentityProviderClientFactory _clientFactory = Substitute.For<IIdentityProviderClientFactory>();
    private readonly FakeLogger<IdentityProviderService> _logger = new();
    private readonly IOptions<IdentityProviderOptions> _optionsProvider = Substitute.For<IOptions<IdentityProviderOptions>>();
    private readonly IdentityProviderOptions _options = GetOptions();

    private readonly IManagementApiClient _client = Substitute.For<IManagementApiClient>();
    private readonly IResourceServersClient _resourceServersClient = Substitute.For<IResourceServersClient>();
    private readonly IClientsClient _clientsClient = Substitute.For<IClientsClient>();

    #region Resource Servers
    
    /*
     * CreateResourceServerAsync
     */

    [Fact]
    public async Task CreateResourceServerAsync_ThrowsIdpException_WhenResponseNull()
    {
        var command = new CreateResourceServerCommand("name", "audience", 3600, []);
        var sut = CreateSut();
        var action = () => sut.CreateResourceServerAsync(command, default);
        await action.Should().ThrowAsync<IdentityProviderException>();
    }
    
    [Fact]
    public async Task CreateResourceServerAsync_ReturnsExternalId_WhenSuccessful()
    {
        const string externalId = "created-id";
        var command = new CreateResourceServerCommand("name", "audience", 3600, [ 
            new RequestScopeModel("scope.one", "scope one"),
            new RequestScopeModel("scope.two", "scope two"),
            new RequestScopeModel("scope.three", "scope three")
        ]);

        var expectedRequest = new ResourceServerCreateRequest {
            Name = command.Name,
            Identifier = command.Audience,
            Scopes = command.Scopes.Select(model => model.ToResourceServerScope()).ToList(),
            AllowOfflineAccess = true,
            TokenLifetime = command.TokenLifetime,
            TokenDialect = TokenDialect.AccessToken,
            SkipConsentForVerifiableFirstPartyClients = true,
            SigningAlgorithm = SigningAlgorithm.RS256,
        };

        _resourceServersClient.CreateAsync(
                Arg.Any<ResourceServerCreateRequest>(), 
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<ResourceServerCreateRequest>().Should().BeEquivalentTo(expectedRequest);
                return new ResourceServer { Id = externalId };
            });
        
        var sut = CreateSut();
        var actual = await sut.CreateResourceServerAsync(command, default);
        actual.Should().Be(externalId);
    }
    
    /*
     * UpdateResourceServerAsync
     */

    [Fact]
    public async Task UpdateResourceServerAsync_ThrowsIdpException_WhenResponseNull()
    {
        var entity = new Entities.ResourceServer("external-id", "audience", "name", 3600);
        var sut = CreateSut();
        var action = () => sut.UpdateResourceServerAsync(entity, default);
        await action.Should().ThrowAsync<IdentityProviderException>();
    }
    
    [Fact]
    public async Task UpdateResourceServerAsync_ReturnsTrue_WhenSuccessful()
    {
        var entity = new Entities.ResourceServer("external-id", "audience", "name", 3600);
        entity.Scopes.Add(new Entities.Scope(entity.Id, "test.scope.one", "description"));
        entity.Scopes.Add(new Entities.Scope(entity.Id, "test.scope.two", "description"));
        entity.Scopes.Add(new Entities.Scope(entity.Id, "test.scope.three", "description"));
        
        var expectedRequest = new ResourceServerUpdateRequest {
            Name = entity.Name,
            Scopes = entity.Scopes.Select(scope => new ResourceServerScope { Value = scope.Value, Description = scope.Description }).ToList(),
            TokenLifetime = entity.TokenLifetime
        };

        _resourceServersClient.UpdateAsync(
                Arg.Any<string>(),
                Arg.Any<ResourceServerUpdateRequest>(), 
                Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<ResourceServerUpdateRequest>().Should().BeEquivalentTo(expectedRequest);
                return new ResourceServer { Id = entity.ExternalId };
            });
        
        var sut = CreateSut();
        var actual = await sut.UpdateResourceServerAsync(entity, default);
        actual.Should().BeTrue();
    }
    
    /*
     * DeleteResourceServerAsync
     */
    
    [Fact]
    public async Task DeleteResourceServerAsync_ReturnsTrue_WhenSuccessful()
    {
        var entity = new Entities.ResourceServer("external-id", "audience", "name", 3600);
        var sut = CreateSut();
        var actual = await sut.DeleteResourceServerAsync(entity, default);
        actual.Should().BeTrue();
        
        await _resourceServersClient.Received(1).DeleteAsync(entity.ExternalId, Arg.Any<CancellationToken>());
    }
    
    #endregion
    
    #region Clients
    
    /*
     * CreateAuthCodeClientAsync
     */

    [Fact]
    public async Task CreateAuthCodeClientAsync_ThrowsIdpException_WhenResponseNull()
    {
        const bool isFirstParty = false;
        
        var command = new CreateClientCommand(
            AppId: "appId", 
            Type: ClientType.AuthCode, 
            Name: "name", 
            Description: null, 
            LoginUrl: null, 
            CallbackUrls: null, 
            SignOutUrls: null);

        _clientsClient.CreateAsync(Arg.Any<ClientCreateRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Client?>(null));

        var sut = CreateSut();
        var action = () => sut.CreateAuthCodeClientAsync(command, isFirstParty, default);
        await action.Should().ThrowAsync<IdentityProviderException>();
    }
    
    [Fact]
    public async Task CreateAuthCodeClientAsync_InjectsAllowedLogoutUrl_WhenNullCollectionProvided()
    {
        const string expectedClientId = "clientId";
        const bool isFirstParty = false;
        var expectedLogoutUrls = $"https://{_options.Domain}/login";
        
        var command = new CreateClientCommand(
            AppId: "appId", 
            Type: ClientType.AuthCode, 
            Name: "name", 
            Description: null, 
            LoginUrl: null, 
            CallbackUrls: null, 
            SignOutUrls: null);

        var expected = new ClientCreateRequest
        {
            Name = command.Name,
            Description = null,
            IsFirstParty = isFirstParty,
            Callbacks = null,
            AllowedLogoutUrls = [expectedLogoutUrls],
            InitiateLoginUri = null,
            IsCustomLoginPageOn = false,
            JwtConfiguration = new JwtConfiguration { SigningAlgorithm = "RS256" },
            ApplicationType = ClientApplicationType.RegularWeb,
            GrantTypes = ["authorization_code", "refresh_token"],
            OidcConformant = true,
            RefreshToken = new RefreshToken(),
            TokenEndpointAuthMethod = TokenEndpointAuthMethod.None
        };

        _clientsClient.CreateAsync(Arg.Any<ClientCreateRequest>(), Arg.Any<CancellationToken>())
            .Returns(info =>
            {
                var actual = info.Arg<ClientCreateRequest>();
                actual.Should().BeEquivalentTo(expected);
                return new Client { ClientId = expectedClientId };
            });

        var sut = CreateSut();
        var actual = await sut.CreateAuthCodeClientAsync(command, isFirstParty, default);
        actual.Should().Be(expectedClientId);
    }
    
    [Fact]
    public async Task CreateAuthCodeClientAsync_InjectsAllowedLogoutUrl_WhenPopulatedCollectionProvided()
    {
        const string expectedClientId = "clientId";
        const bool isFirstParty = false;
        var expectedLogoutUrls = $"https://{_options.Domain}/login";
        
        var command = new CreateClientCommand(
            AppId: "appId", 
            Type: ClientType.AuthCode, 
            Name: "name", 
            Description: "description", 
            LoginUrl: "https://www.login-url.com", 
            CallbackUrls: ["https://www.callback.com"], 
            SignOutUrls: ["https://www.signout.com"]); // Change from previous test

        var expected = new ClientCreateRequest
        {
            Name = command.Name,
            Description = command.Description,
            IsFirstParty = isFirstParty,
            Callbacks = command.CallbackUrls?.ToArray(),
            AllowedLogoutUrls = [command.SignOutUrls!.Single(), expectedLogoutUrls], // Change from previous test
            InitiateLoginUri = command.LoginUrl,
            IsCustomLoginPageOn = false,
            JwtConfiguration = new JwtConfiguration { SigningAlgorithm = "RS256" },
            ApplicationType = ClientApplicationType.RegularWeb,
            GrantTypes = ["authorization_code", "refresh_token"],
            OidcConformant = true,
            RefreshToken = new RefreshToken(),
            TokenEndpointAuthMethod = TokenEndpointAuthMethod.None
        };

        _clientsClient.CreateAsync(Arg.Any<ClientCreateRequest>(), Arg.Any<CancellationToken>())
            .Returns(info =>
            {
                var actual = info.Arg<ClientCreateRequest>();
                actual.Should().BeEquivalentTo(expected);
                return new Client { ClientId = expectedClientId };
            });

        var sut = CreateSut();
        var actual = await sut.CreateAuthCodeClientAsync(command, isFirstParty, default);
        actual.Should().Be(expectedClientId);
    }
    
    [Fact]
    public async Task CreateAuthCodeClientAsync_DoesNotDuplicateAllowedLogoutUrl_WhenRequiredValueAlreadyInCollection()
    {
        const string expectedClientId = "clientId";
        const bool isFirstParty = false;
        var expectedLogoutUrl = $"https://{_options.Domain}/login";
        
        var command = new CreateClientCommand(
            AppId: "appId", 
            Type: ClientType.AuthCode, 
            Name: "name", 
            Description: "description", 
            LoginUrl: "https://www.login-url.com", 
            CallbackUrls: ["https://www.callback.com"], 
            SignOutUrls: ["https://www.signout.com", expectedLogoutUrl]); // Change from previous test

        var expected = new ClientCreateRequest
        {
            Name = command.Name,
            Description = command.Description,
            IsFirstParty = isFirstParty,
            Callbacks = command.CallbackUrls?.ToArray(),
            AllowedLogoutUrls = command.SignOutUrls?.ToArray(), // Change from previous test
            InitiateLoginUri = command.LoginUrl,
            IsCustomLoginPageOn = false,
            JwtConfiguration = new JwtConfiguration { SigningAlgorithm = "RS256" },
            ApplicationType = ClientApplicationType.RegularWeb,
            GrantTypes = ["authorization_code", "refresh_token"],
            OidcConformant = true,
            RefreshToken = new RefreshToken(),
            TokenEndpointAuthMethod = TokenEndpointAuthMethod.None
        };

        _clientsClient.CreateAsync(Arg.Any<ClientCreateRequest>(), Arg.Any<CancellationToken>())
            .Returns(info =>
            {
                var actual = info.Arg<ClientCreateRequest>();
                actual.Should().BeEquivalentTo(expected);
                return new Client { ClientId = expectedClientId };
            });

        var sut = CreateSut();
        var actual = await sut.CreateAuthCodeClientAsync(command, isFirstParty, default);
        actual.Should().Be(expectedClientId);
    }
    
    /*
     * CreateMachineClientAsync
     */
    
    [Fact]
    public async Task CreateMachineClientAsync_ThrowsIdpException_WhenResponseNull()
    {
        const bool isFirstParty = false;
        
        var command = new CreateClientCommand(
            AppId: "appId", 
            Type: ClientType.Machine, 
            Name: "name", 
            Description: null, 
            LoginUrl: null, 
            CallbackUrls: null, 
            SignOutUrls: null);

        _clientsClient.CreateAsync(Arg.Any<ClientCreateRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Client?>(null));

        var sut = CreateSut();
        var action = () => sut.CreateMachineClientAsync(command, isFirstParty, default);
        await action.Should().ThrowAsync<IdentityProviderException>();
    }
    
    [Fact]
    public async Task CreateMachineClientAsync_ReturnsExternalId_WhenSuccessful()
    {
        const string expectedClientId = "expected-client-id";
        const bool isFirstParty = false;
        
        var command = new CreateClientCommand(
            AppId: "appId", 
            Type: ClientType.Machine, 
            Name: "name", 
            Description: "this is a description", 
            LoginUrl: null, 
            CallbackUrls: null, 
            SignOutUrls: null);
        
        var expectedRequest = new ClientCreateRequest
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

        _clientsClient.CreateAsync(Arg.Any<ClientCreateRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var actualRequest = callInfo.Arg<ClientCreateRequest>();
                actualRequest.Should().BeEquivalentTo(expectedRequest);
                return new Client { ClientId = expectedClientId };
            });

        var sut = CreateSut();
        var actual = await sut.CreateMachineClientAsync(command, isFirstParty, default);
        actual.Should().BeEquivalentTo(expectedClientId);
    }
    
    /*
     * UpdateAuthCodeClientAsync
     */
    
    [Fact]
    public async Task UpdateAuthCodeClientAsync_ThrowsIdpException_WhenResponseNull()
    {
        var command = new Entities.Client(
            appId: "appId",
            externalId: "external-identifier",
            type: ClientType.AuthCode, 
            name: "updated-name", 
            description: "updated-description", 
            loginUrl: null, 
            callbackUrls: null, 
            signOutUrls: null);

        var sut = CreateSut();
        var action = () => sut.UpdateAuthCodeClientAsync(command, default);
        await action.Should().ThrowAsync<IdentityProviderException>();
    }
    
    [Fact]
    public async Task UpdateAuthCodeClientAsync_InjectsRequiredLogoutUrl_WhenCollectionNull()
    {
        var expectedLogoutUrl = $"https://{_options.Domain}/login";
        
        var entity = new Entities.Client(
            appId: "appId",
            externalId: "external-identifier",
            type: ClientType.AuthCode, 
            name: "updated-name", 
            description: "updated-description", 
            loginUrl: null, 
            callbackUrls: null, 
            signOutUrls: null);

        var expectedRequest = new ClientUpdateRequest
        {
            Name = entity.Name,
            Description = entity.Description,
            AllowedLogoutUrls = [expectedLogoutUrl],
            InitiateLoginUri = entity.LoginUrl,
            Callbacks = entity.CallbackUrls?.ToArray()
        };

        _clientsClient.UpdateAsync(entity.ExternalId, Arg.Any<ClientUpdateRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<ClientUpdateRequest>().Should().BeEquivalentTo(expectedRequest);
                return new Client();
            });

        var sut = CreateSut();
        var actual = await sut.UpdateAuthCodeClientAsync(entity, default);
        actual.Should().BeTrue();
    }
    
    [Fact]
    public async Task UpdateAuthCodeClientAsync_AppendsRequiredLogoutUrl_WhenCollectionNotNull()
    {
        var expectedLogoutUrl = $"https://{_options.Domain}/login";
        
        var entity = new Entities.Client(
            appId: "appId",
            externalId: "external-identifier",
            type: ClientType.AuthCode, 
            name: "updated-name", 
            description: "updated-description", 
            loginUrl: "https://login", 
            callbackUrls: ["https://callback"], 
            signOutUrls: ["https://signOut"]);

        var expectedRequest = new ClientUpdateRequest
        {
            Name = entity.Name,
            Description = entity.Description,
            AllowedLogoutUrls = [entity.SignOutUrls!.Single(), expectedLogoutUrl],
            InitiateLoginUri = entity.LoginUrl,
            Callbacks = entity.CallbackUrls?.ToArray()
        };

        _clientsClient.UpdateAsync(entity.ExternalId, Arg.Any<ClientUpdateRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<ClientUpdateRequest>().Should().BeEquivalentTo(expectedRequest);
                return new Client();
            });

        var sut = CreateSut();
        var actual = await sut.UpdateAuthCodeClientAsync(entity, default);
        actual.Should().BeTrue();
    }
    
    [Fact]
    public async Task UpdateAuthCodeClientAsync_DoesNotAppendRequiredLogoutUrl_WhenAlreadyInCollection()
    {
        var expectedLogoutUrl = $"https://{_options.Domain}/login";
        
        var entity = new Entities.Client(
            appId: "appId",
            externalId: "external-identifier",
            type: ClientType.AuthCode, 
            name: "updated-name", 
            description: "updated-description", 
            loginUrl: "https://login", 
            callbackUrls: ["https://callback"], 
            signOutUrls: ["https://signOut", expectedLogoutUrl]);

        var expectedRequest = new ClientUpdateRequest
        {
            Name = entity.Name,
            Description = entity.Description,
            AllowedLogoutUrls = entity.SignOutUrls?.ToArray(),
            InitiateLoginUri = entity.LoginUrl,
            Callbacks = entity.CallbackUrls?.ToArray()
        };

        _clientsClient.UpdateAsync(entity.ExternalId, Arg.Any<ClientUpdateRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<ClientUpdateRequest>().Should().BeEquivalentTo(expectedRequest);
                return new Client();
            });

        var sut = CreateSut();
        var actual = await sut.UpdateAuthCodeClientAsync(entity, default);
        actual.Should().BeTrue();
    }
    
    /*
     * UpdateMachineClientAsync
     */
    
    [Fact]
    public async Task UpdateMachineClientAsync_ThrowsIdpException_WhenResponseNull()
    {
        var command = new Entities.Client(
            appId: "appId",
            externalId: "external-identifier",
            type: ClientType.Machine, 
            name: "updated-name", 
            description: "updated-description", 
            loginUrl: null, 
            callbackUrls: null, 
            signOutUrls: null);
        
        var sut = CreateSut();
        var action = () => sut.UpdateMachineClientAsync(command, default);
        await action.Should().ThrowAsync<IdentityProviderException>();
    }
    
    [Fact]
    public async Task UpdateMachineClientAsync_ReturnsTrue_WhenSuccessful()
    {
        var entity = new Entities.Client(
            appId: "appId",
            externalId: "external-identifier",
            type: ClientType.Machine,
            name: "updated-name",
            description: "updated-description",
            loginUrl: null,
            callbackUrls: null,
            signOutUrls: null);
        
        var expectedRequest = new ClientUpdateRequest
        {
            Name = entity.Name,
            Description = entity.Description
        };

        _clientsClient.UpdateAsync(entity.ExternalId, Arg.Any<ClientUpdateRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<ClientUpdateRequest>().Should().BeEquivalentTo(expectedRequest);
                return new Client();
            });

        var sut = CreateSut();
        var actual = await sut.UpdateMachineClientAsync(entity, default);
        actual.Should().BeTrue();
    }
    
    /*
     * DeleteClientsAsync
     */
    
    [Fact]
    public async Task DeleteClientsAsync_ReturnsTrue_WhenSuccessful()
    {
        var entity = new Entities.Client(string.Empty, "relevant-value", ClientType.Machine, string.Empty, null, null, null, null);
        var sut = CreateSut();
        var actual = await sut.DeleteClientAsync(entity, default);
        actual.Should().BeTrue();
        
        await _clientsClient.Received(1).DeleteAsync(entity.ExternalId, Arg.Any<CancellationToken>());
    }
    
    #endregion

    /*
     * Private methods
     */

    private IdentityProviderService CreateSut()
    {
        _client.ResourceServers.Returns(_resourceServersClient);
        _client.Clients.Returns(_clientsClient);
        _clientFactory.GetClientAsync(Arg.Any<CancellationToken>()).Returns(_client);

        _optionsProvider.Value.Returns(_options);

        return new IdentityProviderService(_clientFactory, _optionsProvider, _logger);
    }

    private static IdentityProviderOptions GetOptions()
        => new()
        {
            Domain = "reapit-test.auth0.com",
            ClientId = "management-api-client",
            ClientSecret = "management-api-client-secret",
            TokenCacheSeconds = 86400,
        };
}