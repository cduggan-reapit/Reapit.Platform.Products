using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.DeleteProductClient;

/// <summary>Handler for the <see cref="SoftDeleteProductClientCommand"/> request.</summary>
public class SoftDeleteProductClientCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<SoftDeleteProductClientCommandHandler> logger) 
    : IRequestHandler<SoftDeleteProductClientCommand, ProductClient>
{
    /// <inheritdoc />
    public async Task<ProductClient> Handle(SoftDeleteProductClientCommand request, CancellationToken cancellationToken)
    {
        var client = await unitOfWork.ProductClients.GetProductClientByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(ProductClient), request.Id);
        
        // TODO: delete the client in auth0
        // await _idpClient.DeleteClient(client.ClientId, cancellationToken);
        
        client.SoftDelete();
        
        _ = await unitOfWork.ProductClients.UpdateAsync(client, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Product client deleted: {id} ({blob})", client.Id, client.ToString());
        return client;
    }
}