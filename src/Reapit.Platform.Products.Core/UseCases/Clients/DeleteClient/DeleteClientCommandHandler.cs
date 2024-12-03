using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Clients.DeleteClient;

/// <summary>Handler for the <see cref="DeleteClientCommand"/> request.</summary>
public class DeleteClientCommandHandler(
    IUnitOfWork unitOfWork,
    IIdentityProviderService idpService,
    ILogger<DeleteClientCommandHandler> logger)
    : IRequestHandler<DeleteClientCommand, Entities.Client>
{
    /// <inheritdoc />
    public async Task<Entities.Client> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException(typeof(Entities.Client), request.Id);

        // We don't actually care what comes back, only that something comes back - failures in the third-party library
        // should cause exceptions that we aren't going to pre-emptively catch.
        _ = idpService.DeleteClientAsync(client, cancellationToken);
        
        client.SoftDelete();
        await unitOfWork.Clients.UpdateAsync(client, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Client deleted: {id} ({blob})", client.Id, client.ToString());
        return client;
    }
}