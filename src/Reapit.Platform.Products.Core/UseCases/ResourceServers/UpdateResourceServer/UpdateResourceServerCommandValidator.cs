using System.Text.RegularExpressions;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.Shared;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.UpdateResourceServer;

/// <summary>Validator for the <see cref="UpdateResourceServerCommand"/> request.</summary>
public class UpdateResourceServerCommandValidator : AbstractValidator<UpdateResourceServerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="UpdateResourceServerCommandValidator"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public UpdateResourceServerCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        // Name:
        //  - Max length of 200 characters
        //  - Cannot contain ['<','>']
        //  - Unique 
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(200)
            .WithMessage(ResourceServerValidationMessages.NameTooLong)
            .Must(name => !Regex.IsMatch(name, "[<>]"))
            .WithMessage(ResourceServerValidationMessages.NameInvalid)
            .DependentRules(() =>
            {
                // Only run this if the other "name" rules pass.
                RuleFor(command => command)
                    .MustAsync(IsNameUnique)
                    .WithName(nameof(UpdateResourceServerCommand.Name))
                    .WithMessage(CommonValidationMessages.Unique);
            })
            .When(command => command.Name != null);
        
        // TokenLifetime
        //  - Minimum 60 (must be at least 1 minute)
        //  - Maximum 864000 (up to one day)
        RuleFor(command => command.TokenLifetime)
            .InclusiveBetween(60, 86400)
            .WithMessage(ResourceServerValidationMessages.TokenLifetimeOutOfRange)
            .When(command => command.TokenLifetime != null);
        
        // Use the scope model validator to validate scopes.
        RuleForEach(command => command.Scopes)
            .SetValidator(new ResourceServerRequestScopeModelValidator())
            .When(command => command.Scopes != null);
    }

    private async Task<bool> IsNameUnique(UpdateResourceServerCommand command, CancellationToken cancellationToken)
    {
        // Okay, we're back in the complicated world of uniqueness. If the id doesn't exist, return true.  That's a
        // problem which will be dealt with by the handler.
        var entity = await _unitOfWork.ResourceServers.GetByIdAsync(command.Id, cancellationToken);
        if (entity == null)
            return true;

        // If the name hasn't changed, return true. Our collation is case-insensitive - keep an eye on this if we change db provider.
        if (entity.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            return true;
        
        // Otherwise any records that exist with the same name must be a duplicate.
        var conflicts = await _unitOfWork.ResourceServers.GetAsync(name: command.Name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: cancellationToken);
        return !conflicts.Any();
    }
}