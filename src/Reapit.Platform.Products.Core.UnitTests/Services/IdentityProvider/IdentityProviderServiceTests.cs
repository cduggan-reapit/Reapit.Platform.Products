using Auth0.ManagementApi;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.Shared;

namespace Reapit.Platform.Products.Core.UnitTests.Services.IdentityProvider;

public class IdentityProviderServiceTests
{
    private readonly IIdentityProviderClientFactory _clientFactory = Substitute.For<IIdentityProviderClientFactory>();
    private readonly FakeLogger<IdentityProviderService> _logger = new();

    private readonly IManagementApiClient _client = Substitute.For<IManagementApiClient>();
    private readonly IResourceServersClient _resourceServersClient = Substitute.For<IResourceServersClient>();

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
            new ResourceServerRequestScopeModel("scope.one", "scope one"),
            new ResourceServerRequestScopeModel("scope.two", "scope two"),
            new ResourceServerRequestScopeModel("scope.three", "scope three")
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
     * Private methods
     */

    private IdentityProviderService CreateSut()
    {
        _client.ResourceServers.Returns(_resourceServersClient);
        _clientFactory.GetClientAsync(Arg.Any<CancellationToken>()).Returns(_client);

        return new IdentityProviderService(_clientFactory, _logger);
    }

    /*IIdentityProviderClientFactory clientFactory, ILogger<IdentityProviderService> logger*/
}