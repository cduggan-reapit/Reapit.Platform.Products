using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;

/// <summary>Validator for the <see cref="CreateApplicationCommand"/> request.</summary>
public class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="CreateApplicationCommandValidator"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public CreateApplicationCommandValidator(IUnitOfWork unitOfWork)
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
                RuleFor(request => request.Name)
                    .MustAsync(IsNameUnique)
                    .WithMessage(CommonValidationMessages.Unique);
            });
        
        /*
         * Description
         */

        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .WithMessage(ApplicationValidationMessages.DescriptionTooLong);
    }

    private async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
    {
        var conflicts = await _unitOfWork.Apps.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: cancellationToken);
        return !conflicts.Any();
    }
}