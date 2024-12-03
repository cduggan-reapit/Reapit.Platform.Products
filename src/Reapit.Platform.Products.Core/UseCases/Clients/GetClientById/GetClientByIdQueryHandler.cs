using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Clients.GetClientById;

/// <summary>Handler for the <see cref="GetClientByIdQuery"/> request.</summary>
public class GetClientByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetClientByIdQuery, Entities.Client>
{
    /// <inheritdoc />
    public async Task<Entities.Client> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        => await unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken)
           ?? throw new NotFoundException(typeof(Entities.Client), request.Id);
}