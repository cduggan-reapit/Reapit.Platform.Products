using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Domain.Entities;

public class Dummy : EntityBase
{
    private string _name = default!;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Dummy"/> class.
    /// </summary>
    public Dummy()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dummy"/> class.
    /// </summary>
    /// <param name="name">The name of the Dummy.</param>
    public Dummy(string name)
    {
        _name = name;
        DateCreated = DateTimeOffsetProvider.Now.UtcDateTime;
        DateModified = DateCreated;
    }
    
    /// <summary>The name of the Dummy.</summary>
    /// <remarks>Updating this property also updates <see cref="DateModified"/>.</remarks>
    public string Name { 
        get => _name;
        set {
            DateModified = DateTimeOffsetProvider.Now.UtcDateTime;
            _name = value;
        }
    }
    
    /// <summary>The date and time on which the Dummy was created.</summary>
    /// <remarks>Represents a date and time in UTC.</remarks>
    public DateTime DateCreated { get; init; }
    
    /// <summary>The date and time on which the Dummy was last modified.</summary>
    /// <remarks>Represents a date and time in UTC.</remarks>
    public DateTime DateModified { get; set; }
}