using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1.Examples;

/// <summary>Example provider for the <see cref="ProductDetailsModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ProductDetailsModelExampleProvider : IExamplesProvider<ProductDetailsModel>
{
    private static readonly DateTime BaseDate = new DateTime(2024, 11, 26, 20, 14, 54, DateTimeKind.Utc);

    /// <inheritdoc/>
    public ProductDetailsModel GetExamples()
        => new ProductDetailsModel(
            Id: "a1f9d63c7e0e46d6934666e1b6c229de", 
            Name: "My Product", 
            Description: "An example product with a bit of a silly name.",
            DateCreated: BaseDate, 
            DateModified: BaseDate.AddHours(3));
}