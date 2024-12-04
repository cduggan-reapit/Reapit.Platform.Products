using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Grants.PatchGrant;

/// <summary>Handler for the <see cref="PatchGrantCommand"/> request.</summary>
public class PatchGrantCommandHandler(IUnitOfWork unitOfWork,
    IIdentityProviderService idpService,
    IValidator<PatchGrantCommand> validator,
    ILogger<PatchGrantCommandHandler> logger) 
    : IRequestHandler<PatchGrantCommand, Entities.Grant>
{
    /// <inheritdoc />
    public async Task<Entities.Grant> Handle(PatchGrantCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = await unitOfWork.Grants.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException(nameof(Entities.Grant), request.Id);

        // We know from the validator that these are all present
        var desiredScopes = entity.ResourceServer.Scopes
            .Where(scope => request.Scopes.Contains(scope.Value, StringComparer.OrdinalIgnoreCase))
            .ToList();
        
        entity.SetScopes(desiredScopes);
        
        if (!entity.IsDirty)
            return entity;

        _ = await idpService.UpdateGrantAsync(entity, cancellationToken);
        _ = await unitOfWork.Grants.UpdateAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Grant updated: {id} ({blob})", entity.Id, entity.ToString());
        return entity;
    }
}