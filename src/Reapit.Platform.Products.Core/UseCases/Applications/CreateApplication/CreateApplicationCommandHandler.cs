using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;

/// <summary>Handler for the <see cref="CreateApplicationCommand"/> request.</summary>
public class CreateApplicationCommandHandler(
    IUnitOfWork unitOfWork, 
    IValidator<CreateApplicationCommand> validator, 
    ILogger<CreateApplicationCommandHandler> logger)
    : IRequestHandler<CreateApplicationCommand, Entities.App>
{
    /// <inheritdoc />
    public async Task<Entities.App> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = new Entities.App(
            name: request.Name,
            description: request.Description,
            isFirstParty: request.IsFirstParty);

        _ = await unitOfWork.Apps.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Application created: {id} ({blob})", entity.Id, entity.ToString());
        return entity;
    }
}