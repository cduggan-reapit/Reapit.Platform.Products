namespace Reapit.Platform.Products.Core.UseCases.Clients.GetClients;

/// <summary>Validator for the <see cref="GetClientsQuery"/> request.</summary>
public class GetClientsQueryValidator : AbstractValidator<GetClientsQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetClientsQueryValidator"/> class.</summary>
    public GetClientsQueryValidator()
    {
        // Cursor must be greater than or equal to 0 (or null, which is treated as zero but avoids the where clause)
        RuleFor(query => query.Cursor)
            .GreaterThanOrEqualTo(0)
            .When(query => query.Cursor != null)
            .WithMessage(CommonValidationMessages.CursorOutOfRange);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, QueryConstants.MaximumPageSize)
            .WithMessage(CommonValidationMessages.PageSizeOutOfRange);
    }
}