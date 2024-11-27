namespace Reapit.Platform.Products.Core.UseCases.Products.GetProducts;

/// <summary>Validator for the <see cref="GetProductsQuery"/> request.</summary>
public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    /// <summary>Initializes a new instance of the <see cref="GetProductsQueryValidator"/> class.</summary>
    public GetProductsQueryValidator()
    {
        RuleFor(query => query.Cursor)
            .GreaterThanOrEqualTo(0)
            .WithMessage(CommonValidationMessages.CursorOutOfRange);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(CommonValidationMessages.PageSizeOutOfRange);
    }
}