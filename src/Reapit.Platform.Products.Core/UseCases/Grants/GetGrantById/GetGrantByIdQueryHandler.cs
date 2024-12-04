using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Grants.GetGrantById;

/// <summary>Handler for the <see cref="GetGrantByIdQuery"/> request.</summary>
public class GetGrantByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGrantByIdQuery, Entities.Grant>
{
    /// <inheritdoc />
    public async Task<Entities.Grant> Handle(GetGrantByIdQuery request, CancellationToken cancellationToken)
        => await unitOfWork.Grants.GetByIdAsync(request.Id, cancellationToken)
           ?? throw new NotFoundException(typeof(Entities.Grant), request.Id);
}