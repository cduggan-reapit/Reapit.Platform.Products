using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Applications.PatchApplication;

/// <summary>Handler for the <see cref="PatchApplicationCommand"/> request.</summary>
public class PatchApplicationCommandHandler(
    IUnitOfWork unitOfWork, 
    IValidator<PatchApplicationCommand> validator, 
    ILogger<PatchApplicationCommandHandler> logger) 
    : IRequestHandler<PatchApplicationCommand, Entities.App>
{
    public async Task<Entities.App> Handle(PatchApplicationCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = await unitOfWork.Apps.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException(typeof(Entities.App), request.Id);
        
        // Update the entity and shortcut return if nothing has changed
        entity.Update(name: request.Name, description: request.Description);
        if (!entity.IsDirty)
            return entity;
        
        // Otherwise apply the update
        _ = await unitOfWork.Apps.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Log the change and return the updated entity
        logger.LogInformation("Application updated: {id} ({blob})", entity.Id, entity.ToString());
        return entity;
    }
}