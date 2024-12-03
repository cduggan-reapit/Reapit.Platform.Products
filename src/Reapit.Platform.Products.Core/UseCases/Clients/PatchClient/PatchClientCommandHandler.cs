using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.Clients.PatchClient;

/// <summary>Handler for the <see cref="PatchClientCommand"/> request.</summary>
public class PatchClientCommandHandler(
    IUnitOfWork unitOfWork,
    IIdentityProviderService idpService,
    IValidator<PatchClientCommand> validator,
    ILogger<PatchClientCommandHandler> logger) 
    : IRequestHandler<PatchClientCommand, Entities.Client>
{
    /// <inheritdoc />
    public async Task<Entities.Client> Handle(PatchClientCommand request, CancellationToken cancellationToken)
    {
        // Step 01: Validate
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        // Step 02: Presence
        var entity = await unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(Entities.Client), request.Id);
        
        // Step 03: Changes
        entity.Update(request.Name, request.Description, request.LoginUrl, request.CallbackUrls, request.SignOutUrls);
        if (!entity.IsDirty)
            return entity;

        // Step 04: IdP
        _ = entity.Type == ClientType.AuthCode
            ? await idpService.UpdateAuthCodeClientAsync(entity, cancellationToken)
            : await idpService.UpdateMachineClientAsync(entity, cancellationToken);

        // Step 05: Database
        _ = await unitOfWork.Clients.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Step 06: Log
        logger.LogInformation("Client updated: {id} ({blob})", entity.Id, entity.ToString());
        return entity;
    }
}