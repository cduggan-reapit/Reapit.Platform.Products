using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.Notifications;
using Reapit.Platform.Products.Core.Services.Notifications.Models;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.CreateProduct;

/// <summary>Handler for the <see cref="CreateProductCommand"/> request.</summary>
public class CreateProductCommandHandler(
    IUnitOfWork unitOfWork,
    INotificationsService notifications,
    IValidator<CreateProductCommand> validator,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, Product>
{
    /// <inheritdoc />
    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if(!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var product = new Product(request.Name, request.Description);
        _ = await unitOfWork.Products.CreateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Product created: {id} ({blob})", product.Id, product.ToString());
        _ = await notifications.PublishNotificationAsync(MessageEnvelope.ProductCreated(product), cancellationToken);
        
        return product;
    }
}