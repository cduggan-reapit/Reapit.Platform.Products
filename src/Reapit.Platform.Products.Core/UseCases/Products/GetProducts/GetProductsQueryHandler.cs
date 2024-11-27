using FluentValidation;
using MediatR;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.GetProducts;

/// <summary>Handler for the <see cref="GetProductsQuery"/> request.</summary>
public class GetProductsQueryHandler(IUnitOfWork unitOfWork, IValidator<GetProductsQuery> validator) 
    : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if(!validation.IsValid)
            throw QueryValidationException.ValidationFailed(validation);

        var pagination = new PaginationFilter(request.Cursor, request.PageSize);
        var dateFilter = new TimestampFilter(request.CreatedFrom, request.CreatedTo, request.ModifiedFrom, request.ModifiedTo);
        
        return await unitOfWork.Products.GetProductsAsync(
            name: request.Name, 
            description: request.Description, 
            pagination: pagination, 
            dateFilter: dateFilter, 
            cancellationToken: cancellationToken);
    }
}