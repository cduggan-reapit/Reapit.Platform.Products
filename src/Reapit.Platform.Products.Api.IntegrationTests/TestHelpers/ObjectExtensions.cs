using System.Text;
using System.Text.Json;

namespace Reapit.Platform.Products.Api.IntegrationTests.TestHelpers;

public static class ObjectExtensions
{
    public static StringContent ToStringContent(this object obj)
        => new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
}