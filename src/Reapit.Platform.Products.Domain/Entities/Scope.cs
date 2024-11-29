namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents a resource server scope.</summary>
/// <remarks>
/// We won't interact with scopes directly, we store them separately to enable validation between grant & resource
/// server. As such, I've not used EntityBase but will give it a simple auto-incremental identifier. We should take
/// care not to artificially inflate this when updating a scope collection. 
/// </remarks>
public class Scope(string resourceServerId, string value, string? description)
{
    /// <summary>The unique identifier of the scope.</summary>
    public int Id { get; set; }
    
    /// <summary>The value of the scope.</summary>
    public string Value { get; } = value;

    /// <summary>Optional user-friendly description of the scope.</summary>
    public string? Description { get; } = description;
    
    /// <summary>The unique identifier of the resource server with which this scope is associated.</summary>
    public string ResourceServerId { get; } = resourceServerId;
}