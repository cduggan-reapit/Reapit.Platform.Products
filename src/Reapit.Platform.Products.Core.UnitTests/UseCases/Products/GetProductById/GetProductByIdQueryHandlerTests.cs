using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Products.Core.UseCases.Products.GetProductById;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Products.GetProductById;

public class GetProductByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRepositoryReturnsNull()
    {
        const string id = "test-id";
        _productRepository.GetProductByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenRepositoryReturnsResult()
    {
        const string id = "test-id";
        var product = new Product("name", "description");
        _productRepository.GetProductByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(product);

        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(product);
    }
    
    /*
     * Private methods
     */
    
    private GetProductByIdQueryHandler CreateSut()
    {
        _unitOfWork.Products.Returns(_productRepository);
        return new GetProductByIdQueryHandler(_unitOfWork);
    }
    
    private static GetProductByIdQuery GetRequest(string id = "id")
        => new(id);
}