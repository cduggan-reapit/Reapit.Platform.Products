﻿using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UseCases.Grants.PatchGrant;

/// <summary>Validator for the <see cref="PatchGrantCommand"/> request.</summary>
public class PatchGrantCommandValidator : AbstractValidator<PatchGrantCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    private ICollection<string>? _scopes;

    /// <summary>Initializes a new instance of the <see cref="PatchGrantCommandValidator"/> class.</summary>
    public PatchGrantCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        // I'm cheating a bit.  When() MUST run before Must(), so I use the When() method to set the _scopes collection
        // which we then compare the requested scopes against.  It's cheeky, but it works.
        RuleForEach(request => request.Scopes)
            .Must(scope => _scopes?.Contains(scope, StringComparer.OrdinalIgnoreCase) ?? false)
            .WithMessage(GrantValidationMessages.UnsupportedScope)
            .WhenAsync(GetResourceServerScopes);
    }
    
    private async Task<bool> GetResourceServerScopes(PatchGrantCommand request, CancellationToken cancellationToken)
    {
        var grant = await _unitOfWork.Grants.GetByIdAsync(request.Id, cancellationToken);
        
        // Don't run the test if the grant is null, there's no point.  The handler will throw 404.
        if (grant == null)
            return false;
        
        _scopes = grant.ResourceServer.Scopes.Select(scope => scope.Value).ToList();
        return true;
    }
}