using FluentValidation.Results;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Grants.CreateGrant;

public class CreateGrantCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IResourceServerRepository _resourceServerRepository = Substitute.For<IResourceServerRepository>();
    private readonly IGrantRepository _grantRepository = Substitute.For<IGrantRepository>();
    private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();

    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly IValidator<CreateGrantCommand> _validator = Substitute.For<IValidator<CreateGrantCommand>>();
    private readonly FakeLogger<CreateGrantCommandHandler> _logger = new();

    private const string DefaultScope = "scope.default";
    
    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        var request = GetRequest();
        SetupValidator(false);

        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ThrowsConflictException_WhenClientAlreadyGrantedAccess()
    {
        var request = GetRequest();
        SetupValidator(true);
        
        _resourceServerRepository.GetByIdAsync(request.ResourceServerId, Arg.Any<CancellationToken>())
            .Returns(GetResourceServer());

        var client = GetClient();
        client.Grants.Add(new Entities.Grant("", client.Id, request.ResourceServerId){ Client = default!, ResourceServer = default! });
        _clientRepository.GetByIdAsync(request.ClientId, Arg.Any<CancellationToken>())
            .Returns(client);
        
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ConflictException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenGrantCreated()
    {
        var request = GetRequest(scopes: ["one", "three"]);
        SetupValidator(true);

        var resourceServer = GetResourceServer(scopes: ["one", "two", "three", "four"]);
        _resourceServerRepository.GetByIdAsync(request.ResourceServerId, Arg.Any<CancellationToken>())
            .Returns(resourceServer);

        var client = GetClient();
        _clientRepository.GetByIdAsync(request.ClientId, Arg.Any<CancellationToken>())
            .Returns(client);

        const string externalId = "external-id";
        _idpService.CreateGrantAsync(request, client, resourceServer, Arg.Any<CancellationToken>())
            .Returns(externalId);

        var guid = Guid.NewGuid();
        using var guidContext = new GuidProviderContext(guid);
        using var dateContext = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);

        var expected = new Entities.Grant(externalId, client.Id, resourceServer.Id)
        {
            Client = client,
            ResourceServer = resourceServer,
            Scopes = resourceServer.Scopes.Where(scope => request.Scopes.Contains(scope.Value)).ToList()
        };
        
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(expected);

        await _grantRepository.Received(1).CreateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private CreateGrantCommandHandler CreateSut()
    {
        _unitOfWork.Clients.Returns(_clientRepository);
        _unitOfWork.ResourceServers.Returns(_resourceServerRepository);
        _unitOfWork.Grants.Returns(_grantRepository);
        return new CreateGrantCommandHandler(_unitOfWork, _idpService, _validator, _logger);
    }
    
    private static CreateGrantCommand GetRequest(
        string clientId = "client-id",
        string resourceServerId = "resource-server-id",
        string[]? scopes = null)
        => new(clientId, resourceServerId, scopes ?? []);
    
    private void SetupValidator(bool isValid)
        => _validator.ValidateAsync(Arg.Any<CreateGrantCommand>(), Arg.Any<CancellationToken>())
            .Returns(isValid
                ? new ValidationResult()
                : new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

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