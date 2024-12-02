using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Examples;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Controllers.Shared.Examples;
using Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;
using Reapit.Platform.Products.Core.UseCases.Applications.DeleteApplication;
using Reapit.Platform.Products.Core.UseCases.Applications.GetApplicationById;
using Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;
using Reapit.Platform.Products.Core.UseCases.Applications.PatchApplication;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1;

/// <summary>Endpoints for interacting with applications.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class ApplicationsController(ISender mediator, IMapper mapper) : ReapitApiController
{
    /// <summary>Get a page of applications with optional filters.</summary>
    /// <param name="model">The request filters.</param>
    [HttpGet]
    [ProducesResponseType<ResultPage<ApplicationModel>>(200)]
    [ProducesResponseType<ProblemDetails>(400)]
    [SwaggerResponseExample(200, typeof(ApplicationModelCollectionExample))]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetApplications([FromQuery] GetApplicationsRequestModel model)
    {
        var query = mapper.Map<GetApplicationsQuery>(model);
        var entities = await mediator.Send(query);
        return Ok(mapper.Map<ResultPage<ApplicationModel>>(entities));
    }
    
    /// <summary>Get an application.</summary>
    /// <param name="id">The unique identifier of the application.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<ApplicationDetailsModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(ApplicationDetailsModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetApplicationById([FromRoute] string id)
    {
        var query = new GetApplicationByIdQuery(id);
        var entity = await mediator.Send(query);
        return Ok(mapper.Map<ApplicationDetailsModel>(entity));
    }
    
    /// <summary>Create a new application.</summary>
    /// <param name="model">Model defining the application to create.</param>
    [HttpPost]
    [ProducesResponseType<ApplicationModel>(201)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(CreateApplicationRequestModel), typeof(CreateApplicationRequestModelExample))]
    [SwaggerResponseExample(201, typeof(ApplicationModelExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequestModel model)
    {
        var request = mapper.Map<CreateApplicationCommand>(model);
        var entity = await mediator.Send(request);
        return CreatedAtAction(
            actionName: nameof(GetApplicationById), 
            routeValues: new { id = entity.Id }, 
            value: mapper.Map<ApplicationModel>(entity));
    }

    /// <summary>Update an application.</summary>
    /// <param name="id">The unique identifier of the application.</param>
    /// <param name="model">Model defining the application properties to update.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(PatchApplicationRequestModel), typeof(PatchApplicationRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> PatchApplication([FromRoute] string id, [FromBody] PatchApplicationRequestModel model)
    {
        var request = new PatchApplicationCommand(id, model.Name, model.Description);
        _ = await mediator.Send(request);
        return NoContent();
    }
    
    /// <summary>Delete an application.</summary>
    /// <param name="id">The unique identifier of the application.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> DeleteApplication([FromRoute] string id)
    {
        var request = new DeleteApplicationCommand(id);
        _ = await mediator.Send(request);
        return NoContent();
    }
}