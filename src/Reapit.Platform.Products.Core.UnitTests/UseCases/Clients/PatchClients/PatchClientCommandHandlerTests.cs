using FluentValidation.Results;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Clients.PatchClient;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Command = Reapit.Platform.Products.Core.UseCases.Clients.PatchClient.PatchClientCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Clients.PatchClients;

public class PatchClientCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly IValidator<Command> _validator = Substitute.For<IValidator<Command>>();
    private readonly FakeLogger<PatchClientCommandHandler> _logger = new();

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
    public async Task Handle_ThrowsNotFoundException_WhenClientNotFound()
    {
        var request = GetRequest();
        
        SetupValidator(true);

        _clientRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.Client?>(null));

        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsWithoutCallingIdP_WhenEntityUnchanged()
    {
        var entity = GetEntity(ClientType.Machine);
        var request = GetRequest(entity.Id, entity.Name, entity.Description, null, []);
        
        SetupValidator(true);

        _clientRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(GetEntity(ClientType.Machine));

        var sut = CreateSut();
        _ = await sut.Handle(request, default);

        await _clientRepository.DidNotReceive().UpdateAsync(Arg.Any<Entities.Client>(), Arg.Any<CancellationToken>());
        await _idpService.DidNotReceive().UpdateMachineClientAsync(Arg.Any<Entities.Client>(), Arg.Any<CancellationToken>());
        await _idpService.DidNotReceive().UpdateAuthCodeClientAsync(Arg.Any<Entities.Client>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_UpdatesMachineClient_WhenEntityTypeIsMachine()
    {
        var entity = GetEntity(ClientType.Machine);
        var request = GetRequest(entity.Id, "a different name", entity.Description, null, []);
        
        SetupValidator(true);

        _clientRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);

        await _clientRepository.Received(1).UpdateAsync(actual, Arg.Any<CancellationToken>());
        await _idpService.Received(1).UpdateMachineClientAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        
        await _idpService.DidNotReceive().UpdateAuthCodeClientAsync(Arg.Any<Entities.Client>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_UpdatesAuthCodeClient_WhenCommandTypeIsAuthCode()
    {
        var entity = GetEntity(ClientType.AuthCode);
        var request = GetRequest(entity.Id, entity.Name, entity.Description, entity.LoginUrl, entity.CallbackUrls, ["https://new-sign-out-url"]);
        
        SetupValidator(true);

        _clientRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);

        await _clientRepository.Received(1).UpdateAsync(actual, Arg.Any<CancellationToken>());
        await _idpService.Received(1).UpdateAuthCodeClientAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        
        await _idpService.DidNotReceive().UpdateMachineClientAsync(Arg.Any<Entities.Client>(), Arg.Any<CancellationToken>());
    }
    
    /*
     * Private method
     */

    private PatchClientCommandHandler CreateSut()
    {
        _unitOfWork.Clients.Returns(_clientRepository);
        return new PatchClientCommandHandler(_unitOfWork, _idpService, _validator, _logger);
    }

    private void SetupValidator(bool isValid)
        => _validator.ValidateAsync(Arg.Any<Command>(), Arg.Any<CancellationToken>())
            .Returns(isValid
                ? new ValidationResult()
                : new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

    private static Entities.Client GetEntity(ClientType type) 
        => new(
            appId: "app-id", 
            externalId: "external-id", 
            type: type, 
            name: "client", 
            description: "description", 
            loginUrl: type == ClientType.AuthCode ? "https://login" : null, 
            callbackUrls: type == ClientType.AuthCode ? ["https://callback"] : null, 
            signOutUrls: type == ClientType.AuthCode ? ["https://sign-out"] : null);

    private static Command GetRequest(
        string id = "app-id",
        string name = "client name",
        string? description = null,
        string? loginUrl = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(id, name, description, loginUrl, callbackUrls, signOutUrls);
}