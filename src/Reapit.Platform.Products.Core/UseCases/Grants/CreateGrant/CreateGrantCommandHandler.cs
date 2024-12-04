using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Exceptions;
using Reapit.Platform.Products.Core.Extensions;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;

public class CreateGrantCommandHandler(
    IUnitOfWork unitOfWork, 
    IIdentityProviderService idpService, 
    IValidator<CreateGrantCommand> validator, 
    ILogger<CreateGrantCommandHandler> logger)
    : IRequestHandler<CreateGrantCommand, Entities.Grant>
{
    /// <inheritdoc />
    public async Task<Entities.Grant> Handle(CreateGrantCommand request, CancellationToken cancellationToken)
    {
        // Step 01: Validate
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        // Step 02: Get the entities (we know they exist, so just assert! rather then dealing with nulls)
        var client = (await unitOfWork.Clients.GetByIdAsync(request.ClientId, cancellationToken))!;
        var resourceServer = (await unitOfWork.ResourceServers.GetByIdAsync(request.ResourceServerId, cancellationToken))!;
        
        // Step 03: Check that there isn't already a grant between the referenced client and resource server:
        if (client.Grants.Any(grant => grant.ResourceServerId.Equals(request.ResourceServerId, StringComparison.OrdinalIgnoreCase)))
            throw ConflictException.ResourceExists(nameof(Entities.Grant), (Client: request.ClientId, ResourceServer: request.ResourceServerId).ToJson());
        
        // Step 04: Register with the IdP to get the external identifier
        var externalId = await idpService.CreateGrantAsync(request, client, resourceServer, cancellationToken);
        
        // Step 05: Create the scopes collection for the grant
        var scopes = resourceServer.Scopes.Where(scope => request.Scopes.Contains(scope.Value, StringComparer.OrdinalIgnoreCase)).ToList();
        
        // Step 06: Create the grant entity
        var entity = new Entities.Grant(externalId, client.Id, resourceServer.Id)
        {
            Client = client,
            ResourceServer = resourceServer,
            Scopes = scopes
        };
        
        // Step 07: Save the entity
        _ = await unitOfWork.Grants.CreateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Step 08: Log and return
        logger.LogInformation("Grant created: {id} ({blob})", entity.Id, entity.ToString());
        return entity;
    }
}