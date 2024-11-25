using FluentValidation;
using MediatR;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.UpdateDummy;

/// <summary>Defines the handler for the <see cref="UpdateDummyCommand"/> request.</summary>
public class UpdateDummyCommandHandler : IRequestHandler<UpdateDummyCommand, Dummy>
{
    private readonly IValidator<UpdateDummyCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="UpdateDummyCommandHandler"/> class.</summary>
    /// <param name="validator">The validator for the request type.</param>
    /// <param name="unitOfWork">The unit of work service.</param>
    public UpdateDummyCommandHandler(IValidator<UpdateDummyCommand> validator, IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Dummy> Handle(UpdateDummyCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        var id = Guid.Parse(request.Id);
        var dummy = await _unitOfWork.Dummies.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Dummy), request.Id);
        
        // Update the dummy itself
        dummy.Name = request.Name;
        
        // Apply changes
        await _unitOfWork.Dummies.UpdateAsync(dummy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return dummy;
    }
}