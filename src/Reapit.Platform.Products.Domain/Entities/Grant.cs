using Reapit.Platform.Products.Domain.Entities.Abstract;
using Reapit.Platform.Products.Domain.Entities.Interfaces;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents a grant.</summary>
public class Grant : EntityBase, IHasScopes
{
    /// <summary>Initializes a new instance of the <see cref="Grant"/> class.</summary>
    /// <param name="externalId">The unique identifier of the grant within the IdP service.</param>
    /// <param name="clientId">The unique identifier of the client with which this grant is associated.</param>
    /// <param name="resourceServerId">The unique identifier of the resource server to which this grant gives access.</param>
    public Grant(string externalId, string clientId, string resourceServerId)
    {
        ExternalId = externalId;
        ClientId = clientId;
        ResourceServerId = resourceServerId;
    }
    
    /// <inheritdoc/>
    public void SetScopes(ICollection<Scope> scopes)
    {
        // Get the collections as a collection of names - that's what matters to this service:
        var proposedScopeNames = scopes.Select(scope => scope.Value).ToList();
        var currentScopeNames = Scopes.Select(scope => scope.Value).ToList();
        
        // Find the scopes we need to add 
        var scopeNamesToAdd = proposedScopeNames.Where(proposed 
                => !currentScopeNames.Contains(proposed, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var scopeNamesToRemove = currentScopeNames.Where(current
                => !proposedScopeNames.Contains(current, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // If there are no changes to make, return without making any
        if (!scopeNamesToAdd.Any() && !scopeNamesToRemove.Any())
            return;
        
        // Otherwise we're dirty...
        SetDateModified();
        IsDirty = true;

        // ... so we append the scopes collection (we do a loop to operate on the collection rather than writing a new one)
        var newScopes = scopes.Where(scope => scopeNamesToAdd.Contains(scope.Value))
            .DistinctBy(scope => scope.Value)
            .ToList();
        
        foreach(var scope in newScopes)
            Scopes.Add(scope);
        
        // ... and we remove the scopes that we no longer want
        var oldScopes = Scopes.Where(scope => scopeNamesToRemove.Contains(scope.Value))
            .ToList();

        foreach (var scope in oldScopes)
            Scopes.Remove(scope);
    }
    
    /// <summary>The unique identifier of the grant within the IdP service.</summary>
    public string ExternalId { get; set; }
    
    /// <summary>The unique identifier of the client with which this grant is associated.</summary>
    public string ClientId { get; set; }
    
    /// <summary>The unique identifier of the resource server to which this grant gives access.</summary>
    public string ResourceServerId { get; set; }
    
    /// <summary>The client with which this grant is associated.</summary>
    public required Client Client { get; init; }
    
    /// <summary>The resource server to which this grant gives access.</summary>
    public required ResourceServer ResourceServer { get; init; }
    
    /// <summary>The scopes associated with this grants access.</summary>
    public ICollection<Scope> Scopes { get; init; } = new List<Scope>();

    public override object AsSerializable()
        => new { Id, ClientId, ResourceServerId, Scopes = Scopes.Select(scope => scope.Value) };
}