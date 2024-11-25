using FluentValidation;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.DeleteDummyById;

/// <summary>Defines the validator for the <see cref="DeleteDummyByIdCommand"/> request.</summary>
public class DeleteDummyByIdCommandValidator : AbstractValidator<DeleteDummyByIdCommand>
{
    /// <summary>Initializes a new instance of the <see cref="DeleteDummyByIdCommandValidator"/> class.</summary>
    public DeleteDummyByIdCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Must not be empty")
            .DependentRules(() =>
            {
                // Only run these rules if id is not empty:
                RuleFor(command => command.Id)
                    .Must(id => Guid.TryParse(id, out _))
                    .WithMessage("Must be a valid guid");
            });
    }
}