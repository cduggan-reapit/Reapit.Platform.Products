using MediatR;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.DeleteProduct;

/// <summary>Handler for the <see cref="SoftDeleteProductCommand"/> request.</summary>
public class SoftDeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<SoftDeleteProductCommandHandler> logger) 
    : IRequestHandler<SoftDeleteProductCommand, Product>
{
    /// <inheritdoc />
    public async Task<Product> Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.Products.GetProductByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(Product), request.Id);
        
        entity.SoftDelete();
        _ = await unitOfWork.Products.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Product soft deleted: {id} ({blob})", entity.Id, entity.ToString());
        return entity;
    }
}