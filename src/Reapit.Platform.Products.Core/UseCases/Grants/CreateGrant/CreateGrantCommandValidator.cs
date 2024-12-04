using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;

/// <summary>Validator for the <see cref="CreateGrantCommand"/> request.</summary>
public class CreateGrantCommandValidator : AbstractValidator<CreateGrantCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private Entities.ResourceServer? _resourceServer;
    private Entities.Client? _client;

    /// <summary>Initializes a new instance of the <see cref="CreateGrantCommandValidator"/> class.</summary>
    public CreateGrantCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(request => request.ClientId)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .DependentRules(() =>
            {
                RuleFor(request => request.ClientId)
                    .MustAsync(ClientExists)
                    .WithMessage(GrantValidationMessages.ClientNotFound);
            });

        RuleFor(request => request.ResourceServerId)
            .NotEmpty()
            .WithMessage(CommonValidationMessages.Required)
            .DependentRules(() =>
            {
                RuleFor(request => request.ResourceServerId)
                    .MustAsync(ResourceServerExists)
                    .WithMessage(GrantValidationMessages.ResourceServerNotFound)
                    .DependentRules(() =>
                    {
                        // Nest this again - now we know _resourceServer is not null
                        RuleForEach(request => request.Scopes)
                            .Must(ResourceServerSupportsScope)
                            .WithMessage(GrantValidationMessages.UnsupportedScope);
                    });
            });

    }

    private async Task<bool> ClientExists(string clientId, CancellationToken cancellationToken)
    {
        _client = await _unitOfWork.Clients.GetByIdAsync(clientId, cancellationToken);
        return _client != null;
    }

    private async Task<bool> ResourceServerExists(string resourceServerId, CancellationToken cancellationToken)
    {
        _resourceServer = await _unitOfWork.ResourceServers.GetByIdAsync(resourceServerId, cancellationToken);
        return _resourceServer != null;
    }
    
    private bool ResourceServerSupportsScope(string value) 
        => _resourceServer!.Scopes.Any(scope => scope.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
}