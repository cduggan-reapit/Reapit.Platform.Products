using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Examples;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Controllers.Shared.Examples;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Core.UseCases.Clients.DeleteClient;
using Reapit.Platform.Products.Core.UseCases.Clients.GetClientById;
using Reapit.Platform.Products.Core.UseCases.Clients.GetClients;
using Reapit.Platform.Products.Core.UseCases.Clients.PatchClient;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1;

/// <summary>Endpoints for interacting with applications.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class ClientsController(ISender mediator, IMapper mapper) : ReapitApiController
{
    /// <summary>Get a page of clients with optional filters.</summary>
    /// <param name="model">The request filters.</param>
    [HttpGet]
    [ProducesResponseType<ResultPage<ClientModel>>(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [SwaggerResponseExample(200, typeof(ClientModelCollectionExample))]
    [SwaggerResponseExample(400, typeof(QueryStringProblemDetailsExample))]
    public async Task<IActionResult> GetClients([FromQuery] GetClientsRequestModel model)
    {
        var request = mapper.Map<GetClientsQuery>(model);
        var entities = await mediator.Send(request);
        return Ok(mapper.Map<ResultPage<ClientModel>>(entities));
    }
    
    /// <summary>Get a client.</summary>
    /// <param name="id">The unique identifier of the client.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<ClientDetailsModel>(200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(200, typeof(ClientDetailsModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetClientById([FromRoute] string id)
    {
        var request = new GetClientByIdQuery(id);
        var entity = await mediator.Send(request);
        return Ok(mapper.Map<ClientDetailsModel>(entity));
    }
    
    /// <summary>Create a new client.</summary>
    /// <param name="model">Model defining the client to create.</param>
    [HttpPost]
    [ProducesResponseType<ClientModel>(201)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(CreateClientRequestModel), typeof(CreateClientRequestModelExample))]
    [SwaggerResponseExample(201, typeof(ClientModelExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientRequestModel model)
    {
        var command = new CreateClientCommand(
            model.AppId,
            model.Type,
            model.Name,
            model.Description,
            model.LoginUrl,
            model.CallbackUrls,
            model.SignOutUrls);

        var entity = await mediator.Send(command);
        return CreatedAtAction(nameof(GetClientById), new { id = entity.Id }, mapper.Map<ClientModel>(entity));
    }
    
    /// <summary>Update a client.</summary>
    /// <param name="id">The unique identifier of the client.</param>
    /// <param name="model">Model defining the properties to update.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    [SwaggerRequestExample(typeof(PatchClientRequestModel), typeof(PatchClientRequestModelExample))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> PatchClient([FromRoute] string id, PatchClientRequestModel model)
    {
        var command = new PatchClientCommand(id, model.Name, model.Description, model.LoginUrl, model.CallbackUrls, model.SignOutUrls);
        _ = await mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>Delete a client and any associated grants.</summary>
    /// <param name="id">The unique identifier of the client.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteClient([FromRoute] string id)
    {
        var command = new DeleteClientCommand(id);
        _ = await mediator.Send(command);
        return NoContent();
    }
}