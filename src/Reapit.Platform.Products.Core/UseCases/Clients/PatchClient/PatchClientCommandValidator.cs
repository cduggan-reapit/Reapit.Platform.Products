using System.Text.RegularExpressions;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.Clients.PatchClient;

/// <summary>Validator for the <see cref="PatchClientCommand"/> request.</summary>
public class PatchClientCommandValidator : AbstractValidator<PatchClientCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private Entities.Client? _entity;

    /// <summary>Initialize a new instance of the <see cref="PatchClientCommandValidator"/> class.</summary>
    public PatchClientCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        /*
         * Name
         */

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.NotEmpty)
            .MaximumLength(100)
            .WithMessage(ClientValidationMessages.NameTooLong)
            .Must(name => !Regex.IsMatch(name, "[<>]"))
            .WithMessage(ClientValidationMessages.NameInvalid)
            .DependentRules(() =>
            {
                RuleFor(request => request)
                    .MustAsync(IsNameUnique)
                    .WithName(nameof(PatchClientCommand.Name))
                    .WithMessage(CommonValidationMessages.Unique);
            })
            .When(request => request.Name != null);

        RuleFor(request => request.Description)
            .MaximumLength(140)
            .WithMessage(ClientValidationMessages.DescriptionTooLong)
            .When(request => request.Description != null);

        WhenAsync(IsMachineClient, () =>
        {
            RuleFor(request => request.LoginUrl)
                .Empty()
                .WithMessage(ClientValidationMessages.NotSupportedByMachineClients)
                .When(request => request.LoginUrl != null);

            RuleFor(request => request.SignOutUrls)
                .Empty()
                .WithMessage(ClientValidationMessages.NotSupportedByMachineClients)
                .When(request => request.SignOutUrls != null);

            RuleFor(request => request.CallbackUrls)
                .Empty()
                .WithMessage(ClientValidationMessages.NotSupportedByMachineClients)
                .When(request => request.CallbackUrls != null);
        });

        WhenAsync(IsAuthCodeClient, () =>
        {
            RuleFor(request => request.LoginUrl)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(ClientValidationMessages.RequiredByAuthCodeClients)
                .Must(url => url!.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                .WithMessage(ClientValidationMessages.LoginUrlMustBeHttps)
                .When(request => request.LoginUrl != null);

            RuleFor(request => request.SignOutUrls)
                .NotEmpty()
                .WithMessage(ClientValidationMessages.RequiredByAuthCodeClients)
                .When(request => request.SignOutUrls != null);

            RuleFor(request => request.CallbackUrls)
                .NotEmpty()
                .WithMessage(ClientValidationMessages.RequiredByAuthCodeClients)
                .When(request => request.CallbackUrls != null);
        });
    }

    private async Task<bool> IsNameUnique(PatchClientCommand request, CancellationToken cancellationToken)
    {
        _entity ??= await _unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken);
        
        // If the entity doesn't exist, stop trying to validate it.  We'll let it through and have the handler throw
        // not found if we get that far.
        if (_entity == null)
            return true;

        // If the name hasn't changed, no need to check uniqueness
        if (_entity.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            return true;

        var conflicts = await _unitOfWork.Clients.GetAsync(
            name: request.Name,
            pagination: new PaginationFilter(PageSize: 1),
            cancellationToken: cancellationToken);

        return !conflicts.Any();
    }
    
    private async Task<bool> IsMachineClient(PatchClientCommand request, CancellationToken cancellationToken)
        => await GetClientTypeAsync(request, cancellationToken) == ClientType.Machine;
    
    private async Task<bool> IsAuthCodeClient(PatchClientCommand request, CancellationToken cancellationToken)
        => await GetClientTypeAsync(request, cancellationToken) == ClientType.AuthCode; 

    private async Task<ClientType?> GetClientTypeAsync(PatchClientCommand request, CancellationToken cancellationToken)
    {
        _entity ??= await _unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken);
        return _entity?.Type;
    }
}