using Microsoft.Extensions.Logging;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.CreateProductClient;

/// <summary>Handler for the <see cref="CreateProductClientCommand"/> request.</summary>
public class CreateProductClientCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<CreateProductClientCommand> validator,
    ILogger<CreateProductClientCommandHandler> logger) 
    : IRequestHandler<CreateProductClientCommand, ProductClient>
{
    /// <inheritdoc />
    public async Task<ProductClient> Handle(CreateProductClientCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if(!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        // TODO: create the client in auth0 (bring back the clientId and grantId)
        const string clientId = "client-id", grantId = "grant-id";
        
        var client = new ProductClient(
            request.ProductId, 
            clientId, 
            grantId, 
            request.Name, 
            request.Description, 
            request.Type,
            request.Audience,
            request.CallbackUrls,
            request.SignOutUrls);
        
        _ = await unitOfWork.ProductClients.CreateAsync(client, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Product client created: {id} ({blob})", client.Id, client.ToString());
        return client;
    }
}