using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Examples;

/// <summary>Examples provider for the <see cref="ProductClientDetailsModel"/> type.</summary>
public class ProductClientDetailsModelExampleProvider : IExamplesProvider<ProductClientDetailsModel>
{
    private static readonly DateTime BaseDate = new(2024, 11, 27, 13, 30, 52, DateTimeKind.Utc);
    
    /// <inheritdoc />
    public ProductClientDetailsModel GetExamples()
        => new(
            Id: "e64875d317524c2a97b87e20513bc49a",
            Name: "My Product M2M Client",
            Description: "Machine to machine client for the My Product product",
            Type: ClientType.ClientCredentials.Name,
            DateCreated: BaseDate,
            DateModified: BaseDate.AddDays(2),
            CallbackUrls: null,
            SignOutUrls: null,
            Product: new ProductClientDetailsProductModel(Id: "62b650b9769f4cc4b9638dc720ddfa1f", Name: "My Product"));
}