using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.Services.Notifications;
using Reapit.Platform.Products.Core.Services.Notifications.Models;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.PatchResourceServer;

/// <summary>Handler for the <see cref="PatchResourceServerCommand"/> request.</summary>
public class PatchResourceServerCommandHandler(IUnitOfWork unitOfWork, 
    IIdentityProviderService idpService, 
    INotificationsService notifications,
    IValidator<PatchResourceServerCommand> validator,
    ILogger<PatchResourceServerCommandHandler> logger) 
    : IRequestHandler<PatchResourceServerCommand, Entities.ResourceServer>
{
    /// <inheritdoc />
    public async Task<Entities.ResourceServer> Handle(PatchResourceServerCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = await unitOfWork.ResourceServers.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException(typeof(Entities.ResourceServer), request.Id);

        // Update the entity
        var updatedScopes = request.Scopes?.Select(scope => scope.ToEntity(entity.Id)).ToList();
        entity.Update(request.Name, request.TokenLifetime, updatedScopes);
        
        // Return without changing anything if the entity isn't dirty
        if (!entity.IsDirty)
            return entity;
        
        // Otherwise, update the IdP:
        _ = await idpService.UpdateResourceServerAsync(entity, cancellationToken);
        
        // Then update the repository:
        _ = await unitOfWork.ResourceServers.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        
        // Log that the change was applied and return the entity:
        logger.LogInformation("Resource server updated: {id} ({blob})", entity.Id, entity.AsSerializable());
        _ = await notifications.PublishNotificationAsync(MessageEnvelope.ProductModified(entity), cancellationToken);
        return entity;
    }
}