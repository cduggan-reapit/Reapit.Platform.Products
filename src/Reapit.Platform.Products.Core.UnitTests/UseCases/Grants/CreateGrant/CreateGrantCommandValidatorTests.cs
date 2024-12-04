using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Grants;
using Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Grants.CreateGrant;

public class CreateGrantCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IResourceServerRepository _resourceServerRepository = Substitute.For<IResourceServerRepository>();
    private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();

    private const string DefaultScope = "scope.default";
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenRequestValid()
    {
        var request = GetRequest(scopes: [DefaultScope]);
        SetupClient(request.ClientId, true);
        SetupApi(request.ResourceServerId, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    /*
     * ClientId
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenClientIdEmpty()
    {
        var request = GetRequest(clientId: "");
        SetupApi(request.ResourceServerId, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGrantCommand.ClientId), CommonValidationMessages.Required);

        // Don't need to check for the clients presence when it's not set
        await _clientRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(request.ClientId, default);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenClientDoesNotExist()
    {
        var request = GetRequest();
        SetupClient(request.ClientId, false);
        SetupApi(request.ResourceServerId, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGrantCommand.ClientId), GrantValidationMessages.ClientNotFound);
    }
    
    /*
     * ResourceServerId
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenResourceServerIdEmpty()
    {
        var request = GetRequest(resourceServerId: "");
        SetupClient(request.ClientId, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGrantCommand.ResourceServerId), CommonValidationMessages.Required);

        // Don't need to check for the resource servers presence when it's not set
        await _resourceServerRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(request.ResourceServerId, default);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenResourceServerDoesNotExist()
    {
        var request = GetRequest();
        SetupClient(request.ClientId, true);
        SetupApi(request.ResourceServerId, false);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(CreateGrantCommand.ResourceServerId), GrantValidationMessages.ResourceServerNotFound);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenResourceServerDoesNotSupportScope()
    {
        var request = GetRequest(scopes: [DefaultScope, "scope.unsupported"]);
        SetupClient(request.ClientId, true);
        SetupApi(request.ResourceServerId, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        
        // We know this will be Scopes[1]
        result.Should().Fail("Scopes[1]", GrantValidationMessages.UnsupportedScope);
    }
    
    /*
     * Private methods
     */

    private CreateGrantCommandValidator CreateSut()
    {
        _unitOfWork.ResourceServers.Returns(_resourceServerRepository);
        _unitOfWork.Clients.Returns(_clientRepository);
        return new CreateGrantCommandValidator(_unitOfWork);
    }

    private static CreateGrantCommand GetRequest(
        string clientId = "client-id",
        string resourceServerId = "resource-server-id",
        string[]? scopes = null)
        => new(clientId, resourceServerId, scopes ?? []);
    
    private void SetupClient(string id, bool exists)
        => _clientRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(exists ? GetClient() : null);
    
    private void SetupApi(string id, bool exists)
        => _resourceServerRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(exists ? GetResourceServer() : null);

    private static Entities.Client GetClient()
        => new("irrelevant", "irrelevant", ClientType.Machine, "irrelevant", null, null, null, null);
    
    private static Entities.ResourceServer GetResourceServer(string[]? scopes = null)
    {
        var entity = new Entities.ResourceServer("irrelevant", "irrelevant", "irrelevant", -1);
        
        var scopesToSet = (scopes ?? [DefaultScope])
            .Select(scope => new Entities.Scope(entity.Id, scope, null))
            .ToList();
        
        entity.SetScopes(scopesToSet);

        return entity;
    }
}