using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServers;

public class GetResourceServersQueryHandler(IUnitOfWork unitOfWork, IValidator<GetResourceServersQuery> validator)
    : IRequestHandler<GetResourceServersQuery, IEnumerable<Entities.ResourceServer>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<Entities.ResourceServer>> Handle(GetResourceServersQuery request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw QueryValidationException.ValidationFailed(validation);

        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var dateFilter = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);

        return await unitOfWork.ResourceServers.GetAsync(request.Name, request.Audience, pagination, dateFilter, cancellationToken);
    }
}