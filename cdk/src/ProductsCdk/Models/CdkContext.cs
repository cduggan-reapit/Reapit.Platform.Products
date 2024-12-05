using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductsCdk.Models;

/// <summary>The base stack context.</summary>
/// <param name="AccountId">The AWS account identifier.</param>
/// <param name="Region">The deployment region.</param>
/// <param name="Environment">The name of the environment.</param>
public record CdkContext(
    [property: JsonPropertyName("account")] string AccountId,
    [property: JsonPropertyName("region")] string Region,
    [property: JsonPropertyName("ssm")] string Parameter,
    [property: JsonPropertyName("environment")] EnvironmentContext Environment,
    [property: JsonPropertyName("api")] ApiContext Api,
    [property: JsonPropertyName("database")] DatabaseContext Database)
{
    /// <summary>Create an instance of <see cref="CdkContext"/> from the node context.</summary>
    /// <param name="obj">The node context object.</param>
    public static CdkContext Create(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<CdkContext>(json) ?? throw new Exception("Failed to deserialize cdk.json");
    }
}