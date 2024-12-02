using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Applications.DeleteApplication;

/// <summary>Handler for the <see cref="DeleteApplicationCommand"/> request.</summary>
public class DeleteApplicationCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteApplicationCommandHandler> logger)
    : IRequestHandler<DeleteApplicationCommand, Entities.App>
{
    /// <inheritdoc />
    public async Task<Entities.App> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.Apps.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException(typeof(Entities.App), request.Id);

        // If the entity is associated with any active clients, don't allow deletion. 
        if (entity.Clients.Any())
            throw new ValidationException([new ValidationFailure("", ApplicationValidationMessages.ClientsPreventingDelete)]);
        
        entity.SoftDelete();

        _ = await unitOfWork.Apps.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Log the change and return the updated entity
        logger.LogInformation("Application deleted: {id} ({blob})", entity.Id, entity.ToString());
        return entity;
    }
}