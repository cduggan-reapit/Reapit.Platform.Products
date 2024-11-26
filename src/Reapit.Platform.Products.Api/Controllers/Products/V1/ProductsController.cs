using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Examples;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Controllers.Shared.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1;

/// <summary>Endpoints for management of product information.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType<ProblemDetails>(400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class ProductsController : ReapitApiController
{
    /// <summary>Get a page of products with optional filters.</summary>
    /// <param name="model">The query filter model.</param>
    [HttpGet]
    [ProducesResponseType<ResultPage<ProductModel>>(200)]
    [ProducesResponseType<ProblemDetails>(400)]
    [SwaggerResponseExample(200, typeof(ProductModelResultPageExampleProvider))]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequestModel model)
        => throw new NotImplementedException();
    
    /// <summary>Get a single product.</summary>
    /// <param name="id">The unique identifier of the product.</param>
    [HttpGet("id")]
    [ProducesResponseType<ProductDetailsModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(ProductDetailsModelExampleProvider))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetProductById([FromRoute] string id)
        => throw new NotImplementedException();
    
    /// <summary>Create a new product.</summary>
    /// <param name="model">Definition of the product to create.</param>
    [HttpPost]
    [ProducesResponseType<ProductModel>(201)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(CreateProductRequestModel), typeof(CreateProductRequestModelExampleProvider))]
    [SwaggerResponseExample(201, typeof(ProductModelExampleProvider))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestModel model)
        => throw new NotImplementedException();

    /// <summary>Update a product.</summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="model">Definition of the properties to update.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(PatchProductRequestModel), typeof(PatchProductRequestModelExampleProvider))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> UpdateProduct([FromRoute] string id, [FromBody] PatchProductRequestModel model)
        => throw new NotImplementedException();
    
    /// <summary>Delete a product.</summary>
    /// <param name="id">The unique identifier of the product.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteProduct([FromRoute] string id)
        => throw new NotImplementedException();
}