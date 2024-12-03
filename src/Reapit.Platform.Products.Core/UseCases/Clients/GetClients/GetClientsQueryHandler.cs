using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.Clients.GetClients;

/// <summary>Handler for the <see cref="GetClientsQuery"/> request.</summary>
public class GetClientsQueryHandler(IUnitOfWork unitOfWork, IValidator<GetClientsQuery> validator)
    : IRequestHandler<GetClientsQuery, IEnumerable<Entities.Client>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<Entities.Client>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw QueryValidationException.ValidationFailed(validation);
        
        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var dateFilter = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);

        var type = request.Type == null ? null : ClientType.GetByName(request.Type);
        
        return await unitOfWork.Clients.GetAsync(
            appId: request.AppId, 
            type: type, 
            name: request.Name, 
            description: request.Description,
            pagination: pagination, 
            dateFilter: dateFilter, 
            cancellationToken: cancellationToken);
    }
}