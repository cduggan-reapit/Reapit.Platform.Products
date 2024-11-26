using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Products.Api.Controllers.Shared.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1;

/// <summary>Endpoints for management of product information.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType<ProblemDetails>(400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class ProductsController
{
    [HttpGet]
    [ProducesResponseType<object>(200)]
    [ProducesResponseType<ProblemDetails>(400)]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetProducts()
        => throw new NotImplementedException();
    
    [HttpGet("id")]
    [ProducesResponseType<object>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetProductById([FromRoute] string id)
        => throw new NotImplementedException();
    
    [HttpPost]
    [ProducesResponseType<object>(201)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> CreateProduct()
        => throw new NotImplementedException();
    
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> UpdateProduct([FromRoute] string id)
        => throw new NotImplementedException();
    
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteProduct([FromRoute] string id)
        => throw new NotImplementedException();
}