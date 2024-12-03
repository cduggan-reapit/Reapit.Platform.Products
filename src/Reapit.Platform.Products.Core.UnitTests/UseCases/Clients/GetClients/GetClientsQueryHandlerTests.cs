using FluentValidation.Results;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.UseCases.Clients.GetClients;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Clients.GetClients;

public class GetClientsQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IClientRepository _repository = Substitute.For<IClientRepository>();
    private readonly IValidator<GetClientsQuery> _validator = Substitute.For<IValidator<GetClientsQuery>>();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsQueryStringException_WhenValidationFailed()
    {
        _validator.ValidateAsync(Arg.Any<GetClientsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<QueryValidationException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntities_WhenValidationSucceeds_AndTypeNotNull()
    {
        _validator.ValidateAsync(Arg.Any<GetClientsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var request = GetRequest(
            createdFrom: DateTime.UnixEpoch.AddDays(1),
            createdTo: DateTime.UnixEpoch.AddDays(2),
            modifiedFrom: DateTime.UnixEpoch.AddDays(3),
            modifiedTo: DateTime.UnixEpoch.AddDays(4));

        var entities = new[] { new Entities.Client("app-id", "external-id", ClientType.Machine, "name", "description", null, null, null) };
        _repository.GetAsync(
                request.AppId,
                request.Type!,
                request.Name,
                request.Description, 
                new PaginationFilter(request.Cursor, request.PageSize),
                new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo),
                Arg.Any<CancellationToken>())
            .Returns(entities);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(entities);
    }
    
    [Fact]
    public async Task Handle_ReturnsEntities_WhenValidationSucceeds_AndTypeNull()
    {
        _validator.ValidateAsync(Arg.Any<GetClientsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var request = GetRequest(
            type: null,
            createdFrom: DateTime.UnixEpoch.AddDays(1),
            createdTo: DateTime.UnixEpoch.AddDays(2),
            modifiedFrom: DateTime.UnixEpoch.AddDays(3),
            modifiedTo: DateTime.UnixEpoch.AddDays(4));

        var entities = new[] { new Entities.Client("app-id", "external-id", ClientType.Machine, "name", "description", null, null, null) };
        _repository.GetAsync(
                request.AppId,
                null,
                request.Name,
                request.Description, 
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
    
    private GetClientsQueryHandler CreateSut()
    {
        _unitOfWork.Clients.Returns(_repository);
        return new GetClientsQueryHandler(_unitOfWork, _validator);
    }
    
    private static GetClientsQuery GetRequest(
        long? cursor = 1731533393578244L, 
        int pageSize = 25,
        string? appId = "app",
        string? type = "machine",
        string? name = "name",
        string? description = "audience",
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        DateTime? modifiedFrom = null,
        DateTime? modifiedTo = null)
        => new(cursor, pageSize, appId, type, name, description, createdFrom, createdTo, modifiedFrom, modifiedTo);
}