using System.Text.Json.Serialization;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.Services.Notifications.Models;

public class MessageEnvelope
{
    private readonly string _entityType;
    private readonly string _action;
    
    /// <summary>The event that triggered the publication of this message.</summary>
    [JsonPropertyName("type")]
    public string Type => $"{_entityType}.{_action}";
    
    /// <summary>The version of the event message payload.</summary>
    [JsonPropertyName("version")]
    public int Version { get; }
    
    /// <summary>The timestamp of the message creation.</summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; } = DateTimeOffsetProvider.Now;

    /// <summary>The message payload.</summary>
    [JsonPropertyName("data")]
    public object Payload { get; }
    
    /// <summary>Initializes a new instance of the <see cref="MessageEnvelope"/> class.</summary>
    /// <param name="entityType">The type of entity that the notification is associated with.</param>
    /// <param name="action">The action that caused the notification to be raised.</param>
    /// <param name="content">Details of the event.</param>
    public MessageEnvelope(string entityType, string action, object content)
    {
        _entityType = entityType;
        _action = action;
        
        Payload = content;
        Version = 1;
    }
    
    /// <summary>Initializes a new instance of the <see cref="MessageEnvelope"/> class.</summary>
    /// <param name="entityType">The type of entity that the notification is associated with.</param>
    /// <param name="action">The action that caused the notification to be raised.</param>
    /// <param name="version">Version information about the content model.</param>
    /// <param name="content">Details of the event.</param>
    public MessageEnvelope(string entityType, string action, int version, object content)
    {
        _entityType = entityType;
        _action = action;
        
        Payload = content;
        Version = version;
    }
    
    /// <summary>Gets a <see cref="MessageEnvelope"/> object representing a product creation event.</summary>
    /// <param name="resourceServer">The created product.</param>
    public static MessageEnvelope ProductCreated(ResourceServer resourceServer)
        => ProductMessageEnvelope("created", resourceServer);
    
    /// <summary>Gets a <see cref="MessageEnvelope"/> object representing a product update event.</summary>
    /// <param name="resourceServer">The created product.</param>
    public static MessageEnvelope ProductModified(ResourceServer resourceServer)
        => ProductMessageEnvelope("modified", resourceServer);
    
    /// <summary>Gets a <see cref="MessageEnvelope"/> object representing a product deletion event.</summary>
    /// <param name="resourceServer">The created product.</param>
    public static MessageEnvelope ProductDeleted(ResourceServer resourceServer)
        => ProductMessageEnvelope("deleted", resourceServer);
    
    /// <summary>Gets a <see cref="MessageEnvelope"/> object representing a product event.</summary>
    /// <param name="resourceServer">The product.</param>
    /// <param name="eventType">The type of event.</param>
    private static MessageEnvelope ProductMessageEnvelope(string eventType, ResourceServer resourceServer)
        => new("product", eventType, content: resourceServer.AsSerializable());
}