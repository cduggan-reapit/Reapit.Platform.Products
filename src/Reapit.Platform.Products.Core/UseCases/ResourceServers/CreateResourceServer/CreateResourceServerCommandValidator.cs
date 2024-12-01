using System.Text.RegularExpressions;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;

/// <summary>Validator for the <see cref="CreateResourceServerCommand"/> request.</summary>
public class CreateResourceServerCommandValidator : AbstractValidator<CreateResourceServerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="CreateResourceServerCommandValidator"/> class.</summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    public CreateResourceServerCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        // Name:
        //  - Required
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
                RuleFor(command => command.Name)
                    .MustAsync(IsNameUnique)
                    .WithMessage(CommonValidationMessages.Unique);
            });
        
        // Audience
        //  - Required
        //  - Max length of 600 characters
        //  - Unique
        RuleFor(command => command.Audience)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(600)
            .WithMessage(ResourceServerValidationMessages.AudienceTooLong)
            .DependentRules(() =>
            {
                RuleFor(command => command.Audience)
                    .MustAsync(IsAudienceUnique)
                    .WithMessage(CommonValidationMessages.Unique);
            });
        
        // TokenLifetime
        //  - Minimum 60 (must be at least 1 minute)
        //  - Maximum 864000 (up to one day)
        RuleFor(command => command.TokenLifetime)
            .InclusiveBetween(60, 86400)
            .WithMessage(ResourceServerValidationMessages.TokenLifetimeOutOfRange);
        
        // Use the scope model validator to validate scopes.
        RuleForEach(command => command.Scopes).SetValidator(new RequestScopeModelValidator());
    }

    private async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
    {
        var conflicts = await _unitOfWork.ResourceServers.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: cancellationToken);
        return !conflicts.Any();
    }
    
    private async Task<bool> IsAudienceUnique(string audience, CancellationToken cancellationToken)
    {
        var conflicts = await _unitOfWork.ResourceServers.GetAsync(audience: audience, pagination: new PaginationFilter(PageSize: 1), cancellationToken: cancellationToken);
        return !conflicts.Any();
    }
}