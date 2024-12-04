namespace Reapit.Platform.Products.Core.UseCases.Grants.GetGrants;

/// <summary>Validator for the <see cref="GetGrantsQuery"/> request.</summary>
public class GetGrantsQueryValidator : AbstractValidator<GetGrantsQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetGrantsQueryValidator"/> class.</summary>
    public GetGrantsQueryValidator()
    {
        RuleFor(query => query.Cursor)
            .GreaterThanOrEqualTo(0)
            .When(query => query.Cursor != null)
            .WithMessage(CommonValidationMessages.CursorOutOfRange);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, QueryConstants.MaximumPageSize)
            .WithMessage(CommonValidationMessages.PageSizeOutOfRange);
    }
}