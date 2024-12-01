using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.UpdateResourceServer;

/// <summary>Handler for the <see cref="UpdateResourceServerCommand"/> request.</summary>
public class UpdateResourceServerCommandHandler(IUnitOfWork unitOfWork, 
    IIdentityProviderService idpService, 
    IValidator<UpdateResourceServerCommand> validator,
    ILogger<UpdateResourceServerCommandHandler> logger) 
    : IRequestHandler<UpdateResourceServerCommand, Entities.ResourceServer>
{
    /// <inheritdoc />
    public async Task<Entities.ResourceServer> Handle(UpdateResourceServerCommand request, CancellationToken cancellationToken)
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
        
        // TODO: product.modified
        
        // Log that the change was applied and return the entity:
        logger.LogInformation("Resource server updated: {id} ({blob})", entity.Id, entity.AsSerializable());
        return entity;
    }
}