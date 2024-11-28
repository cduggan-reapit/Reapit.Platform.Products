namespace Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClients;

/// <summary>Validator for the <see cref="GetProductClientsQuery"/> request.</summary>
public class GetProductClientsQueryValidator : AbstractValidator<GetProductClientsQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetProductClientsQueryValidator"/> class.</summary>
    public GetProductClientsQueryValidator()
    {
        RuleFor(query => query.Cursor)
            .GreaterThanOrEqualTo(0)
            .WithMessage(CommonValidationMessages.CursorOutOfRange);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(CommonValidationMessages.PageSizeOutOfRange);
    }
}