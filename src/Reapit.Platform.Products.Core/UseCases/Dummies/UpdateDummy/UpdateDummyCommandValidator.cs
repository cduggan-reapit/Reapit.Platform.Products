using FluentValidation;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.UpdateDummy;

/// <summary>Defines the validator for the <see cref="UpdateDummyCommand"/> request.</summary>
public class UpdateDummyCommandValidator : AbstractValidator<UpdateDummyCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>Initializes a new instance of the <see cref="UpdateDummyCommandValidator"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public UpdateDummyCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        /*
         * Id
         */
        
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Must not be empty")
            .DependentRules(() =>
            {
                // Only run these rules if id is not empty:
                RuleFor(command => command.Id)
                    .Must(id => Guid.TryParse(id, out _))
                    .WithMessage("Must be a valid guid");
            });
        
        /*
         * Name
         */
        
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Must not be empty")
            .DependentRules(() =>
            {
                // Only run these rules if name is not null or empty
                RuleFor(command => command.Name)
                    .MaximumLength(100)
                    .WithMessage("Must be fewer than 100 characters in length");
            });
        
        // Only check the database when we know the string is an acceptable length and id is a guid
        // This is repeating validation logic really, which we don't need if we let the DB say no
        RuleFor(command => command)
            .MustAsync(async (command, cancellationToken) => await IsNameUnique(command, cancellationToken))
            .When(command => Guid.TryParse(command.Id, out _) && !string.IsNullOrEmpty(command.Name) && command.Name.Length <= 100)
            .WithName(nameof(UpdateDummyCommand.Name))
            .WithMessage("Must be unique");
    }
    
    private async Task<bool> IsNameUnique(UpdateDummyCommand command, CancellationToken cancellationToken)
    {
        // Note: in a live situation, we'd let the database handle this check through a unique index and handle the 
        //       resulting exception.  This is just an example of validation that involves the database 🙂
        var duplicates = await _unitOfWork.Dummies.GetAsync(
            name: command.Name, 
            createdFrom: null, 
            createdTo: null, 
            modifiedFrom: null, 
            modifiedTo: null, 
            cancellationToken: cancellationToken);

        // Return true if all records with the command name match the id in the command (or there are no duplicates)
        var guid = Guid.Parse(command.Id);
        return duplicates.All(item => item.Id == guid);
    }
}