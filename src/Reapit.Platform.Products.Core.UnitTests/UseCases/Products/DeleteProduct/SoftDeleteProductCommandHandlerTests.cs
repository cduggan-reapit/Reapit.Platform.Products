using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Services.Notifications.Models;
using Reapit.Platform.Products.Core.UnitTests.TestServices;
using Reapit.Platform.Products.Core.UseCases.Products.DeleteProduct;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Products.DeleteProduct;

public class SoftDeleteProductCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MockNotificationsService _notifications = new();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly FakeLogger<SoftDeleteProductCommandHandler> _logger = new();
    
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
    public async Task Handle_ReturnsEntity_WhenUpdateSuccessful()
    {
        const string id = "test-id";
        var product = new Product("name", "description");
        _productRepository.GetProductByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(product);

        // Fix the time so that the generated notification timestamp is static.
        using var _ = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        
        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);

        await _productRepository.Received(1).UpdateAsync(Arg.Is<Product>(p => p.DateDeleted != null), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        var expectedNotification = MessageEnvelope.ProductDeleted(actual);
        _notifications.LastMessage.Should().BeEquivalentTo(expectedNotification);
    }
    
    /*
     * Private methods
     */
    
    private SoftDeleteProductCommandHandler CreateSut()
    {
        _unitOfWork.Products.Returns(_productRepository);
        return new SoftDeleteProductCommandHandler(_unitOfWork, _notifications, _logger);
    }
    
    private static SoftDeleteProductCommand GetRequest(string id = "id")
        => new(id);
}