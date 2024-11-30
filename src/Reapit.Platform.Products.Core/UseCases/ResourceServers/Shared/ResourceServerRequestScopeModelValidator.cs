namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.Shared;

/// <summary>Validator for the <see cref="ResourceServerRequestScopeModel"/> type.</summary>
public class ResourceServerRequestScopeModelValidator : AbstractValidator<ResourceServerRequestScopeModel>
{
    /// <summary>Initializes a new instance of the <see cref="ResourceServerRequestScopeModelValidator"/> class.</summary>
    public ResourceServerRequestScopeModelValidator()
    {
        // Value
        //  - Required
        //  - Max length of 280 characters
        RuleFor(scope => scope.Value)
            .MinimumLength(1)
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(280)
            .WithMessage(ResourceServerRequestScopeValidationMessages.ValueTooLong);

        // Description
        //  - Max length of 500 characters
        RuleFor(scope => scope.Description)
            .MaximumLength(500)
            .WithMessage(ResourceServerRequestScopeValidationMessages.DescriptionTooLong);
    }
}