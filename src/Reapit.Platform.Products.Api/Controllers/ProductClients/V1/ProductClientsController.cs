﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Examples;
using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Controllers.Shared.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1;

/// <summary>Endpoints for management of product clients.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType<ProblemDetails>(400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
[Route("/api/product-clients")]
public class ProductClientsController(ISender mediator, IMapper mapper) : ReapitApiController
{
    /// <summary>Get a page of product clients with optional filters.</summary>
    /// <param name="model">The query filter model.</param>
    [HttpGet]
    [ProducesResponseType<ResultPage<ProductClientModel>>(200)]
    [ProducesResponseType<ProblemDetails>(400)]
    [SwaggerResponseExample(200, typeof(ProductClientModelResultPageExampleProvider))]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetProductClients([FromQuery] GetProductClientsRequestModel model)
    {
        throw new NotImplementedException();
    }

    /// <summary>Get a single product.</summary>
    /// <param name="id">The unique identifier of the product.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<ProductClientDetailsModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(ProductClientDetailsModelExampleProvider))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetProductClientById([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    /// <summary>Create a new product.</summary>
    /// <param name="model">Definition of the product to create.</param>
    [HttpPost]
    [ProducesResponseType<ProductClientModel>(201)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(CreateProductClientRequestModel), typeof(CreateProductClientRequestModelExampleProvider))]
    [SwaggerResponseExample(201, typeof(ProductClientModelExampleProvider))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> CreateProductClient([FromBody] CreateProductClientRequestModel model)
    {
        throw new NotImplementedException();
    }

    /// <summary>Update a product.</summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="model">Definition of the properties to update.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(PatchProductClientRequestModel), typeof(PatchProductClientRequestModelExampleProvider))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> PatchProductClient([FromRoute] string id, [FromBody] PatchProductClientRequestModel model)
    {
        throw new NotImplementedException();
    }

    /// <summary>Delete a product.</summary>
    /// <param name="id">The unique identifier of the product.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteProductClient([FromRoute] string id)
    {
        throw new NotImplementedException();
    }
}