using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Grants.DeleteGrant;

/// <summary>Handler for the <see cref="DeleteGrantCommand"/> request.</summary>
public class DeleteGrantCommandHandler(
    IUnitOfWork unitOfWork,
    IIdentityProviderService idpService,
    ILogger<DeleteGrantCommandHandler> logger)
    : IRequestHandler<DeleteGrantCommand, Entities.Grant>
{
    /// <inheritdoc />
    public async Task<Entities.Grant> Handle(DeleteGrantCommand request, CancellationToken cancellationToken)
    {
        var grant = await unitOfWork.Grants.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException(typeof(Entities.Client), request.Id);

        // We don't actually care what comes back, only that something comes back - failures in the third-party library
        // should cause exceptions that we aren't going to pre-emptively catch.
        _ = idpService.DeleteGrantAsync(grant, cancellationToken);

        grant.SoftDelete();
        await unitOfWork.Grants.UpdateAsync(grant, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Grant deleted: {id} ({blob})", grant.Id, grant.ToString());
        return grant;
    }
}