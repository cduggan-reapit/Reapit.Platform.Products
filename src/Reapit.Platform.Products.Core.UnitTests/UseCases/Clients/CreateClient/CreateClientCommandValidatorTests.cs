using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Clients;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Command = Reapit.Platform.Products.Core.UseCases.Clients.CreateClient.CreateClientCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Clients.CreateClient;

public class CreateClientCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
    private readonly IAppRepository _appRepository = Substitute.For<IAppRepository>();

    private const string AppId = "application-id";
    
    [Fact]
    public async Task Validation_ReturnsSuccess_ForValidMachineRequest()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetMachineRequest();
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validation_ReturnsSuccess_ForValidAuthCodeRequest()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetAuthCodeRequest(["https://example.callback"], ["https://example.signout"]);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    #region Common Validators

    /*
     * AppId
     */

    [Fact]
    public async Task Validation_ReturnsFailure_WhenApplicationDoesNotExist()
    {
        SetupAppRepository(exists: false);
        SetupClientRepository();
        
        var request = GetMachineRequest();
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.AppId), ClientValidationMessages.ApplicationNotFound);
    }
    
    /*
     * Name
     */
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenNameEmpty()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetMachineRequest(name: "");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenNameTooLong()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetMachineRequest(name: new string('a', 101));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ClientValidationMessages.NameTooLong);
    }
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenNameInvalid()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetMachineRequest(name: ">name");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ClientValidationMessages.NameInvalid);
    }
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenNameUnavailable()
    {
        SetupAppRepository();
        SetupClientRepository(isUnique: false);
        
        var request = GetMachineRequest();
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), CommonValidationMessages.Unique);
    }
    
    /*
     * Type
     */
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenTypeInvalid()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetRequest(type: "");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Type), ClientValidationMessages.TypeInvalid);
    }
    
    /*
     * Description
     */
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenDescriptionTooLong()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetRequest(description: new string('b', 141));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Description), ClientValidationMessages.DescriptionTooLong);
    }
    
    #endregion

    #region Machine Client Validators

    [Fact]
    public async Task Validation_ReturnsFailure_WhenLoginUrlProvided_AndTypeIsMachine()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetMachineRequest(loginUrl: "https://loginurl");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.LoginUrl), ClientValidationMessages.NotSupportedByMachineClients);
    }
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenCallbackUrlsProvided_AndTypeIsMachine()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetMachineRequest(callbackUrls: ["https://callback"]);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.CallbackUrls), ClientValidationMessages.NotSupportedByMachineClients);
    }
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenSignOutUrlProvided_AndTypeIsMachine()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetMachineRequest(signOutUrls: ["https://sign-out"]);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.SignOutUrls), ClientValidationMessages.NotSupportedByMachineClients);
    }

    #endregion

    #region AuthCode Validators

    [Fact]
    public async Task Validation_ReturnsFailure_WhenLoginUrlNotProvided_AndTypeIsAuthCode()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetAuthCodeRequest(["https://callback"], ["https://sign-out"], loginUrl: null);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.LoginUrl), ClientValidationMessages.RequiredByAuthCodeClients);
    }
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenLoginUrlNotSecure_AndTypeIsAuthCode()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetAuthCodeRequest(["https://callback"], ["https://sign-out"], loginUrl: "http://insecure");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.LoginUrl), ClientValidationMessages.LoginUrlMustBeHttps);
    }
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenCallbackUrlsNotProvided_AndTypeIsAuthCode()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetAuthCodeRequest([], ["https://sign-out"], loginUrl: "https://logout");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.CallbackUrls), ClientValidationMessages.RequiredByAuthCodeClients);
    }
    
    [Fact]
    public async Task Validation_ReturnsFailure_WhenSignOutUrlNotProvided_AndTypeIsAuthCode()
    {
        SetupAppRepository();
        SetupClientRepository();
        
        var request = GetAuthCodeRequest(["https://callback"], null, loginUrl: "https://logout");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.SignOutUrls), ClientValidationMessages.RequiredByAuthCodeClients);
    }

    #endregion
    
    /*
     * Private methods
     */

    private CreateClientCommandValidator CreateSut()
    {
        _unitOfWork.Apps.Returns(_appRepository);
        _unitOfWork.Clients.Returns(_clientRepository);
        return new CreateClientCommandValidator(_unitOfWork);
    }

    private void SetupAppRepository(string appId = AppId, bool exists = true) 
        => _appRepository.GetByIdAsync(appId, Arg.Any<CancellationToken>())
            .Returns(exists 
                ? new Entities.App("name", "description", true) 
                : null);

    private void SetupClientRepository(string? name = "client name", bool isUnique = true)
        => _clientRepository.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(isUnique 
                ? Array.Empty<Entities.Client>() 
                : [new Entities.Client(string.Empty, string.Empty, ClientType.Machine, string.Empty, null, null, null, null)]);
    
    private static Command GetRequest(
        string type = "machine",
        string appId = AppId,
        string name = "client name",
        string? description = null,
        string? loginUrl = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(appId, type, name, description, loginUrl, callbackUrls, signOutUrls);
    
    private static Command GetMachineRequest(
        string appId = AppId,
        string name = "client name",
        string? description = null,
        string? loginUrl = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(appId, "machine", name, description, loginUrl, callbackUrls, signOutUrls);
    
    private static Command GetAuthCodeRequest(
        ICollection<string>? callbackUrls,
        ICollection<string>? signOutUrls,
        string appId = AppId,
        string name = "client name",
        string? description = null,
        string? loginUrl = "https://www.example.net/login")
        => new(appId, "authCode", name, description, loginUrl, callbackUrls, signOutUrls);
}