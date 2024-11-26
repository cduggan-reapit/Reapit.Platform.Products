using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1.Examples;

/// <summary>Example provider for a page of <see cref="ProductModel"/> objects.</summary>
[ExcludeFromCodeCoverage]
public class ProductModelResultPageExampleProvider : IExamplesProvider<ResultPage<ProductModel>> 
{
    private static readonly DateTime BaseDate = new DateTime(2024, 11, 26, 20, 10, 18, DateTimeKind.Utc);

    /// <inheritdoc/>
    public ResultPage<ProductModel> GetExamples()
    {
        var model = new ProductModel("918b836660464618b448101d25a21ece", "My Product", BaseDate, BaseDate.AddDays(17));
        var cursor = (long)(model.DateModified - DateTime.UtcNow).TotalMicroseconds;
        return new ResultPage<ProductModel>([model], 1, cursor);
    }
}