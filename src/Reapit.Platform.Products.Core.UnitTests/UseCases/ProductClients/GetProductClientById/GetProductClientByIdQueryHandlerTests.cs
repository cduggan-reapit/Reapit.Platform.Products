using Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClientById;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.GetProductClientById;

public class GetProductClientByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductClientRepository _productClientRepository = Substitute.For<IProductClientRepository>();

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
    public async Task Handle_ReturnsEntity_WhenRepositoryReturnsResult()
    {
        const string id = "test-id";
        var client = GetProductClient();
        _productClientRepository.GetProductClientByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(client);

        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(client);
    }

    /*
     * Private methods
     */

    private GetProductClientByIdQueryHandler CreateSut()
    {
        _unitOfWork.ProductClients.Returns(_productClientRepository);
        return new GetProductClientByIdQueryHandler(_unitOfWork);
    }

    private static GetProductClientByIdQuery GetRequest(string id = "id")
        => new(id);
    
    private static ProductClient GetProductClient(
        string productId = "productId",
        string clientId = "clientId",
        string grantId = "grantId",
        string name = "name",
        string description = "description",
        ClientType? type = null,
        string? audience = null, 
        ICollection<string>? callbackUrls = null, 
        ICollection<string>? signOutUrls = null)
        => new(productId, clientId, grantId, name, description, type ?? ClientType.ClientCredentials, audience, callbackUrls, signOutUrls);
}