using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.ProductClients;
using Reapit.Platform.Products.Core.UseCases.ProductClients.PatchProductClient;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;
using static Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.TestProductClientFactory;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.PatchProductClient;

public class PatchProductClientCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductClientRepository _productClientRepository = Substitute.For<IProductClientRepository>();

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenCommandValid()
    {
        // client_credentials client
        var command = GetCommand(name: "new name", description: "new description");
        var client = GetProductClient();
        
        SetupGetById(command.Id, client);
        SetupIsUnique(command.Name, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
        
        // The name has changed, so we expect these to each get hit once:
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientsAsync();
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenCommandPropertiesNull()
    {
        // client_credentials client
        var command = GetCommand();
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
        
        // No database hits when nothing has changed, please:
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientsAsync();
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    /*
     * Name
     */

    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameIsEmpty_AndNameIsNotNull()
    {
        var command = GetCommand(name: "");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.Name), CommonValidationMessages.NotEmpty);
        
        // No database hits please:
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientsAsync();
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameIsTooLong()
    {
        var command = GetCommand(name: new string('a', 101));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.Name), ProductClientValidationMessages.NameTooLong);
        
        // No database hits please:
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientsAsync();
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameContainsInvalidCharacters()
    {
        var command = GetCommand(name: "<name>");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.Name), ProductClientValidationMessages.NameMalformed);
        
        // No database hits please:
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientsAsync();
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameChanged_ButEntityNotFound()
    {
        // client_credentials client
        var command = GetCommand(name: "new name", description: "new description");
        
        SetupGetById(command.Id, null);
        SetupIsUnique(command.Name, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
        
        // The name has changed, so we only the `GetById` method to get hit:
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameIsNotUnique()
    {
        // client_credentials client
        var command = GetCommand(name: "new name", description: "new description");
        var client = GetProductClient();
        
        SetupGetById(command.Id, client);
        SetupIsUnique(command.Name, false);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.Name), CommonValidationMessages.Unique);
        
        // The name has changed, so we expect these to each get hit once:
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientsAsync();
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    /*
     * Description
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenDescriptionIsTooLong()
    {
        var command = GetCommand(description: new string('a', 141));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.Description), ProductClientValidationMessages.DescriptionTooLong);
    }
    
    /*
     * CallbackUrls
     */

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenCallbackUrlsChanged_ForAuthCodeClient()
    {
        // client_credentials client
        var command = GetCommand(name: "new name", callbackUrls: ["http://example.com/callback"]);
        var client = GetProductClient(type: ClientType.AuthorizationCode);
        
        SetupGetById(command.Id, client);
        SetupIsUnique(command.Name, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
        
        // The name has changed, so we expect these to each get hit - but we expect the callback tests to re-use the
        // existing value rather than re-fetching the entity.
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientsAsync();
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenCallbackUrlsEmpty_ForAuthCodeClient()
    {
        var command = GetCommand(callbackUrls: []);
        var client = GetProductClient(type: ClientType.AuthorizationCode);
        
        SetupGetById(command.Id, client);
        SetupIsUnique(command.Name, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.CallbackUrls), CommonValidationMessages.NotEmpty);
        
        // We should fetch the entity once to check it's type
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenCallbackUrlsProvided_ForClientCredentialsClient()
    {
        var command = GetCommand(callbackUrls: ["https://www.example.com/callback"]);
        var client = GetProductClient(type: ClientType.ClientCredentials);
        
        SetupGetById(command.Id, client);
        SetupIsUnique(command.Name, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.CallbackUrls), ProductClientValidationMessages.UnsupportedByClientCredentials);
        
        // We should fetch the entity once to check it's type
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    /*
     * SignOutUrls
     */

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenSignOutUrlsChanged_ForAuthCodeClient()
    {
        // client_credentials client
        var command = GetCommand(name: "new name", signOutUrls: ["http://example.com/sign-out"]);
        var client = GetProductClient(type: ClientType.AuthorizationCode);
        
        SetupGetById(command.Id, client);
        SetupIsUnique(command.Name, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
        
        // The name has changed, so we expect these to each get hit - but we expect the signOut tests to re-use the
        // existing value rather than re-fetching the entity.
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientsAsync();
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenSignOutUrlsEmpty_ForAuthCodeClient()
    {
        var command = GetCommand(signOutUrls: []);
        var client = GetProductClient(type: ClientType.AuthorizationCode);
        
        SetupGetById(command.Id, client);
        SetupIsUnique(command.Name, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.SignOutUrls), CommonValidationMessages.NotEmpty);
        
        // We should fetch the entity once to check it's type
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenSignOutUrlsProvided_ForClientCredentialsClient()
    {
        var command = GetCommand(signOutUrls: ["https://www.example.com/sign-out"]);
        var client = GetProductClient(type: ClientType.ClientCredentials);
        
        SetupGetById(command.Id, client);
        SetupIsUnique(command.Name, true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(PatchProductClientCommand.SignOutUrls), ProductClientValidationMessages.UnsupportedByClientCredentials);
        
        // We should fetch the entity once to check it's type
        await _productClientRepository.ReceivedWithAnyArgs(1).GetProductClientByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private PatchProductClientCommandValidator CreateSut()
    {
        _unitOfWork.ProductClients.Returns(_productClientRepository);
        return new PatchProductClientCommandValidator(_unitOfWork);
    }
    
    private static PatchProductClientCommand GetCommand(
        string id = "id",
        string? name = null, 
        string? description = null, 
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(id, name, description, callbackUrls, signOutUrls);

    private void SetupGetById(string id, ProductClient? client)
        => _productClientRepository.GetProductClientByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(client);
    
    private void SetupIsUnique(string? name, bool isUnique)
        => _productClientRepository.GetProductClientsAsync(
                name: name, 
                pagination: new PaginationFilter(PageSize: 1),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(isUnique ? Array.Empty<ProductClient>() : [GetProductClient()]);
}