using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents an application.</summary>
/// <remarks>An application is a container for clients.</remarks>
public class App(string name, string? description, bool isFirstParty) : EntityBase
{
    /// <summary>The name of the app.</summary>
    public string Name { get; private set; } = name;

    /// <summary>A description of the app.</summary>
    public string? Description { get; private set; } = description;

    /// <summary>Flag indicating whether an application can skip consent.</summary>
    /// <remarks>This cannot be changed after initialization.</remarks>
    public bool IsFirstParty { get; } = isFirstParty;
    
    /// <summary>Update an app.</summary>
    /// <param name="name">The name of the app.</param>
    /// <param name="description">A description of the app.</param>
    public void Update(string? name = null, string? description = null)
    {
        Name = GetUpdateValue(Name, name);
        Description = GetUpdateValue(Description, description);
    }
    
    /// <summary>The clients associated with this application.</summary>
    public ICollection<Client> Clients { get; } = new List<Client>();
    
    /// <inheritdoc/>
    public override object AsSerializable()
        => new { Id, Name, DateCreated, DateModified };
}