using FluentValidation;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Products.CreateProduct;

/// <summary>Validator for the <see cref="CreateProductCommand"/> request.</summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes an instance of the <see cref="CreateProductCommandValidator"/> class.</summary>
    public CreateProductCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        RuleFor(command => command.Description)
            .MaximumLength(1000)
            .WithMessage(ProductValidationMessages.DescriptionTooLong);
        
        RuleFor(command => command.Name)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(100)
            .WithMessage(ProductValidationMessages.NameTooLong)
            .MustAsync(IsNameUnique)
            .WithMessage(CommonValidationMessages.Unique);
    }

    private async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
    {
        var conflicts = await _unitOfWork.Products.GetProductsAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: cancellationToken);
        return !conflicts.Any();
    }
}