using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClients;

/// <summary>Handler for the <see cref="GetProductClientsQuery"/> request.</summary>
public class GetProductClientsQueryHandler(IUnitOfWork unitOfWork, IValidator<GetProductClientsQuery> validator) 
    : IRequestHandler<GetProductClientsQuery, IEnumerable<ProductClient>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<ProductClient>> Handle(GetProductClientsQuery request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if(!validation.IsValid)
            throw QueryValidationException.ValidationFailed(validation);

        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var dateFilter = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);
        
        // Note: if the type is null or invalid, it won't apply a type filter. We could validate it if we wanted to.
        return await unitOfWork.ProductClients.GetProductClientsAsync(
            name: request.Name, 
            description: request.Description, 
            productId: request.ProductId,
            type: request.Type ?? string.Empty,
            pagination: pagination, 
            dateFilter: dateFilter, 
            cancellationToken: cancellationToken);
    }
}