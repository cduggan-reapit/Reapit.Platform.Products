using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Domain.Entities;

/// <summary>Represents a grant.</summary>
public class Grant : EntityBase
{
    /// <summary>Initializes a new instance of the <see cref="Grant"/> class.</summary>
    /// <param name="client">The client with which this grant is associated.</param>
    /// <param name="resourceServer">The resource server to which this grant gives access.</param>
    /// <param name="scopes">The scopes associated with this grants access.</param>
    public Grant(Client client, ResourceServer resourceServer, ICollection<Scope> scopes)
        : this(client.Id, resourceServer.Id)
    {
        Client = client;
        ResourceServer = resourceServer;
        Scopes = scopes;
    }

    /// <summary>Initializes a new instance of the <see cref="Grant"/> class.</summary>
    /// <param name="clientId">The unique identifier of the client with which this grant is associated.</param>
    /// <param name="resourceServerId">The unique identifier of the resource server to which this grant gives access.</param>
    public Grant(string clientId, string resourceServerId)
    {
        ClientId = clientId;
        ResourceServerId = resourceServerId;
    }
    
    /// <summary>The unique identifier of the client with which this grant is associated.</summary>
    public string ClientId { get; set; }
    
    /// <summary>The unique identifier of the resource server to which this grant gives access.</summary>
    public string ResourceServerId { get; set; }
    
    /// <summary>The client with which this grant is associated.</summary>
    public Client? Client { get; init; }
    
    /// <summary>The resource server to which this grant gives access.</summary>
    public ResourceServer? ResourceServer { get; init; }
    
    /// <summary>The scopes associated with this grants access.</summary>
    public ICollection<Scope> Scopes { get; init; } = new List<Scope>();

    public override object AsSerializable()
        => new { Id, ClientId, ResourceServerId, Scopes = Scopes.Select(scope => scope.Value) };
}