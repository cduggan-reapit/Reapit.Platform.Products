using System.Text.Json;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities.Interfaces;
using Reapit.Platform.Products.Domain.Services;

namespace Reapit.Platform.Products.Domain.Entities.Abstract;

/// <summary>Base class for owned entities.</summary>
public abstract class EntityBase : IHasCursor
{
    /// <summary>Initializes a new instance of the <see cref="EntityBase"/> class.</summary>
    protected EntityBase()
    {
        Id = IdentityGenerator.Create();
        SetDateCreated();
    }

    /// <summary>The unique identifier of the entity.</summary>
    public string Id { get; init; }
    
    /// <summary>The cursor of the entity, used when paging results.</summary>
    public long Cursor { get; private set; }
    
    /// <summary>The creation timestamp of the entity.</summary>
    public DateTime DateCreated { get; private set; }
    
    /// <summary>The last modification timestamp of the entity.</summary>
    public DateTime DateModified { get; set; }
    
    /// <summary>The deletion timestamp of the entity.</summary>
    public DateTime? DateDeleted { get; private set; }
    
    /// <summary>Flag indicating whether the entity has been changed.</summary>
    public bool IsDirty { get; protected set; }
    
    /// <summary>
    /// Method to determine the value of a field in an update operation, setting the last modified date and dirty flag
    /// if the value should be changed. 
    /// </summary>
    /// <param name="current">The current value of the field.</param>
    /// <param name="updated">The proposed value of the field.</param>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <returns>The new value when `proposed` neither null nor equal to `current`, otherwise current.</returns>
    internal T GetUpdateValue<T>(T current, T? updated)
    {
        // If no value provided, return unchanged
        if(updated == null)
            return current;
        
        // If the value is unchanged, return unchanged
        if(updated.Equals(current))
            return current;
        
        // Otherwise return the updated value
        IsDirty = true;
        SetDateModified();
        return updated;
    }

    /// <summary>
    /// Method to determine the value of a collection property in an update operation, setting the last modified date
    /// and dirty flag if the value should be changed. 
    /// </summary>
    /// <param name="current">The current value of the field.</param>
    /// <param name="proposed">The proposed value of the field.</param>
    /// <typeparam name="T">The type of object in the collection.</typeparam>
    internal ICollection<T>? GetCollectionUpdateValue<T>(ICollection<T>? current, ICollection<T>? proposed)
    {
        var nullSafeCurrent = current ?? [];
        var nullSafeProposed = proposed ?? [];
        
        // If both collections are the same (including both empty) don't change anything
        if (nullSafeCurrent.SequenceEqual(nullSafeProposed))
            return current;

        // Otherwise it's dirty
        SetDateModified();
        IsDirty = true;

        return proposed;
    }
    
    /// <summary>Set the creation date to the current timestamp.</summary>
    private void SetDateCreated()
    {
        // Get the date once so all the two date fields get the same value
        var created = DateTimeOffsetProvider.Now;
        
        DateCreated = created.UtcDateTime;
        DateModified = created.UtcDateTime;
        
        // And the cursor is the unix epoch in microseconds
        Cursor = (long)(created - DateTimeOffset.UnixEpoch).TotalMicroseconds;
    }

    /// <summary>Set the modified date to the current timestamp.</summary>
    protected void SetDateModified() 
        => DateModified = DateTimeOffsetProvider.Now.UtcDateTime;

    /// <summary>Marks the entity as deleted.</summary>
    public void SoftDelete()
        => DateDeleted = DateTimeOffsetProvider.Now.UtcDateTime;
    
    /// <summary>Gets an anonymous, serializable object representing this entity.</summary>
    public abstract object AsSerializable();

    /// <inheritdoc />
    public override string ToString() 
        => JsonSerializer.Serialize(AsSerializable());
}