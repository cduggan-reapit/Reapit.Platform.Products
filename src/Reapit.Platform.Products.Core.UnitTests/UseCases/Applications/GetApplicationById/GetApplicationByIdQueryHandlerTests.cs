using Reapit.Platform.Products.Core.UseCases.Applications.GetApplicationById;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Applications.GetApplicationById;

public class GetApplicationByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAppRepository _repository = Substitute.For<IAppRepository>();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenRepositoryReturnsNull()
    {
        const string id = "test-id";
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<App?>(null));

        var request = GetRequest(id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenRepositoryReturnsResult()
    {
        const string id = "test-id";
        var entity = new App("name", "description", true);
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

    private GetApplicationByIdQueryHandler CreateSut()
    {
        _unitOfWork.Apps.Returns(_repository);
        return new GetApplicationByIdQueryHandler(_unitOfWork);
    }
    
    private static GetApplicationByIdQuery GetRequest(string id = "id")
        => new(id);
}