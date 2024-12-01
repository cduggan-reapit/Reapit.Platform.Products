using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;

/// <summary>Handler for the <see cref="CreateResourceServerCommand"/> request.</summary>
public class CreateResourceServerCommandHandler(
    IUnitOfWork unitOfWork, 
    IIdentityProviderService idpService,
    IValidator<CreateResourceServerCommand> validator,
    ILogger<CreateResourceServerCommandHandler> logger) 
    : IRequestHandler<CreateResourceServerCommand, Entities.ResourceServer>
{
    /// <inheritdoc/>
    public async Task<Entities.ResourceServer> Handle(CreateResourceServerCommand request, CancellationToken cancellationToken)
    {
        // Step 01: Validate the command
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        // Step 02: Create the resource server in the IdP to get its external identifier
        var externalId = await idpService.CreateResourceServerAsync(request, cancellationToken);
        
        // Step 03: Create the base entity
        var entity = new Entities.ResourceServer(externalId, request.Audience, request.Name, request.TokenLifetime);
        
        // Step 04: Add scopes to the entity
        entity.SetScopes(request.Scopes.Select(scope => scope.ToEntity(entity.Id)).ToList());

        // Step 05: Commit the changes
        _ = await unitOfWork.ResourceServers.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 06: send notification
        // TODO: product.created
        
        // Step 07: Log completion of the handler
        logger.LogInformation("Resource server created: {id} ({blob})", entity.Id, entity.AsSerializable());
        
        // Return the entity
        return entity;
    }
}