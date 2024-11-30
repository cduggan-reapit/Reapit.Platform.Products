namespace Reapit.Platform.Products.Domain.Entities.Interfaces;

public interface IHasScopes
{
    /// <summary>The collection of scopes associated with this entity.</summary>
    public ICollection<Scope> Scopes { get; }

    /// <summary>Update the entities scope collection.</summary>
    /// <param name="scopes">The desired scopes collection.</param>
    public void SetScopes(ICollection<Scope> scopes);
}