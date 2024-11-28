using System.Text.RegularExpressions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.PatchProductClient;

/// <summary>Validator for the <see cref="PatchProductClientCommand"/> request.</summary>
public class PatchProductClientCommandValidator : AbstractValidator<PatchProductClientCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    private ProductClient? _entity;
    
    public PatchProductClientCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        /*
         * Name
         */
        
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.NotEmpty)
            .MaximumLength(100)
            .WithMessage(ProductClientValidationMessages.NameTooLong)
            .Must(name => !Regex.IsMatch(name, "[<>]"))
            .WithMessage(ProductClientValidationMessages.NameMalformed)
            .DependentRules(() =>
            {
                // Only do this if the rest of the rules pass
                RuleFor(command => command)
                    .MustAsync(IsUniqueName)
                    .WithName(nameof(PatchProductClientCommand.Name))
                    .WithMessage(CommonValidationMessages.Unique);
            })
            .When(command => command.Name is not null);
        
        /*
         * Description
         */
            
        RuleFor(command => command.Description)
            .MaximumLength(140)
            .WithMessage(ProductClientValidationMessages.DescriptionTooLong);
        
        /*
         * CallbackUrls
         */

        When(
            predicate: command => command.CallbackUrls is not null, 
            action: () =>
            {
                // CallbackUrls not be empty for authCode apps
                RuleFor(command => command.CallbackUrls)
                    .NotEmpty()
                    .WhenAsync(async (command, cancellationToken) => await IsAuthorizationCodeClient(command, cancellationToken) == true)
                    .WithMessage(CommonValidationMessages.NotEmpty);
                
                // CallbackUrls must be null or empty for non-authCode apps
                RuleFor(command => command.CallbackUrls)
                    .Empty()
                    .WhenAsync(async (command, cancellationToken) => await IsAuthorizationCodeClient(command, cancellationToken) == false)
                    .WithMessage(ProductClientValidationMessages.UnsupportedByClientCredentials);
            });
        
        /*
         * CallbackUrls
         */

        When(
            predicate: command => command.SignOutUrls is not null, 
            action: () =>
            {
                // CallbackUrls not be empty for authCode apps
                RuleFor(command => command.SignOutUrls)
                    .NotEmpty()
                    .WhenAsync(async (command, cancellationToken) => await IsAuthorizationCodeClient(command, cancellationToken) == true)
                    .WithMessage(CommonValidationMessages.NotEmpty);;
                
                // CallbackUrls must be null or empty for non-authCode apps
                RuleFor(command => command.SignOutUrls)
                    .Empty()
                    .WhenAsync(async (command, cancellationToken) => await IsAuthorizationCodeClient(command, cancellationToken) == false)
                    .WithMessage(ProductClientValidationMessages.UnsupportedByClientCredentials);;
            });
    }

    private async Task<bool> IsUniqueName(PatchProductClientCommand command, CancellationToken cancellationToken)
    {
        // If the entity doesn't exist, don't report a conflict and allow the handler to throw a 404
        var entity = _entity ??= await _unitOfWork.ProductClients.GetProductClientByIdAsync(command.Id, cancellationToken);
        if (entity == null)
            return true;
        
        // If the name hasn't changed, we don't flag a conflict
        if (entity.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            return true;
        
        // At this point we know the user intends to change the name so we look for any other entities with the same name:
        var conflicts = await _unitOfWork.ProductClients.GetProductClientsAsync(
            name: command.Name, 
            pagination: new PaginationFilter(PageSize: 1), 
            cancellationToken: cancellationToken);
        
        // If there are no conflicts, the new name is unique
        return !conflicts.Any();
    }

    private async Task<bool?> IsAuthorizationCodeClient(PatchProductClientCommand command,
        CancellationToken cancellationToken)
    {
        // If the entity doesn't exist, return false - this is used in the WhenAsync clause so will avoid running 
        // rules for missing objects.
        var entity = _entity ??= await _unitOfWork.ProductClients.GetProductClientByIdAsync(command.Id, cancellationToken);
        if (entity == null)
            return null;
        
        return entity.Type == ClientType.AuthorizationCode;
    }
}