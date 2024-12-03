namespace Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;

/// <summary>Validator for the <see cref="GetApplicationsQuery"/> request.</summary>
public class GetApplicationsQueryValidator : AbstractValidator<GetApplicationsQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetApplicationsQueryValidator"/> class.</summary>
    public GetApplicationsQueryValidator()
    {
        RuleFor(query => query.Cursor)
            .GreaterThanOrEqualTo(0)
            .WithMessage(CommonValidationMessages.CursorOutOfRange);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, QueryConstants.MaximumPageSize)
            .WithMessage(CommonValidationMessages.PageSizeOutOfRange);
    }
}