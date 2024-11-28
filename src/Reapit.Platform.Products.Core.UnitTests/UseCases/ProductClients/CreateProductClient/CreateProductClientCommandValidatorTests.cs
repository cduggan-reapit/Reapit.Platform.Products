using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.ProductClients;
using Reapit.Platform.Products.Core.UseCases.ProductClients.CreateProductClient;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;
using static Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.TestProductClientFactory;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.CreateProductClient;

public class CreateProductClientCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IProductClientRepository _productClientRepository = Substitute.For<IProductClientRepository>();

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenClientCredentialsRequestValid()
    {
        var command = GetCommand(
            type: ClientType.ClientCredentials.Name,
            audience: "https://www.example.com/audience",
            callbackUrls: null,
            signOutUrls: []);

        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenAuthorizationCodeRequestValid()
    {
        var command = GetCommand(
            type: ClientType.AuthorizationCode.Name,
            audience: "",
            callbackUrls: ["https://www.example.com/callback"],
            signOutUrls: ["https://www.example.com/signout"]);

        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Pass();
    }
    
    /*
     * Name
     */

    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameIsEmpty()
    {
        var command = GetCommand(name: "");
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Name), CommonValidationMessages.Required);
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameIsTooLong()
    {
        var command = GetCommand(name: new string('a', 101));
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Name), ProductClientValidationMessages.NameTooLong);
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameContainsChevrons()
    {
        var command = GetCommand(name: "<name>");
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Name), ProductClientValidationMessages.NameMalformed);
        await _productClientRepository.DidNotReceiveWithAnyArgs().GetProductClientsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameIsNotUnique()
    {
        var command = GetCommand(name: "not-unique");
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: false);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Name), CommonValidationMessages.Unique);
    }
    
    /*
     * Description
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenDescriptionTooLong()
    {
        var command = GetCommand(description: new string('a', 141));
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Description), ProductClientValidationMessages.DescriptionTooLong);
    }

    /*
     * ProductId
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenProductIdEmpty()
    {
        var command = GetCommand(productId: string.Empty);
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.ProductId), CommonValidationMessages.Required);
        
        await _productRepository.DidNotReceive().GetProductByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenProductIdNotFound()
    {
        var command = GetCommand(productId: "product!");
        SetupProductRepository(command.ProductId, exists: false);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.ProductId), ProductClientValidationMessages.ProductNotFound);
    }
    
    /*
     * Type
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenTypeEmpty()
    {
        var command = GetCommand(type: "");
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Type), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenTypeInvalid()
    {
        var command = GetCommand(type: "unsupported");
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Type), ProductClientValidationMessages.UnsupportedType);
    }
    
    /*
     * Audience
     */

    [Fact]
    public async Task Validate_ReturnsFailure_WhenAudienceEmpty_AndTypeIsClientCredentials()
    {
        var command = GetCommand(type: ClientType.ClientCredentials.Name, audience: string.Empty);
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Audience), ProductClientValidationMessages.RequiredByClientCredentials);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenAudienceNotEmpty_AndTypeIsAuthorizationCode()
    {
        var command = GetCommand(
            type: ClientType.AuthorizationCode.Name, 
            audience: "audience", 
            callbackUrls:["https://www.example.com/callback"], 
            signOutUrls:["https://www.example.com/sign-out"]);
        
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.Audience), ProductClientValidationMessages.UnsupportedByAuthorizationCode);
    }
    
    /*
     * CallbackUrls
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenCallbackUrlsNotEmpty_AndTypeIsClientCredentials()
    {
        var command = GetCommand(
            type: ClientType.ClientCredentials.Name, 
            callbackUrls:["https://www.example.com/callback"]);
        
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.CallbackUrls), ProductClientValidationMessages.UnsupportedByClientCredentials);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenCallbackUrlsEmpty_AndTypeIsAuthorizationCode()
    {
        var command = GetCommand(
            type: ClientType.AuthorizationCode.Name, 
            audience: null, 
            callbackUrls:[], 
            signOutUrls:["https://www.example.com/sign-out"]);
        
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.CallbackUrls), ProductClientValidationMessages.RequiredByAuthorizationCode);
    }
    
    /*
     * SignOutUrls
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenSignOutUrlsNotEmpty_AndTypeIsClientCredentials()
    {
        var command = GetCommand(
            type: ClientType.ClientCredentials.Name, 
            signOutUrls:["https://www.example.com/sign-out"]);
        
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.SignOutUrls), ProductClientValidationMessages.UnsupportedByClientCredentials);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenSignOutUrlsEmpty_AndTypeIsAuthorizationCode()
    {
        var command = GetCommand(
            type: ClientType.AuthorizationCode.Name, 
            audience: null,
            callbackUrls:["https://www.example.com/callback"],
            signOutUrls:[]);
        
        SetupProductRepository(command.ProductId, exists: true);
        SetupProductClientRepository(command.Name, isUnique: true);

        var sut = CreateSut();
        var result = await sut.ValidateAsync(command);
        result.Should().Fail(nameof(CreateProductClientCommand.SignOutUrls), ProductClientValidationMessages.RequiredByAuthorizationCode);
    }
    
    /*
     * Private methods
     */

    private CreateProductClientCommandValidator CreateSut()
    {
        _unitOfWork.Products.Returns(_productRepository);
        _unitOfWork.ProductClients.Returns(_productClientRepository);
        return new CreateProductClientCommandValidator(_unitOfWork);
    }
    
    private static CreateProductClientCommand GetCommand(
        string productId = "product-id",
        string name = "name", 
        string? description = "description", 
        string type = "client_credentials",
        string? audience = "https://example.audience",
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(productId, name, description, type, audience, callbackUrls, signOutUrls);

    private void SetupProductRepository(string productId, bool exists) 
        => _productRepository.GetProductByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(exists ? new Product("name", null) : null);

    private void SetupProductClientRepository(string name, bool isUnique)
        => _productClientRepository.GetProductClientsAsync(
                name: name, 
                pagination: new PaginationFilter(PageSize: 1),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(isUnique ? Array.Empty<ProductClient>() : [GetProductClient()]);
}