using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;

/// <summary>Handler for the <see cref="GetApplicationsQuery"/> request.</summary>
public class GetApplicationsQueryHandler(IUnitOfWork unitOfWork, IValidator<GetApplicationsQuery> validator)
    : IRequestHandler<GetApplicationsQuery, IEnumerable<Entities.App>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<Entities.App>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw QueryValidationException.ValidationFailed(validation);

        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var dateFilter = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);

        return await unitOfWork.Apps.GetAsync(
            name: request.Name, 
            description: request.Description, 
            isFirstParty: request.IsFirstParty, 
            pagination: pagination, 
            dateFilter: dateFilter,
            cancellationToken: cancellationToken);
    }
}