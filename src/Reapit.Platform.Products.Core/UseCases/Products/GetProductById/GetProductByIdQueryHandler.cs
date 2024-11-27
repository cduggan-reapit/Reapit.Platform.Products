using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.GetProductById;

/// <summary>Handler for the <see cref="GetProductById"/> request.</summary>
public class GetProductByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetProductByIdQuery, Product>
{
    /// <inheritdoc />
    public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        => await unitOfWork.Products.GetProductByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(Product), request.Id);
}