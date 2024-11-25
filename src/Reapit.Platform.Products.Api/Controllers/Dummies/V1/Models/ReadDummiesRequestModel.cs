using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;

/// <summary>Request model used when fetching one or more Dummies.</summary>
/// <param name="Name">The name of the Dummy.</param>
/// <param name="CreatedFrom">Apply filter for Dummies created on or after a given date and time (UTC).</param>
/// <param name="CreatedTo">Apply filter for Dummies created before a given date and time (UTC).</param>
/// <param name="ModifiedFrom">Apply filter for Dummies last modified on or after a given date and time (UTC).</param>
/// <param name="ModifiedTo">Apply filter for Dummies last modified before a given date and time (UTC).</param>
public record ReadDummiesRequestModel(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("createdFrom")] DateTime? CreatedFrom,
    [property: JsonPropertyName("createdTo")] DateTime? CreatedTo,
    [property: JsonPropertyName("modifiedFrom")] DateTime? ModifiedFrom,
    [property: JsonPropertyName("modifiedTo")] DateTime? ModifiedTo);