using FluentValidation;
using MediatR;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.CreateDummy;

/// <summary>Defines the handler for the <see cref="CreateDummyCommand"/> request.</summary>
public class CreateDummyCommandHandler : IRequestHandler<CreateDummyCommand, Dummy>
{
    private readonly IValidator<CreateDummyCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="CreateDummyCommandHandler"/> class.</summary>
    /// <param name="validator">The validator for the request type.</param>
    /// <param name="unitOfWork">The unit of work service.</param>
    public CreateDummyCommandHandler(
        IValidator<CreateDummyCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    /// <exception cref="ValidationException">the provided request failed validation.</exception>
    public async Task<Dummy> Handle(CreateDummyCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var dummy = new Dummy(request.Name);

        await _unitOfWork.Dummies.CreateAsync(dummy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return dummy;
    }
}