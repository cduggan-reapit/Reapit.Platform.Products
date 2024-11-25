using FluentValidation;
using MediatR;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.DeleteDummyById;

/// <summary>Defines the handler for the <see cref="DeleteDummyByIdCommand"/> request.</summary>
public class DeleteDummyByIdCommandHandler : IRequestHandler<DeleteDummyByIdCommand>
{
    private readonly IValidator<DeleteDummyByIdCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="DeleteDummyByIdCommandHandler"/> class.</summary>
    /// <param name="validator">The validator for the request type.</param>
    /// <param name="unitOfWork">The unit of work service.</param>
    public DeleteDummyByIdCommandHandler(IValidator<DeleteDummyByIdCommand> validator, IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    /// <exception cref="ValidationException">the provided request failed validation.</exception>
    /// <exception cref="NotFoundException">no object was found with the requested identifier.</exception>
    public async Task Handle(DeleteDummyByIdCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        var id = Guid.Parse(request.Id);
        var dummy = await _unitOfWork.Dummies.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Dummy), request.Id);

        await _unitOfWork.Dummies.DeleteAsync(dummy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}