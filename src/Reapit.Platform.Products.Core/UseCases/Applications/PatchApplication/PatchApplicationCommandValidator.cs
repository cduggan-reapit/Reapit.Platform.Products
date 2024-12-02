using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Applications.PatchApplication;

/// <summary>Validator for the <see cref="PatchApplicationCommand"/> request.</summary>
public class PatchApplicationCommandValidator : AbstractValidator<PatchApplicationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="PatchApplicationCommandValidator"/> class.</summary>
    public PatchApplicationCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        /*
         * Name
         */

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(ApplicationValidationMessages.NameTooLong)
            .DependentRules(() =>
            {
                RuleFor(request => request)
                    .MustAsync(IsNameUnique)
                    .WithName(nameof(PatchApplicationCommand.Name))
                    .WithMessage(CommonValidationMessages.Unique);
            })
            .When(request => request.Name is not null);
        
        /*
         * Description
         */

        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .WithMessage(ApplicationValidationMessages.DescriptionTooLong);
    }

    private async Task<bool> IsNameUnique(PatchApplicationCommand request, CancellationToken cancellationToken)
    {
        // Try to get the entity - if it's null return true to allow the handler to throw 404.
        var entity = await _unitOfWork.Apps.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return true;

        // If the name hasn't changed, return true
        if (entity.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            return true;
        
        // If the name has changed, check the database for anything with the proposed name:
        var conflicts = await _unitOfWork.Apps.GetAsync(name: request.Name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: cancellationToken);
        return !conflicts.Any();
    }
}