using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Clients;
using Reapit.Platform.Products.Core.UseCases.Clients.PatchClient;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Command = Reapit.Platform.Products.Core.UseCases.Clients.PatchClient.PatchClientCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Clients.PatchClients;

public class PatchClientCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IClientRepository _repository = Substitute.For<IClientRepository>();

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenAllPropertiesNull_ForAuthCodeClient()
    {
        const string id = "identifier";

        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(GetEntity(type: ClientType.AuthCode));
        
        var request = GetRequest(id);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenRequestValid_ForAuthCodeClient()
    {
        const string id = "identifier",
            name = "new name";

        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(GetEntity(type: ClientType.AuthCode, name: "old name"));
        
        SetupUniqueCheck(name, true);
        
        var request = GetRequest(
            id, 
            name: name, 
            description: "new description", 
            loginUrl: "https://valid.login",
            callbackUrls: ["http://callback"],
            signOutUrls: ["http://sign-out"]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenAllPropertiesNull_ForMachineClient()
    {
        const string id = "identifier";

        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(GetEntity(type: ClientType.Machine));
        
        var request = GetRequest(id);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenRequestValid_ForMachineClient()
    {
        const string id = "identifier",
            name = "new name";

        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(GetEntity(type: ClientType.Machine, name: "old name"));
        
        SetupUniqueCheck(name, true);
        
        var request = GetRequest(
            id, 
            name: name, 
            description: "new description", 
            loginUrl: "",
            callbackUrls: [],
            signOutUrls: []);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    /*
     * Shared validation
     */

    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameEmpty()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine));
        
        var request = GetRequest(id, name: "");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), CommonValidationMessages.NotEmpty);
        
        // We expect a database request as part of the When() clauses, but shouldn't hit GetAsync
        await _repository.Received(1).GetByIdAsync(id, default);
        await _repository.DidNotReceiveWithAnyArgs().GetAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameTooLong()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine));
        
        var request = GetRequest(id, name: new string('a', 101));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ClientValidationMessages.NameTooLong);
        
        // We expect a database request as part of the When() clauses, but shouldn't hit GetAsync
        await _repository.Received(1).GetByIdAsync(id, default);
        await _repository.DidNotReceiveWithAnyArgs().GetAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameContainsInvalidCharacters()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine));
        
        var request = GetRequest(id, name: "< name");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ClientValidationMessages.NameInvalid);
        
        // We expect a database request as part of the When() clauses, but shouldn't hit GetAsync
        await _repository.Received(1).GetByIdAsync(id, default);
        await _repository.DidNotReceiveWithAnyArgs().GetAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameNotUnique()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine, name: "old name"));
        SetupUniqueCheck("conflict", false);
        
        var request = GetRequest(id, name: "conflict");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), CommonValidationMessages.Unique);
        
        // We expect a database request as part of the When() clauses, but shouldn't hit GetAsync
        await _repository.Received(1).GetByIdAsync(id, default);
        await _repository.ReceivedWithAnyArgs(1).GetAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameChanged_ButEntityNotFound()
    {
        const string id = "identifier";
        SetupGetById(id, null);
        SetupUniqueCheck("conflict", false);
        
        var request = GetRequest(id, name: "conflict");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameUnchanged()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine, name: "matches"));
        SetupUniqueCheck("matches", false);
        
        var request = GetRequest(id, name: "matches");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
        
        // We expect a database request as part of the When() clauses, but shouldn't hit GetAsync
        await _repository.Received(1).GetByIdAsync(id, default);
        await _repository.DidNotReceiveWithAnyArgs().GetAsync();
    }
    
    /*
     * Description
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenDescriptionTooLong()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine));
        
        var request = GetRequest(id, description: new string('a', 141));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Description), ClientValidationMessages.DescriptionTooLong);
    }
    
    /*
     * Machine client
     */

    [Fact]
    public async Task Validate_ReturnsFailure_WhenLoginUrlProvided_ForMachineClient()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine));
        
        var request = GetRequest(id, loginUrl: "https://login-url");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.LoginUrl), ClientValidationMessages.NotSupportedByMachineClients);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenCallbackUrlProvided_ForMachineClient()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine));
        
        var request = GetRequest(id, callbackUrls: ["https://callback-url"]);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.CallbackUrls), ClientValidationMessages.NotSupportedByMachineClients);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenSignOutUrlProvided_ForMachineClient()
    {
        const string id = "identifier";
        SetupGetById(id, GetEntity(ClientType.Machine));
        
        var request = GetRequest(id, signOutUrls: ["https://sign-out-url"]);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.SignOutUrls), ClientValidationMessages.NotSupportedByMachineClients);
    }
    
   /*
    * AuthCode Client
    */
   
   [Fact]
   public async Task Validate_ReturnsFailure_WhenLoginUrlEmpty_ForAuthCodeClient()
   {
       const string id = "identifier";
       SetupGetById(id, GetEntity(ClientType.AuthCode));
        
       var request = GetRequest(id, loginUrl: "");
       var sut = CreateSut();
       var result = await sut.ValidateAsync(request);
       result.Should().Fail(nameof(Command.LoginUrl), ClientValidationMessages.RequiredByAuthCodeClients);
   }
   
   [Fact]
   public async Task Validate_ReturnsFailure_WhenLoginUrlNotSecure_ForAuthCodeClient()
   {
       const string id = "identifier";
       SetupGetById(id, GetEntity(ClientType.AuthCode));
        
       var request = GetRequest(id, loginUrl: "http://insecure.url");
       var sut = CreateSut();
       var result = await sut.ValidateAsync(request);
       result.Should().Fail(nameof(Command.LoginUrl), ClientValidationMessages.LoginUrlMustBeHttps);
   }
    
   [Fact]
   public async Task Validate_ReturnsFailure_WhenCallbackUrlEmpty_ForAuthCodeClient()
   {
       const string id = "identifier";
       SetupGetById(id, GetEntity(ClientType.AuthCode));
        
       var request = GetRequest(id, callbackUrls: []);
       var sut = CreateSut();
       var result = await sut.ValidateAsync(request);
       result.Should().Fail(nameof(Command.CallbackUrls), ClientValidationMessages.RequiredByAuthCodeClients);
   }
    
   [Fact]
   public async Task Validate_ReturnsFailure_WhenSignOutUrlEmpty_ForAuthCodeClient()
   {
       const string id = "identifier";
       SetupGetById(id, GetEntity(ClientType.AuthCode));
        
       var request = GetRequest(id, signOutUrls: []);
       var sut = CreateSut();
       var result = await sut.ValidateAsync(request);
       result.Should().Fail(nameof(Command.SignOutUrls), ClientValidationMessages.RequiredByAuthCodeClients);
   }
   
    /*
     * Private methods
     */
    
    private PatchClientCommandValidator CreateSut()
    {
        _unitOfWork.Clients.Returns(_repository);
        return new PatchClientCommandValidator(_unitOfWork);
    }

    // Most properties don't matter
    //  - Name matters (because it gets compared to the request name)
    //  - Type matters (because it decides which route things go down)
    private static Entities.Client GetEntity(ClientType type, string name = "client name") 
        => new(
            appId: string.Empty, 
            externalId: string.Empty, 
            type: type, 
            name: name, 
            description: null, 
            loginUrl: null, 
            callbackUrls: null, 
            signOutUrls: null);

    private void SetupGetById(string id, Entities.Client? client)
        => _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(client);
    
    private void SetupUniqueCheck(string name, bool isUnique)
     => _repository.GetAsync(
             name: name, 
             pagination: new PaginationFilter(PageSize: 1), 
             cancellationToken: Arg.Any<CancellationToken>())
        .Returns(isUnique ? [] : [GetEntity(ClientType.Machine)]);

    private static Command GetRequest(
        string id = "id",
        string? name = null,
        string? description = null,
        string? loginUrl = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(id, name, description, loginUrl, callbackUrls, signOutUrls);
}