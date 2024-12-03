using System.Text.RegularExpressions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;

/// <summary>Validator for the <see cref="CreateClientCommand"/> request.</summary>
public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClientCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        /*
         * Common validators - things that apply to all client types
         */

        RuleFor(request => request.AppId)
            .MustAsync(ApplicationExistsWithId)
            .WithMessage(ClientValidationMessages.ApplicationNotFound);

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(ClientValidationMessages.NameTooLong)
            .Must(name => !Regex.IsMatch(name, "[<>]"))
            .WithMessage(ClientValidationMessages.NameInvalid)
            .DependentRules(() =>
            {
                RuleFor(request => request.Name)
                    .MustAsync(IsNameUnique)
                    .WithMessage(CommonValidationMessages.Unique);
            });

        RuleFor(request => request.Type)
            .Must(type => ClientType.GetByName(type) != null)
            .WithMessage(ClientValidationMessages.TypeInvalid);

        RuleFor(request => request.Description)
            .MaximumLength(140)
            .WithMessage(ClientValidationMessages.DescriptionTooLong);
        
        /*
         * Machine Client: the remaining fields must be null
         */
        
        When(predicate: request => ClientType.GetByName(request.Type) == ClientType.Machine, 
            action: () =>
            {
                RuleFor(request => request.LoginUrl)
                    .Empty()
                    .WithMessage(ClientValidationMessages.NotSupportedByMachineClients);
                
                RuleFor(request => request.SignOutUrls)
                    .Empty()
                    .WithMessage(ClientValidationMessages.NotSupportedByMachineClients);
                
                RuleFor(request => request.CallbackUrls)
                    .Empty()
                    .WithMessage(ClientValidationMessages.NotSupportedByMachineClients);
            });
        
        When(predicate: request => ClientType.GetByName(request.Type) == ClientType.AuthCode, 
            action: () =>
            {
                RuleFor(request => request.LoginUrl)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage(ClientValidationMessages.RequiredByAuthCodeClients)
                    .Must(url => url!.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    .WithMessage(ClientValidationMessages.LoginUrlMustBeHttps);
                
                RuleFor(request => request.SignOutUrls)
                    .NotEmpty()
                    .WithMessage(ClientValidationMessages.RequiredByAuthCodeClients);
                
                RuleFor(request => request.CallbackUrls)
                    .NotEmpty()
                    .WithMessage(ClientValidationMessages.RequiredByAuthCodeClients);
            });
    }
    
    private async Task<bool> ApplicationExistsWithId(string appId, CancellationToken cancellationToken)
        => await _unitOfWork.Apps.GetByIdAsync(appId, cancellationToken) != null;
    
    private async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
    {
        var conflicts = await _unitOfWork.Clients.GetAsync(
            name: name, 
            pagination: new PaginationFilter(PageSize: 1), 
            cancellationToken: cancellationToken);
        return !conflicts.Any();
    }
}