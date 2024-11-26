using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Infrastructure.Configuration;

/// <summary>Representation of the database user secret object.</summary>
public record DatabaseUserSecretModel(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("engine")] string Engine,
    [property: JsonPropertyName("host")] string Host,
    [property: JsonPropertyName("port")] int Port,
    [property: JsonPropertyName("dbClusterIdentifier")]
    string Cluster)
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetConnectionString(string databaseName)
        => $"Server={Host}; Port={Port}; Database={databaseName}; Uid={Username}; Pwd='{Password}';";
};