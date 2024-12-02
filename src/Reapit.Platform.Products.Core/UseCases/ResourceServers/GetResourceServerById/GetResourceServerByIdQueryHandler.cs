using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServerById;

/// <summary>Handler for the <see cref="GetResourceServerByIdQuery"/> request.</summary>
public class GetResourceServerByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetResourceServerByIdQuery, Entities.ResourceServer>
{
    /// <inheritdoc />
    public async Task<Entities.ResourceServer> Handle(GetResourceServerByIdQuery request, CancellationToken cancellationToken)
        => await unitOfWork.ResourceServers.GetByIdAsync(request.Id, cancellationToken)
           ?? throw new NotFoundException(typeof(Entities.ResourceServer), request.Id);
}