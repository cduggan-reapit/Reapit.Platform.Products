using FluentValidation.Results;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClients;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.Services;
using static Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.TestProductClientFactory;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.GetProductClients;

public class GetProductClientsQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductClientRepository _clientRepository = Substitute.For<IProductClientRepository>();
    private readonly IValidator<GetProductClientsQuery> _validator = Substitute.For<IValidator<GetProductClientsQuery>>();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsQueryStringException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<GetProductClientsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<QueryValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntities_WhenValidationSucceeds()
    {
        _validator.ValidateAsync(Arg.Any<GetProductClientsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var request = GetRequest(
            createdFrom: DateTime.UnixEpoch.AddDays(1),
            createdTo: DateTime.UnixEpoch.AddDays(2),
            modifiedFrom: DateTime.UnixEpoch.AddDays(3),
            modifiedTo: DateTime.UnixEpoch.AddDays(4));

        var clients = new[] { GetProductClient() };
        _clientRepository.GetProductClientsAsync(
                request.Name, 
                request.Description, 
                request.ProductId,
                request.Type!,
                new PaginationFilter(request.Cursor, request.PageSize),
                new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo),
                Arg.Any<CancellationToken>())
            .Returns(clients);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(clients);
    }
    
    /*
     * Private methods
     */
    
    private GetProductClientsQueryHandler CreateSut()
    {
        _unitOfWork.ProductClients.Returns(_clientRepository);
        return new GetProductClientsQueryHandler(_unitOfWork, _validator);
    }
    
    private static GetProductClientsQuery GetRequest(
        long? cursor = 1731533393578244L, 
        int pageSize = 25,
        string? name = "name",
        string? description = "description",
        string? productId = "productId",
        string? type = "client_credentials",
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        DateTime? modifiedFrom = null,
        DateTime? modifiedTo = null)
        => new(cursor, pageSize, name, description, productId, type, createdFrom, createdTo, modifiedFrom, modifiedTo);
}