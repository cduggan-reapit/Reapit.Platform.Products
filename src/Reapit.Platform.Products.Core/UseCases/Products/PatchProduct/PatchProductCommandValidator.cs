using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Products.PatchProduct;

/// <summary>Validator for the <see cref="PatchProductCommand"/> request.</summary>
public class PatchProductCommandValidator : AbstractValidator<PatchProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="PatchProductCommandValidator"/> class.</summary>
    public PatchProductCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        RuleFor(command => command.Description)
            .MaximumLength(1000)
            .WithMessage(ProductValidationMessages.DescriptionTooLong);

        RuleFor(command => command)
            .Cascade(CascadeMode.Stop)
            .Must(command => !string.IsNullOrWhiteSpace(command.Name))
            .WithMessage(CommonValidationMessages.NotEmpty)
            .Must(command => command.Name!.Length <= 100)
            .WithMessage(ProductValidationMessages.NameTooLong)
            .MustAsync(IsNameUnique)
            .WithMessage(CommonValidationMessages.Unique)
            .WithName(nameof(PatchProductCommand.Name))
            .When(command => command.Name is not null);
    }
    
    private async Task<bool> IsNameUnique(PatchProductCommand command, CancellationToken cancellationToken)
    {
        // If it doesn't exist, return true. Validation should pass and allow the handler to throw NotFound.
        var subject = await _unitOfWork.Products.GetProductByIdAsync(command.Id, cancellationToken);
        if (subject is null)
            return true;

        // If the name's unchanged then there's no need to check again.
        if (subject.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            return true;
        
        // If any records exist with the requested name, then it is not unique and we return false.
        var others = await _unitOfWork.Products.GetProductsAsync(
            name: command.Name, 
            pagination: new PaginationFilter(PageSize: 1),
            cancellationToken: cancellationToken);

        return !others.Any();
    }
}