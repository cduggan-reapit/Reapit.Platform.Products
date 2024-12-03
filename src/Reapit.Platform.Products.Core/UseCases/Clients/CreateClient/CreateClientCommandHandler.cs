using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;

/// <summary>Handler for the <see cref="CreateClientCommand"/> request.</summary>
public class CreateClientCommandHandler(
    IUnitOfWork unitOfWork, 
    IIdentityProviderService idpService, 
    IValidator<CreateClientCommand> validator, 
    ILogger<CreateClientCommandHandler> logger) 
    : IRequestHandler<CreateClientCommand, Entities.Client>
{
    /// <inheritdoc />
    public async Task<Entities.Client> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        // Step 01: Validate the command
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        // Step 02: Get the app. Although this throws an exception, it should never be reached as the validator should
        //          ensure that the app exists before it gets to this point.
        var app = await unitOfWork.Apps.GetByIdAsync(request.AppId, cancellationToken)
                  ?? throw new NotFoundException(typeof(Entities.App), request.AppId);

        // Validation tells us that this is safe
        var clientType = ClientType.GetByName(request.Type)!;
        
        // Step 03: Create the client using the appropriate method in the IdP to get the externalId 
        var externalId = clientType == ClientType.AuthCode
            ? await idpService.CreateAuthCodeClientAsync(request, app.IsFirstParty, cancellationToken)
            : await idpService.CreateMachineClientAsync(request, app.IsFirstParty, cancellationToken);
        
        // Step 04: Create the client entity
        var entity = new Entities.Client(request.AppId, externalId, clientType, request.Name, request.Description, request.LoginUrl, request.CallbackUrls, request.SignOutUrls);
        
        // Step 05: Commit the client
        _ = await unitOfWork.Clients.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Step 06: Log completion of the handler
        logger.LogInformation("Client created: {id} ({blob})", entity.Id, entity.ToString());
        
        // Return the entity
        return entity;
    }
}