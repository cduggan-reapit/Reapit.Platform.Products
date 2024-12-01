namespace Reapit.Platform.Products.Core.UseCases.Common.Scopes;

/// <summary>Validator for the <see cref="RequestScopeModel"/> type.</summary>
public class RequestScopeModelValidator : AbstractValidator<RequestScopeModel>
{
    /// <summary>Initializes a new instance of the <see cref="RequestScopeModelValidator"/> class.</summary>
    public RequestScopeModelValidator()
    {
        // Value
        //  - Required
        //  - Max length of 280 characters
        RuleFor(scope => scope.Value)
            .MinimumLength(1)
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(280)
            .WithMessage(RequestScopeModelValidationMessages.ValueTooLong);

        // Description
        //  - Max length of 500 characters
        RuleFor(scope => scope.Description)
            .MaximumLength(500)
            .WithMessage(RequestScopeModelValidationMessages.DescriptionTooLong);
    }
}