using Auth0.ManagementApi.Models;

namespace Reapit.Platform.Products.Core.UseCases.Common.Scopes;

/// <summary>Represents a resource server scope when included in a request.</summary>
/// <param name="Value">The value of the scope.</param>
/// <param name="Description">An optional, user-friendly description of the scope.</param>
public record RequestScopeModel(string Value, string? Description)
{
    /// <summary>Create an instance of <see cref="Entities.Scope"/> from this model.</summary>
    /// <param name="resourceServerId">The unique identifier of the resource server.</param>
    public Entities.Scope ToEntity(string resourceServerId)
        => new(resourceServerId, Value, Description);
    
    /// <summary>Create an instance of <see cref="ResourceServerScope"/> from this model.</summary>
    public ResourceServerScope ToResourceServerScope() 
        => new() { Value = Value, Description = Description ?? string.Empty };
};