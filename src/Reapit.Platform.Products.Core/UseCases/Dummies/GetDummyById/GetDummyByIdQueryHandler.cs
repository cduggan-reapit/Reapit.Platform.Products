using FluentValidation;
using MediatR;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.GetDummyById;

/// <summary>Defines the handler for the <see cref="GetDummyByIdQuery"/> request.</summary>
public class GetDummyByIdQueryHandler : IRequestHandler<GetDummyByIdQuery, Dummy>
{
    private readonly IValidator<GetDummyByIdQuery> _validator;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="GetDummyByIdQueryHandler"/> class.</summary>
    /// <param name="validator">The validator for the request type.</param>
    /// <param name="unitOfWork">The unit of work service.</param>
    public GetDummyByIdQueryHandler(
        IValidator<GetDummyByIdQuery> validator,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }
    
    /// <inheritdoc/>
    /// <exception cref="ValidationException">the provided request failed validation.</exception>
    /// <exception cref="NotFoundException">no object was found with the requested identifier.</exception>
    public async Task<Dummy> Handle(GetDummyByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var id = Guid.Parse(request.Id);
        return await _unitOfWork.Dummies.GetByIdAsync(id, cancellationToken) 
                     ?? throw new NotFoundException(nameof(Dummy), request.Id);
    }
}