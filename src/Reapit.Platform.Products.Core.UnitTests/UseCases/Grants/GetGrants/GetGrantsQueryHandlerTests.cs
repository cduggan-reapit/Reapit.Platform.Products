using FluentValidation.Results;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.UseCases.Grants.GetGrants;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Grants.GetGrants;

public class GetGrantsQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGrantRepository _repository = Substitute.For<IGrantRepository>();
    private readonly IValidator<GetGrantsQuery> _validator = Substitute.For<IValidator<GetGrantsQuery>>();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsQueryStringException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<GetGrantsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<QueryValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntities_WhenValidationSucceeds()
    {
        _validator.ValidateAsync(Arg.Any<GetGrantsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var request = GetRequest(
            createdFrom: DateTime.UnixEpoch.AddDays(1),
            createdTo: DateTime.UnixEpoch.AddDays(2),
            modifiedFrom: DateTime.UnixEpoch.AddDays(3),
            modifiedTo: DateTime.UnixEpoch.AddDays(4));

        var entities = new[] 
        { 
            new Entities.Grant("external-id", "client-id", "resource-server-id")
            {
                Client = default!,
                ResourceServer = default!
            } 
        };
        
        _repository.GetAsync(
                request.ClientId,
                request.ResourceServerId,
                new PaginationFilter(request.Cursor, request.PageSize),
                new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo),
                Arg.Any<CancellationToken>())
            .Returns(entities);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(entities);
    }
    
    /*
     * Private methods
     */
    
    private GetGrantsQueryHandler CreateSut()
    {
        _unitOfWork.Grants.Returns(_repository);
        return new GetGrantsQueryHandler(_unitOfWork, _validator);
    }
    
    private static GetGrantsQuery GetRequest(
        long? cursor = 1731533393578244L, 
        int pageSize = 25,
        string? clientId = "client-id",
        string? resourceServerId = "resource-server-id",
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        DateTime? modifiedFrom = null,
        DateTime? modifiedTo = null)
        => new(cursor, pageSize, clientId, resourceServerId, createdFrom, createdTo, modifiedFrom, modifiedTo);
}