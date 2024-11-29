using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents an application.</summary>
/// <remarks>An application is a container for clients.</remarks>
public class App(string name) : EntityBase
{
    /// <summary>The name of the app.</summary>
    public string Name { get; private set; } = name;
    
    /// <summary>A description of the app.</summary>
    public string? Description { get; private set; }
    
    /// <summary>Flag indicating whether an application requires authorisation or is automatically available to users.</summary>
    public bool RequiresAuthorization { get; private set; }
    
    /// <summary>The user credentials (authorisation_code) clients associated with the app.</summary>
    public ICollection<Client>? Clients { get; private set; }

    /// <summary>Update an app.</summary>
    /// <param name="name">The name of the app.</param>
    /// <param name="description">A description of the app.</param>
    public void Update(string? name = null, string? description = null)
    {
        Name = GetUpdateValue(Name, name);
        Description = GetUpdateValue(Description, description);
    }
    
    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, DateCreated, DateModified };
}