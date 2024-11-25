using FluentValidation;

namespace Reapit.Platform.Products.Core.UseCases.Dummies.GetDummyById;

/// <summary>Defines the validator for the <see cref="GetDummyByIdQuery"/> request.</summary>
public class GetDummyByIdQueryValidator : AbstractValidator<GetDummyByIdQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetDummyByIdQueryValidator"/> class.</summary>
    public GetDummyByIdQueryValidator()
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