using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Applications.GetApplicationById;

/// <summary>Handler for the <see cref="GetApplicationByIdQuery"/> request.</summary>
public class GetApplicationByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetApplicationByIdQuery, Entities.App>
{
    /// <inheritdoc />
    public async Task<Entities.App> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
        => await unitOfWork.Apps.GetByIdAsync(request.Id, cancellationToken)
           ?? throw new NotFoundException(typeof(Entities.App), request.Id);
}