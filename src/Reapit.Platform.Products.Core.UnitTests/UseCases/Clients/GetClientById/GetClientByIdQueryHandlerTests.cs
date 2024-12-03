using Reapit.Platform.Products.Core.UseCases.Clients.GetClientById;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Clients.GetClientById;

public class GetClientByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IClientRepository _repository = Substitute.For<IClientRepository>();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRepositoryReturnsNull()
    {
        const string id = "test-id";
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.Client?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenRepositoryReturnsResult()
    {
        const string id = "test-id";
        var entity = new Entities.Client("app-id", "external-id", ClientType.Machine, "machine client", "description", null, null, null);
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var request = GetRequest(id);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(entity);
    }
    
    /*
     * Private methods
     */
    
    private GetClientByIdQueryHandler CreateSut()
    {
        _unitOfWork.Clients.Returns(_repository);
        return new GetClientByIdQueryHandler(_unitOfWork);
    }
    
    private static GetClientByIdQuery GetRequest(string id = "id")
        => new(id);
}