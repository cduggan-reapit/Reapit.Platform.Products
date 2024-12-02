using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Examples;

/// <summary>Example provider for the <see cref="ResourceServerModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ResourceServerModelExample : IExamplesProvider<ResourceServerModel>
{
    private static readonly DateTime BaseDate = new(2024, 12, 2, 8, 48, 32, DateTimeKind.Utc);

    /// <inheritdoc />
    public ResourceServerModel GetExamples()
        => new(
            Id: "552b294da03f4492b4136d0e03863b33", 
            Name: "Example API", 
            Audience: "https://example.net/audience",
            DateCreated: BaseDate, 
            DateModified: BaseDate.AddDays(3));
}