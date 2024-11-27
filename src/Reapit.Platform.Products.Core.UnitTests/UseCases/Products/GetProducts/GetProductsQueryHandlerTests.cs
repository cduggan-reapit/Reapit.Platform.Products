using FluentValidation.Results;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.UseCases.Products.GetProducts;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Products.GetProducts;

public class GetProductsQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductRepository _groupRepository = Substitute.For<IProductRepository>();
    private readonly IValidator<GetProductsQuery> _validator = Substitute.For<IValidator<GetProductsQuery>>();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsQueryStringException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<QueryValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntities_WhenValidationSucceeds()
    {
        _validator.ValidateAsync(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var request = GetRequest(
            createdFrom: DateTime.UnixEpoch.AddDays(1),
            createdTo: DateTime.UnixEpoch.AddDays(2),
            modifiedFrom: DateTime.UnixEpoch.AddDays(3),
            modifiedTo: DateTime.UnixEpoch.AddDays(4));

        // Two-birds, one stone.  Make sure the query parameters are passed correctly by only returning the value if they are.
        var groups = new[] { new Product("name", "description") };
        _groupRepository.GetProductsAsync(
                request.Name, 
                request.Description, 
                new PaginationFilter(request.Cursor, request.PageSize),
                new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo),
                Arg.Any<CancellationToken>())
            .Returns(groups);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(groups);
    }
    
    /*
     * Private methods
     */
    
    private GetProductsQueryHandler CreateSut()
    {
        _unitOfWork.Products.Returns(_groupRepository);
        return new GetProductsQueryHandler(_unitOfWork, _validator);
    }
    
    private static GetProductsQuery GetRequest(
        long? cursor = 1731533393578244L, 
        int pageSize = 25,
        string? name = "name",
        string? description = "description",
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        DateTime? modifiedFrom = null,
        DateTime? modifiedTo = null)
        => new(cursor, pageSize, name, description, createdFrom, createdTo, modifiedFrom, modifiedTo);
}