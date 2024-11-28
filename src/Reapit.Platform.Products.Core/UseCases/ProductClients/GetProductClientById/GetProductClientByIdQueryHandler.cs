using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClientById;

/// <summary>Handler for the <see cref="GetProductClientByIdQuery"/> request.</summary>
public class GetProductClientByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetProductClientByIdQuery, ProductClient>
{
    /// <inheritdoc />
    public async Task<ProductClient> Handle(GetProductClientByIdQuery request, CancellationToken cancellationToken)
        => await unitOfWork.ProductClients.GetProductClientByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(ProductClient), request.Id);
}