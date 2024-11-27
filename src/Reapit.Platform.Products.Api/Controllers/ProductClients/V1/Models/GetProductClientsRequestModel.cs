using System.Text.Json.Serialization;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Reapit.Platform.Swagger.Attributes;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;

/// <summary>Request model used when fetching a collection of products.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="Name">Limit results to records matching this name.</param>
/// <param name="Description">Limit results to records containing this description.</param>
/// <param name="ProductId">Limit results to records associated with the product with this unique identifier.</param>
/// <param name="Type">Limit results to records with this type value.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date (UTC).</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date (UTC).</param>
public record GetProductClientsRequestModel(
    [property: JsonPropertyName("cursor")] long? Cursor = null,
    [property: JsonPropertyName("pageSize")] int PageSize = 25,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("productId")] string? ProductId = null,
    [property: JsonPropertyName("type"), SwaggerSelect(typeof(ClientType), false)] string? Type = null,
    [property: JsonPropertyName("createdFrom")] DateTime? CreatedFrom = null,
    [property: JsonPropertyName("createdTo")] DateTime? CreatedTo = null,
    [property: JsonPropertyName("modifiedFrom")] DateTime? ModifiedFrom = null,
    [property: JsonPropertyName("modifiedTo")] DateTime? ModifiedTo = null);