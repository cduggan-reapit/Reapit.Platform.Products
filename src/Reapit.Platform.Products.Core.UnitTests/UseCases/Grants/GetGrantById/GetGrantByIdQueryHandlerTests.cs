using Reapit.Platform.Products.Core.UseCases.Grants.GetGrantById;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Grants.GetGrantById;

public class GetGrantByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGrantRepository _repository = Substitute.For<IGrantRepository>();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRepositoryReturnsNull()
    {
        const string id = "test-id";
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.Grant?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenRepositoryReturnsResult()
    {
        const string id = "test-id";
        var entity = new Entities.Grant("external-id", "client-id", "resource-server-id")
        {
            Client = default!,
            ResourceServer = default!
        };
        
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
    
    private GetGrantByIdQueryHandler CreateSut()
    {
        _unitOfWork.Grants.Returns(_repository);
        return new GetGrantByIdQueryHandler(_unitOfWork);
    }
    
    private static GetGrantByIdQuery GetRequest(string id = "id")
        => new(id);
}