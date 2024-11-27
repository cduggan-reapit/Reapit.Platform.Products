using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Products;
using Reapit.Platform.Products.Core.UseCases.Products.PatchProduct;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Products.PatchProduct;

public class PatchProductCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenCommandValid()
    {
        var command = GetCommand();
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Pass();
        
        // It should call GetById (because the values are otherwise valid), but since that returns null without further
        // configuration, it should not call Get
        await _productRepository.Received(1).GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _productRepository.DidNotReceiveWithAnyArgs().GetProductsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNoValuesProvided()
    {
        var command = GetCommand(name: null, description: null);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Pass();
        
        // It shouldn't make any database requests here, as there are no values to change
        await _productRepository.DidNotReceive().GetProductByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _productRepository.DidNotReceiveWithAnyArgs().GetProductsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameUnchanged()
    {
        var product = new Product("name", "description");
        var command = GetCommand(name: product.Name, description: null);
        
        _productRepository.GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);
        
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Pass();
        
        // We expect it to hit the first query, but bail out before the second because the name hasn't changed
        await _productRepository.Received(1).GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _productRepository.DidNotReceiveWithAnyArgs().GetProductsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameAvailable()
    {
        var product = new Product("name", "description");
        var command = GetCommand(name: "new name", description: null);
        
        _productRepository.GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);
        
        _productRepository.GetProductsAsync(name: command.Name, pagination: new PaginationFilter(PageSize:1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Product>());
        
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameEmpty()
    {
        var command = GetCommand(name: "  ", description: null);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Fail(nameof(PatchProductCommand.Name), CommonValidationMessages.NotEmpty);
        
        // We use cascade.stop so it should fail before it reaches the database checks and bail out
        await _productRepository.DidNotReceive().GetProductByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _productRepository.DidNotReceiveWithAnyArgs().GetProductsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameTooLong()
    {
        var command = GetCommand(name: new string('a', 101), description: null);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Fail(nameof(PatchProductCommand.Name), ProductValidationMessages.NameTooLong);
        
        // We use cascade.stop so it should fail before it reaches the database checks and bail out
        await _productRepository.DidNotReceive().GetProductByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _productRepository.DidNotReceiveWithAnyArgs().GetProductsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameUnavailable()
    {
        var product = new Product("name", "description");
        var command = GetCommand(name: "different name", description: null);
        
        _productRepository.GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepository.GetProductsAsync()
            .ReturnsForAnyArgs([new Product("arbitrary", "conflict")]);
        
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Fail(nameof(PatchProductCommand.Name), CommonValidationMessages.Unique);
        
        // We expect it to hit the first query, but bail out before the second because the name hasn't changed
        await _productRepository.Received(1).GetProductByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _productRepository.Received(1)
            .GetProductsAsync(name: command.Name, pagination: new PaginationFilter(PageSize:1), cancellationToken: Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenDescriptionTooLong()
    {
        var command = GetCommand(name: null, description: new string('a', 1001));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Fail(nameof(PatchProductCommand.Description), ProductValidationMessages.DescriptionTooLong);
        
        // Belt and braces
        await _productRepository.DidNotReceive().GetProductByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _productRepository.DidNotReceiveWithAnyArgs().GetProductsAsync();
    }
    
    /*
     * Private methods
     */

    private PatchProductCommandValidator CreateSut()
    {
        _unitOfWork.Products.Returns(_productRepository);
        return new PatchProductCommandValidator(_unitOfWork);
    }
    
    private static PatchProductCommand GetCommand(string id = "id", string? name = "name", string? description = "description")
        => new(id, name, description);
}