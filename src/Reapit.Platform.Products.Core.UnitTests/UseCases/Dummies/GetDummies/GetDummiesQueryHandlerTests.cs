using Reapit.Platform.Products.Core.UseCases.Dummies.GetDummies;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Dummies.GetDummies;

public class GetDummiesQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDummyRepository _dummyRepository = Substitute.For<IDummyRepository>();

    [Fact]
    public async Task Handle_RequestsEntitiesFromDummiesCollection_WithMappedParameters()
    {
        var query = new GetDummiesQuery(
            Name: "test-name", 
            CreatedFrom: DateTime.UnixEpoch,
            CreatedTo: DateTime.UnixEpoch.AddDays(1), 
            ModifiedFrom: DateTime.UnixEpoch.AddDays(2),
            ModifiedTo: DateTime.UnixEpoch.AddDays(3));

        var dummies = new[]
        {
            new Dummy("test-dummy-1") { Id = Guid.NewGuid() },
            new Dummy("test-dummy-2") { Id = Guid.NewGuid() }
        };

        // We don't need to check Received here, we configure the mock to return only if the values are correctly mapped
        _dummyRepository.GetAsync(
                name: query.Name, 
                createdFrom: query.CreatedFrom, 
                createdTo: query.CreatedTo, 
                modifiedFrom: query.ModifiedFrom,
                modifiedTo: query.ModifiedTo, 
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(dummies);

        var sut = CreateSut();
        var actual = await sut.Handle(query, default);
        actual.Should().BeEquivalentTo(dummies);
    }
    
    /*
     * Private methods
     */

    private GetDummiesQueryHandler CreateSut()
    {
        _unitOfWork.Dummies.Returns(_dummyRepository);
        return new GetDummiesQueryHandler(_unitOfWork);
    }
}