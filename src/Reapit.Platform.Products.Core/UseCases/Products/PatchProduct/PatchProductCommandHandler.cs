using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.PatchProduct;

/// <summary>Handler for the <see cref="PatchProductCommand"/> request.</summary>
public class PatchProductCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<PatchProductCommand> validator,
    ILogger<PatchProductCommandHandler> logger) 
    : IRequestHandler<PatchProductCommand, Product>
{
    /// <inheritdoc />
    public async Task<Product> Handle(PatchProductCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if(!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        var entity = await unitOfWork.Products.GetProductByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(Product), request.Id);
        
        entity.Update(request.Name, request.Description);
        if(!entity.IsDirty)
            return entity;
        
        _ = await unitOfWork.Products.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Product updated: {id} ({blob})", entity.Id, entity.ToString());
        return entity;
    }
}