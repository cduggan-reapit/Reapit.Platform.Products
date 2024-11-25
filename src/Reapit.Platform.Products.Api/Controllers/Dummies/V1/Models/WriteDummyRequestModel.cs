using System.Text.Json.Serialization;
using Reapit.Platform.Swagger.Attributes;

namespace Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;

/// <summary>Request model used when creating or updating a Dummy.</summary>
/// <param name="Name">The name of the Dummy to create.</param>
public record WriteDummyRequestModel([property: JsonPropertyName("name"), SwaggerRequired] string Name);