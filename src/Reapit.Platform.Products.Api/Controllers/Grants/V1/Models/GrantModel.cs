using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;

/// <summary>Representation of a grant.</summary>
/// <param name="Id">The unique identifier of the grant.</param>
/// <param name="Client">Lightweight representation of the client with which the grant is associated.</param>
/// <param name="ResourceServer">Lightweight representation of the resource server with which the grant is associated.</param>
/// <param name="Scopes">The collection of scopes allowed for the grant.</param>
/// <param name="DateCreated">The timestamp of the creation of the grant.</param>
/// <param name="DateModified">The timestamp of the last modification to the grant.</param>
public record GrantModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("client")] GrantClientModel Client,
    [property: JsonPropertyName("resourceServer")] GrantResourceServerModel ResourceServer,
    [property: JsonPropertyName("scopes")] IEnumerable<string> Scopes,
    [property: JsonPropertyName("created")] DateTime DateCreated, 
    [property: JsonPropertyName("modified")] DateTime DateModified);