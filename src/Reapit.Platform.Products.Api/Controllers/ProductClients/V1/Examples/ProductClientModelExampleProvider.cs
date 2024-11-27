using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Examples;

/// <summary>Example provider for the <see cref="ProductClientModel"/> class.</summary>
public class ProductClientModelExampleProvider : IExamplesProvider<ProductClientModel>
{
    private static readonly DateTime BaseDate = new(2024, 11, 27, 13, 30, 52, DateTimeKind.Utc);

    /// <inheritdoc />
    public ProductClientModel GetExamples()
        => new(
            Id: "e64875d317524c2a97b87e20513bc49a",
            Name: "My Product M2M Client",
            Type: ClientType.ClientCredentials.Name,
            ProductId: "62b650b9769f4cc4b9638dc720ddfa1f",
            DateCreated: BaseDate,
            DateModified: BaseDate.AddDays(2));
}