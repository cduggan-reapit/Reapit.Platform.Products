using Reapit.Platform.Products.Domain.Entities.Abstract;
using Reapit.Platform.Products.Domain.Entities.Interfaces;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Representation of a resource server.</summary>
public class ResourceServer(string externalId, string audience, string name, int tokenLifetime) : EntityBase, IHasScopes
{
    /// <summary>Update the properties of the resource server.</summary>
    /// <param name="name">The name of the resource server.</param>
    /// <param name="tokenLifetime">Expiration value (in seconds) for access tokens.</param>
    /// <param name="scopes">The scopes available for this resource server.</param>
    public void Update(string? name = null, int? tokenLifetime = null, ICollection<Scope>? scopes = null)
    {
        Name = GetUpdateValue(Name, name);
        TokenLifetime = GetUpdateValue(TokenLifetime, tokenLifetime) ?? TokenLifetime;
    }

    /// <summary>The unique identifier of the resource server in the IdP system.</summary>
    public string ExternalId { get; } = externalId;
    
    /// <summary>The name of the resource server.</summary>
    public string Name { get; private set; } = name;

    /// <summary>Unique identifier for the API used as the audience parameter on authorization calls. Can not be changed once set.</summary>
    public string Audience { get; } = audience;

    /// <summary>Expiration value (in seconds) for access tokens.</summary>
    public int TokenLifetime { get; private set; } = tokenLifetime;
    
    /// <summary>The scopes available for this resource server.</summary>
    public ICollection<Scope> Scopes { get; init; } = new List<Scope>();
    
    /// <summary>The grants associated with this resource server.</summary>
    public ICollection<Grant> Grants { get; init; } = new List<Grant>();
    
    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, DateCreated, DateModified };

    /// <inheritdoc/>
    public void SetScopes(ICollection<Scope> scopes)
    {
        // Get the collections as a collection of names - that's what matters to this service:
        var proposedScopes = scopes.ToList();
        var currentScopes = Scopes.ToList();

        // Find the scopes we need to add 
        var scopesToAdd = proposedScopes.Where(proposed 
                => !currentScopes.Any(current => AreScopesEqual(current, proposed)))
            .ToList();

        var scopeNamesToRemove = currentScopes.Where(current
                => !proposedScopes.Any(proposed => AreScopesEqual(current, proposed)))
            .ToList();

        // If there are no changes to make, return without making any
        if (!scopesToAdd.Any() && !scopeNamesToRemove.Any())
            return;
        
        // Otherwise we're dirty...
        SetDateModified();
        IsDirty = true;

        // ... so we append the scopes collection (we do a loop to operate on the collection rather than writing a new one)
        foreach(var scope in scopesToAdd)
            Scopes.Add(scope);
        
        // ... and we remove the scopes that we no longer want
        foreach (var scope in scopeNamesToRemove)
            Scopes.Remove(scope);
        return;

        // Local function to compare the value & description of two scopes.
        bool AreScopesEqual(Scope a, Scope b)
        {
            // Treat null and empty as the same thing
            var aDesc = a.Description ?? string.Empty;
            var bDesc = b.Description ?? string.Empty;

            // The scopes are equal if the values are the same (case-insensitive) 
            return a.Value.Equals(b.Value, StringComparison.OrdinalIgnoreCase)
                   // and the descriptions are the same (case-sensitive).
                   && aDesc.Equals(bDesc, StringComparison.Ordinal);
        }
    }
}