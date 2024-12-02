using Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServerById;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ResourceServers.GetResourceServerById;

public class GetResourceServerByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IResourceServerRepository _repository = Substitute.For<IResourceServerRepository>();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRepositoryReturnsNull()
    {
        const string id = "test-id";
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ResourceServer?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenRepositoryReturnsResult()
    {
        const string id = "test-id";
        var entity = new ResourceServer("externalId", "audience", "name", 3600);
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
    
    private GetResourceServerByIdQueryHandler CreateSut()
    {
        _unitOfWork.ResourceServers.Returns(_repository);
        return new GetResourceServerByIdQueryHandler(_unitOfWork);
    }
    
    private static GetResourceServerByIdQuery GetRequest(string id = "id")
        => new(id);
}