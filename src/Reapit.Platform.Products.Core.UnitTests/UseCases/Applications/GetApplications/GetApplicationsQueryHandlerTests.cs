using FluentValidation.Results;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Applications.GetApplications;

public class GetApplicationsQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAppRepository _repository = Substitute.For<IAppRepository>();
    private readonly IValidator<GetApplicationsQuery> _validator = Substitute.For<IValidator<GetApplicationsQuery>>();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsQueryStringException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<GetApplicationsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<QueryValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntities_WhenValidationSucceeds()
    {
        _validator.ValidateAsync(Arg.Any<GetApplicationsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var request = GetRequest(
            createdFrom: DateTime.UnixEpoch.AddDays(1),
            createdTo: DateTime.UnixEpoch.AddDays(2),
            modifiedFrom: DateTime.UnixEpoch.AddDays(3),
            modifiedTo: DateTime.UnixEpoch.AddDays(4));

        // Two-birds, one stone.  Make sure the query parameters are passed correctly by only returning the value if they are.
        var entities = new[] { new App("name", "description", true) };
        _repository.GetAsync(
                request.Name, 
                request.Description,
                request.IsFirstParty,
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
    
    private GetApplicationsQueryHandler CreateSut()
    {
        _unitOfWork.Apps.Returns(_repository);
        return new GetApplicationsQueryHandler(_unitOfWork, _validator);
    }
    
    private static GetApplicationsQuery GetRequest(
        long? cursor = 1731533393578244L, 
        int pageSize = 25,
        string? name = "name",
        string? description = "audience",
        bool? isFirstParty = true,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        DateTime? modifiedFrom = null,
        DateTime? modifiedTo = null)
        => new(cursor, pageSize, name, description, isFirstParty, createdFrom, createdTo, modifiedFrom, modifiedTo);
}