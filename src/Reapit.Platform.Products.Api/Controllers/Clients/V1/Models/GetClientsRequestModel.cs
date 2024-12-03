using System.Text.Json.Serialization;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Reapit.Platform.Swagger.Attributes;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;

/// <summary>Request for a page of clients with optional filters.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="AppId">Limit results to records associated with the application with this unique identifier.</param>
/// <param name="Type">The type of claim.</param>
/// <param name="Name">Limit results to records matching this name.</param>
/// <param name="Description">Limit results to records matching this description.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date.</param>
public record GetClientsRequestModel(
    [property: JsonPropertyName("cursor")] long? Cursor = null,
    [property: JsonPropertyName("pageSize")] int PageSize = 25,
    [property: JsonPropertyName("applicationId")] string? AppId = null, 
    [property: JsonPropertyName("type"), SwaggerSelect(typeof(ClientType))] string? Type = null, 
    [property: JsonPropertyName("name")] string? Name = null, 
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("createdFrom")] DateTime? CreatedFrom = null,
    [property: JsonPropertyName("createdTo")] DateTime? CreatedTo = null,
    [property: JsonPropertyName("modifiedFrom")] DateTime? ModifiedFrom = null,
    [property: JsonPropertyName("modifiedTo")] DateTime? ModifiedTo = null);