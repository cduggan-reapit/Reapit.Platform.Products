using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Examples;

/// <summary>Examples provider for a result page containing <see cref="ProductClientModel"/> objects.</summary>
public class ProductClientModelResultPageExampleProvider : IExamplesProvider<ResultPage<ProductClientModel>>
{
    /// <inheritdoc />
    public ResultPage<ProductClientModel> GetExamples()
    {
        var model = new ProductClientModelExampleProvider().GetExamples();
        var cursor = (long)(model.DateModified - DateTime.UtcNow).TotalMicroseconds;
        return new ResultPage<ProductClientModel>([model], 1, cursor);
    }
}