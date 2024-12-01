using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.DeleteResourceServer;

/// <summary>Handler for the <see cref="DeleteResourceServerCommand"/> request.</summary>
public class DeleteResourceServerCommandHandler(
    IUnitOfWork unitOfWork, 
    IIdentityProviderService idpService, 
    ILogger<DeleteResourceServerCommandHandler> logger) 
    : IRequestHandler<DeleteResourceServerCommand, Entities.ResourceServer>
{
    /// <inheritdoc />
    public async Task<Entities.ResourceServer> Handle(DeleteResourceServerCommand request, CancellationToken cancellationToken)
    {
        // Get the entity:
        var entity = await unitOfWork.ResourceServers.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(Entities.ResourceServer), request.Id);
        
        // Delete in the IdP:
        _ = await idpService.DeleteResourceServerAsync(entity, cancellationToken);
        
        // Delete in the repository:
        entity.SoftDelete();
        _ = await unitOfWork.ResourceServers.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // TODO: product.deleted
        
        // Log that it has been deleted
        logger.LogInformation("Resource server deleted: {id} ({blob})", entity.Id, entity.AsSerializable());
        
        // Return the deleted entity
        return entity;
    }
}