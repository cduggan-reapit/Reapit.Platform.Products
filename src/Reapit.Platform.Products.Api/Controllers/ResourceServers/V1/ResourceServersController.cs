using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Examples;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Controllers.Shared.Examples;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.DeleteResourceServer;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServerById;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServers;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.PatchResourceServer;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1;

/// <summary>Endpoints for interacting with resource servers.</summary>
[Route("/api/resource-servers")]
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class ResourceServersController(ISender mediator, IMapper mapper) : ReapitApiController
{
    /// <summary>Get a page of resource servers with optional filters.</summary>
    /// <param name="model">The optional filters to apply.</param>
    [HttpGet]
    [ProducesResponseType<ResultPage<ResourceServerModel>>(200)]
    [SwaggerResponseExample(200, typeof(ResourceServerModelPageExample))]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetResourceServers([FromQuery] GetResourceServersRequestModel model)
    {
        var query = mapper.Map<GetResourceServersQuery>(model);
        var result = await mediator.Send(query);
        return Ok(mapper.Map<ResultPage<ResourceServerModel>>(result));
    }

    /// <summary>Get a resource server.</summary>
    /// <param name="id">The unique identifier of the resource server.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<ResourceServerDetailsModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(ResourceServerDetailsModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetResourceServerById([FromRoute] string id)
    {
        var query = new GetResourceServerByIdQuery(id);
        var entity = await mediator.Send(query);
        return Ok(mapper.Map<ResourceServerDetailsModel>(entity));
    }
    
    /// <summary>Create a new resource server.</summary>
    /// <param name="model">Definition of the resource server to create.</param>
    [HttpPost]
    [ProducesResponseType<ResourceServerModel>(201)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(CreateResourceServerRequestModel), typeof(CreateResourceServerRequestModelExample))]
    [SwaggerResponseExample(201, typeof(ResourceServerModelExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> CreateResourceServer([FromBody] CreateResourceServerRequestModel model)
    {
        var command = mapper.Map<CreateResourceServerCommand>(model);
        var entity = await mediator.Send(command);
        return CreatedAtAction(
            actionName: nameof(GetResourceServerById), 
            routeValues: new { id = entity.Id }, 
            value: mapper.Map<ResourceServerModel>(entity));
    }
    
    /// <summary>Update a resource server.</summary>
    /// <param name="id">The unique identifier of the resource server.</param>
    /// <param name="model">Model defining the properties to update.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(UpdateResourceServerRequestModel), typeof(UpdateResourceServerRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> PatchResourceServer([FromRoute] string id, [FromBody] UpdateResourceServerRequestModel model)
    {
        var command = new PatchResourceServerCommand(
            id,
            model.Name,
            model.TokenLifetime,
            model.Scopes?.Select(scope => new RequestScopeModel(scope.Value, scope.Description)).ToList());

        _ = await mediator.Send(command);
        return NoContent();
    }

    /// <summary>Delete a resource server.</summary>
    /// <param name="id">The unique identifier of the resource server.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteResourceServer([FromRoute] string id)
    {
        var command = new DeleteResourceServerCommand(id);
        _ = await mediator.Send(command);
        return NoContent();
    }
}