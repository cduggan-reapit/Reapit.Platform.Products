using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1.Examples;

/// <summary>Example provider for the <see cref="ProductModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ProductModelExampleProvider : IExamplesProvider<ProductModel> 
{
    private static readonly DateTime BaseDate = new DateTime(2024, 11, 26, 20, 10, 18, DateTimeKind.Utc);

    /// <inheritdoc/>
    public ProductModel GetExamples() 
        => new("918b836660464618b448101d25a21ece", "My Product", BaseDate, BaseDate.AddDays(17));
}