using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;

/// <summary>Request to get a collection of resource servers matching the provided filters.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="Name">Limit results to records matching this name.</param>
/// <param name="Audience">Limit results to records matching this audience.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date (UTC).</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date (UTC).</param>
public record GetResourceServersRequestModel(
    [property: JsonPropertyName("cursor")] long? Cursor = null,
    [property: JsonPropertyName("pageSize")] int PageSize = 25,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("audience")] string? Audience = null,
    [property: JsonPropertyName("createdFrom")] DateTime? CreatedFrom = null,
    [property: JsonPropertyName("createdTo")] DateTime? CreatedTo = null,
    [property: JsonPropertyName("modifiedFrom")] DateTime? ModifiedFrom = null,
    [property: JsonPropertyName("modifiedTo")] DateTime? ModifiedTo = null);
