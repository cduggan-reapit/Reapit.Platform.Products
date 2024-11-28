using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.PatchProductClient;

/// <summary>Handler for the <see cref="PatchProductClientCommand"/> request.</summary>
public class PatchProductClientCommandHandler(
    IUnitOfWork unitOfWork, 
    IValidator<PatchProductClientCommand> validator, 
    ILogger<PatchProductClientCommandHandler> logger)
    : IRequestHandler<PatchProductClientCommand, ProductClient>
{
    /// <inheritdoc />
    public async Task<ProductClient> Handle(PatchProductClientCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if(!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        var client = await unitOfWork.ProductClients.GetProductClientByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProductClient), request.Id);
        
        client.Update(request.Name, request.Description, request.CallbackUrls, request.SignOutUrls);
        if (!client.IsDirty)
            return client;
        
        // TODO: update auth0 (after dirty check)
        
        _ = await unitOfWork.ProductClients.UpdateAsync(client, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Product client updated: {id}, ({blob})", request.Id, client.ToString());
        return client;
    }
}