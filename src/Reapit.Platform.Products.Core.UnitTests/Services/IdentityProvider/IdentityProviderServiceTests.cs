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
     * UpdateResourceServerAsync
     */

    [Fact]
    public async Task UpdateResourceServerAsync_ThrowsIdpException_WhenResponseNull()
    {
        var entity = new Domain.Entities.ResourceServer("external-id", "audience", "name", 3600);
        var sut = CreateSut();
        var action = () => sut.UpdateResourceServerAsync(entity, default);
        await action.Should().ThrowAsync<IdentityProviderException>();
    }
    
    [Fact]
    public async Task UpdateResourceServerAsync_ReturnsTrue_WhenSuccessful()
    {
        var entity = new Domain.Entities.ResourceServer("external-id", "audience", "name", 3600);

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
        var entity = new Domain.Entities.ResourceServer("external-id", "audience", "name", 3600);
        var sut = CreateSut();
        var actual = await sut.DeleteResourceServerAsync(entity, default);
        actual.Should().BeTrue();
        
        await _resourceServersClient.Received(1).DeleteAsync(entity.ExternalId, Arg.Any<CancellationToken>());
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
}