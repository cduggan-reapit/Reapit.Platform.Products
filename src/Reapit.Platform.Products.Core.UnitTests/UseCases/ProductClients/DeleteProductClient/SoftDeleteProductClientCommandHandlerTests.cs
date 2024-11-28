using Reapit.Platform.Products.Core.UseCases.ProductClients.DeleteProductClient;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using static Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.TestProductClientFactory;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.DeleteProductClient;

public class SoftDeleteProductClientCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductClientRepository _productClientRepository = Substitute.For<IProductClientRepository>();
    private readonly FakeLogger<SoftDeleteProductClientCommandHandler> _logger = new();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRepositoryReturnsNull()
    {
        const string id = "test-id";
        _productClientRepository.GetProductClientByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ProductClient?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntity_WhenUpdateSuccessful()
    {
        const string id = "test-id";
        var client = GetProductClient();
        _productClientRepository.GetProductClientByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(client);

        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.DateDeleted.Should().NotBeNull();
        
        await _productClientRepository.Received(1).UpdateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */

    private SoftDeleteProductClientCommandHandler CreateSut()
    {
        _unitOfWork.ProductClients.Returns(_productClientRepository);
        return new SoftDeleteProductClientCommandHandler(_unitOfWork, _logger);
    }

    private static SoftDeleteProductClientCommand GetRequest(string id = "id")
        => new(id);
}