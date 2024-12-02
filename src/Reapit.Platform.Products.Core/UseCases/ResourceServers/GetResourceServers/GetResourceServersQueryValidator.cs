namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServers;

/// <summary>Validator for the <see cref="GetResourceServersQuery"/> request.</summary>
public class GetResourceServersQueryValidator : AbstractValidator<GetResourceServersQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetResourceServersQueryValidator"/> class.</summary>
    public GetResourceServersQueryValidator()
    {
        // Cursor must be greater than or equal to 0 (or null, which is treated as zero but avoids the where clause)
        RuleFor(query => query.Cursor)
            .GreaterThanOrEqualTo(0)
            .When(query => query.Cursor != null)
            .WithMessage(CommonValidationMessages.CursorOutOfRange);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(CommonValidationMessages.PageSizeOutOfRange);
    }
}