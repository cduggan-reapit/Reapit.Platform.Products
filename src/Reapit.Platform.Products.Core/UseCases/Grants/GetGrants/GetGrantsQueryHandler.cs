using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Grants.GetGrants;

/// <summary>Handler for the <see cref="GetGrantsQuery"/> request.</summary>
public class GetGrantsQueryHandler(IUnitOfWork unitOfWork, IValidator<GetGrantsQuery> validator)
    : IRequestHandler<GetGrantsQuery, IEnumerable<Entities.Grant>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<Entities.Grant>> Handle(GetGrantsQuery request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw QueryValidationException.ValidationFailed(validation);

        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var dateFilter = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);

        return await unitOfWork.Grants.GetAsync(
            clientId: request.ClientId, 
            resourceServerId: request.ResourceServerId, 
            pagination: pagination, 
            dateFilter: dateFilter,
            cancellationToken: cancellationToken);
    }
}