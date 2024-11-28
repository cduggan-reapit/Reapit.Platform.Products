using Reapit.Platform.Products.Data.Services;
using System.Text.RegularExpressions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.CreateProductClient;

/// <summary>Validator for the <see cref="CreateProductClientCommand"/> request.</summary>
public class CreateProductClientCommandValidator : AbstractValidator<CreateProductClientCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary></summary>
    /// <param name="unitOfWork"></param>
    public CreateProductClientCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        /*
         * Name
         */
        
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(ProductClientValidationMessages.NameTooLong)
            .Must(name => !Regex.IsMatch(name, "[<>]"))
            .WithMessage(ProductClientValidationMessages.NameMalformed)
            .DependentRules(() =>
            {
                // Only do this if the rest of the rules pass
                RuleFor(command => command.Name)
                    .MustAsync(IsUniqueName)
                    .WithMessage(CommonValidationMessages.Unique);
            });
        
        /*
         * Description
         */
        
        RuleFor(command => command.Description)
            .MaximumLength(140)
            .WithMessage(ProductClientValidationMessages.DescriptionTooLong);
        
        /*
         * ProductId
         */

        RuleFor(command => command.ProductId)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .DependentRules(() =>
            {
                RuleFor(command => command.ProductId)
                    .MustAsync(IsProductFound)
                    .WithMessage(ProductClientValidationMessages.ProductNotFound);
            });
        
        /*
         * Type
         */
        
        RuleFor(command => command.Type)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .Must(type => ClientType.GetByName(type) is not null)
            .WithMessage(ProductClientValidationMessages.UnsupportedType);
        
        /*
         * Audience
         */

        RuleFor(command => command.Audience)
            .NotEmpty()
            .WithMessage(ProductClientValidationMessages.RequiredByClientCredentials)
            .When(command => ClientType.GetByName(command.Type) == ClientType.ClientCredentials);
        
        RuleFor(command => command.Audience)
            .Empty()
            .WithMessage(ProductClientValidationMessages.UnsupportedByAuthorizationCode)
            .When(command => ClientType.GetByName(command.Type) == ClientType.AuthorizationCode);
        
        /*
         * CallbackUrls
         */
        
        RuleFor(command => command.CallbackUrls)
            .Empty()
            .WithMessage(ProductClientValidationMessages.UnsupportedByClientCredentials)
            .When(command => ClientType.GetByName(command.Type) == ClientType.ClientCredentials);
        
        RuleFor(command => command.CallbackUrls)
            .NotEmpty()
            .WithMessage(ProductClientValidationMessages.RequiredByAuthorizationCode)
            .When(command => ClientType.GetByName(command.Type) == ClientType.AuthorizationCode);
        
        /*
         * SignOutUrls
         */
        
        RuleFor(command => command.SignOutUrls)
            .Empty()
            .WithMessage(ProductClientValidationMessages.UnsupportedByClientCredentials)
            .When(command => ClientType.GetByName(command.Type) == ClientType.ClientCredentials);
        
        RuleFor(command => command.SignOutUrls)
            .NotEmpty()
            .WithMessage(ProductClientValidationMessages.RequiredByAuthorizationCode)
            .When(command => ClientType.GetByName(command.Type) == ClientType.AuthorizationCode);
    }

    private async Task<bool> IsUniqueName(string name, CancellationToken cancellationToken)
    {
        // Get existing records with the same name & return true if there are no conflicts, otherwise false.
        var conflicts = await _unitOfWork.ProductClients.GetProductClientsAsync(name: name, cancellationToken: cancellationToken, pagination: new PaginationFilter(PageSize: 1));
        return !conflicts.Any();
    }
    
    private async Task<bool> IsProductFound(string productId, CancellationToken cancellationToken)
    {
        // Get the product by id. Return true if found, otherwise false.
        var product = await _unitOfWork.Products.GetProductByIdAsync(productId, cancellationToken);
        return product is not null;
    }
}