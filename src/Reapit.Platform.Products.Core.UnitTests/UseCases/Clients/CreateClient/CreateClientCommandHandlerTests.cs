using FluentValidation.Results;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Command = Reapit.Platform.Products.Core.UseCases.Clients.CreateClient.CreateClientCommand;


namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Clients.CreateClient;

public class CreateClientCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
    private readonly IAppRepository _appRepository = Substitute.For<IAppRepository>();
    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly IValidator<Command> _validator = Substitute.For<IValidator<Command>>();
    private readonly FakeLogger<CreateClientCommandHandler> _logger = new();

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
    public async Task Handle_ThrowsNotFoundException_WhenAppNotFound()
    {
        var request = GetRequest();
        
        SetupValidator(true);

        _appRepository.GetByIdAsync(request.AppId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.App?>(null));

        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_CreatesMachineClient_WhenCommandTypeIsMachine()
    {
        const bool isFirstParty = true;
        const string clientId = "client-id";
        var request = GetRequest();
        
        SetupValidator(true);

        _appRepository.GetByIdAsync(request.AppId, Arg.Any<CancellationToken>())
            .Returns(new Entities.App("", "", isFirstParty));

        _idpService.CreateMachineClientAsync(request, isFirstParty, Arg.Any<CancellationToken>())
            .Returns(clientId);

        using var guidProvider = new GuidProviderContext(Guid.NewGuid());
        using var timeProvider = new DateTimeOffsetProviderContext(DateTimeOffset.UtcNow);

        var expected = new Entities.Client(
            appId: request.AppId, 
            externalId: clientId, 
            type: ClientType.Machine, 
            name: request.Name, 
            description: request.Description,
            loginUrl: request.LoginUrl, 
            callbackUrls: request.CallbackUrls, 
            signOutUrls: request.SignOutUrls); 
        
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(expected);

        await _clientRepository.Received(1).CreateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_CreatesMachineClient_WhenCommandTypeIsAuthCode()
    {
        const bool isFirstParty = true;
        const string clientId = "client-id";
        var request = GetRequest(
            type: ClientType.AuthCode,
            loginUrl: "https://loginurl",
            callbackUrls: ["https://callback"],
            signOutUrls: ["https://sign-out"]);
        
        SetupValidator(true);

        _appRepository.GetByIdAsync(request.AppId, Arg.Any<CancellationToken>())
            .Returns(new Entities.App("", "", isFirstParty));

        _idpService.CreateAuthCodeClientAsync(request, isFirstParty, Arg.Any<CancellationToken>())
            .Returns(clientId);

        using var guidProvider = new GuidProviderContext(Guid.NewGuid());
        using var timeProvider = new DateTimeOffsetProviderContext(DateTimeOffset.UtcNow);

        var expected = new Entities.Client(
            appId: request.AppId, 
            externalId: clientId, 
            type: ClientType.AuthCode, 
            name: request.Name, 
            description: request.Description,
            loginUrl: request.LoginUrl, 
            callbackUrls: request.CallbackUrls, 
            signOutUrls: request.SignOutUrls); 
        
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(expected);

        await _clientRepository.Received(1).CreateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private method
     */

    private CreateClientCommandHandler CreateSut()
    {
        _unitOfWork.Apps.Returns(_appRepository);
        _unitOfWork.Clients.Returns(_clientRepository);
        return new CreateClientCommandHandler(_unitOfWork, _idpService, _validator, _logger);
    }

    private void SetupValidator(bool isValid)
        => _validator.ValidateAsync(Arg.Any<Command>(), Arg.Any<CancellationToken>())
            .Returns(isValid
                ? new ValidationResult()
                : new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));
    
    private static Command GetRequest(
        string type = "machine",
        string appId = "app-id",
        string name = "client name",
        string? description = null,
        string? loginUrl = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(appId, type, name, description, loginUrl, callbackUrls, signOutUrls);
}