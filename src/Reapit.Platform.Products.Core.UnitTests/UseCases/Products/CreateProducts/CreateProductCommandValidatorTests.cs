using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Products;
using Reapit.Platform.Products.Core.UseCases.Products.CreateProduct;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Products.CreateProducts;

public class CreateProductCommandValidatorTests
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
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameTooLong()
    {
        var command = GetCommand(name: new string('a', 101));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Fail(nameof(CreateProductCommand.Name), ProductValidationMessages.NameTooLong);
        
        // If it fails on this, we don't want to hit the database...
        await _productRepository.DidNotReceiveWithAnyArgs().GetProductsAsync();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenDescriptionTooLong()
    {
        var command = GetCommand(description: new string('a', 1001));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Fail(nameof(CreateProductCommand.Description), ProductValidationMessages.DescriptionTooLong);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameUnavailable()
    {
        _productRepository.GetProductsAsync()
            .ReturnsForAnyArgs([new Product("conflicting", "product")]);
        
        var command = GetCommand();
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.Should().Fail(nameof(CreateProductCommand.Name), CommonValidationMessages.Unique);
        
        // If it fails on this, we don't want to hit the database...
        await _productRepository.Received(1).GetProductsAsync(
            name: command.Name,
            pagination: new PaginationFilter(PageSize: 1),
            cancellationToken: Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private CreateProductCommandValidator CreateSut()
    {
        _unitOfWork.Products.Returns(_productRepository);
        return new CreateProductCommandValidator(_unitOfWork);
    }
    
    private static CreateProductCommand GetCommand(string name = "name", string? description = "description")
        => new(name, description);
}