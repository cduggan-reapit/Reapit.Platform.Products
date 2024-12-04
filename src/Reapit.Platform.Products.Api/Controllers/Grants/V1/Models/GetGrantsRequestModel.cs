using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;

/// <summary>Request to get a collection of grants matching the provided filters.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="ClientId">Limit results to records associated with the client with this unique identifier.</param>
/// <param name="ResourceServerId">Limit results to records associated with the resource server with this unique identifier.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date (UTC).</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date (UTC).</param>
public record GetGrantsRequestModel(
    [property: JsonPropertyName("cursor")] long? Cursor = null,
    [property: JsonPropertyName("pageSize")] int PageSize = 25,
    [property: JsonPropertyName("clientId")] string? ClientId = null,
    [property: JsonPropertyName("resourceServerId")] string? ResourceServerId = null,
    [property: JsonPropertyName("createdFrom")] DateTime? CreatedFrom = null,
    [property: JsonPropertyName("createdTo")] DateTime? CreatedTo = null,
    [property: JsonPropertyName("modifiedFrom")] DateTime? ModifiedFrom = null,
    [property: JsonPropertyName("modifiedTo")] DateTime? ModifiedTo = null);