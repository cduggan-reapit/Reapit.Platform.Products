using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Examples;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Controllers.Shared.Examples;
using Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;
using Reapit.Platform.Products.Core.UseCases.Grants.DeleteGrant;
using Reapit.Platform.Products.Core.UseCases.Grants.GetGrantById;
using Reapit.Platform.Products.Core.UseCases.Grants.GetGrants;
using Reapit.Platform.Products.Core.UseCases.Grants.PatchGrant;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1;

/// <summary>Endpoints for interacting with grants.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class GrantsController(ISender mediator, IMapper mapper) : ReapitApiController 
{
    /// <summary>Get a page of grants with optional filters.</summary>
    /// <param name="model">The request filters.</param>
    [HttpGet]
    [ProducesResponseType<ResultPage<GrantModel>>(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [SwaggerResponseExample(200, typeof(GrantModelCollectionExample))]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetGrants([FromQuery] GetGrantsRequestModel model)
    {
        var request = mapper.Map<GetGrantsQuery>(model);
        var entities = await mediator.Send(request);
        return Ok(mapper.Map<ResultPage<GrantModel>>(entities));
    }
    
    /// <summary>Get a grant.</summary>
    /// <param name="id">The unique identifier of the grant.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<GrantModel>(200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(200, typeof(GrantModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetGrantById([FromRoute] string id)
    {
        var request = new GetGrantByIdQuery(id);
        var entity = await mediator.Send(request);
        return Ok(mapper.Map<GrantModel>(entity));
    }
    
    /// <summary>Create a new grant.</summary>
    /// <param name="model">Model defining the grant to create.</param>
    [HttpPost]
    [ProducesResponseType<GrantModel>(201)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(CreateGrantRequestModel), typeof(CreateGrantRequestModelExample))]
    [SwaggerResponseExample(201, typeof(GrantModelExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> CreateGrant([FromBody] CreateGrantRequestModel model)
    {
        var command = mapper.Map<CreateGrantCommand>(model);
        var entity = await mediator.Send(command);
        return CreatedAtAction(nameof(GetGrantById), new { id = entity.Id }, mapper.Map<GrantModel>(entity));
    }
    
    /// <summary>Update a grant.</summary>
    /// <param name="id">The unique identifier of the grant.</param>
    /// <param name="model">Model defining the properties to update.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(PatchGrantRequestModel), typeof(PatchGrantRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> PatchGrant([FromRoute] string id, PatchGrantRequestModel model)
    {
        var command = new PatchGrantCommand(id, model.Scopes);
        _ = await mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>Delete a grant and any associated grants.</summary>
    /// <param name="id">The unique identifier of the grant.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteGrant([FromRoute] string id)
    {
        var command = new DeleteGrantCommand(id);
        _ = await mediator.Send(command);
        return NoContent();
    }
}