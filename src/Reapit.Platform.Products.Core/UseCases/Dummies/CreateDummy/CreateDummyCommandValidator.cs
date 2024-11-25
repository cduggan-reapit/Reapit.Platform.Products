using FluentValidation;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.CreateDummy;

/// <summary>Defines the validator for the <see cref="CreateDummyCommand"/> request.</summary>
public class CreateDummyCommandValidator : AbstractValidator<CreateDummyCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>Initializes a new instance of the <see cref="CreateDummyCommandValidator"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public CreateDummyCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Must not be empty")
            .DependentRules(() =>
            {
                // Only run these rules if name is not null or empty
                RuleFor(command => command.Name)
                    .MaximumLength(100)
                    .WithMessage("Must be fewer than 100 characters in length");
                    
                // Only check the database when we know the string is an acceptable length
                RuleFor(command => command.Name)
                    .MustAsync(async (name, cancellationToken) => await IsNameUnique(name, cancellationToken))
                    .When(command => command.Name.Length <= 100)
                    .WithMessage("Must be unique");
            });
    }

    private async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
    {
        // Note: in a live situation, we'd let the database handle this check through a unique index and handle the 
        //       resulting exception.  This is just an example of validation that involves the database 🙂
        
        var duplicates = await _unitOfWork.Dummies.GetAsync(
            name: name, 
            createdFrom: null, 
            createdTo: null, 
            modifiedFrom: null, 
            modifiedTo: null, 
            cancellationToken: cancellationToken);
        
        return !duplicates.Any();
    }
}