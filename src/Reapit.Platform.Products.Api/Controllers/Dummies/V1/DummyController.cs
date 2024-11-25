using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Swagger.Attributes;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1.Examples;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;
using Reapit.Platform.Products.Core.UseCases.Dummies.CreateDummy;
using Reapit.Platform.Products.Core.UseCases.Dummies.DeleteDummyById;
using Reapit.Platform.Products.Core.UseCases.Dummies.GetDummies;
using Reapit.Platform.Products.Core.UseCases.Dummies.GetDummyById;
using Reapit.Platform.Products.Core.UseCases.Dummies.UpdateDummy;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Dummies.V1;

/// <summary>Endpoints for interacting with Dummies.</summary>
[IntroducedInVersion(1,0)]
public class DummyController : ReapitApiController
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    
    /// <summary>Initializes a new instance of the <see cref="DummyController"/> class.</summary>
    /// <param name="mapper"></param>
    /// <param name="mediator"></param>
    public DummyController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>Fetch a collection of dummies.</summary>
    /// <param name="model">The request model.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReadDummyResponseModel>), 200)]
    [SwaggerResponseExample(200, typeof(ReadDummyModelCollectionExample))]
    public async Task<IActionResult> GetDummies([FromQuery] ReadDummiesRequestModel model)
    {
        var query = _mapper.Map<GetDummiesQuery>(model);
        var dummies = await _mediator.Send(query);
        return Ok(_mapper.Map<IEnumerable<ReadDummyResponseModel>>(dummies));
    }

    /// <summary>Fetch a single dummy by it's unique identifier.</summary>
    /// <param name="id">The unique identifier of the dummy.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReadDummyResponseModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerResponseExample(200, typeof(ReadDummyModelExample))]
    public async Task<IActionResult> GetDummyById([SwaggerRequired] string id)
    {
        var query = new GetDummyByIdQuery(id);
        var dummy = await _mediator.Send(query);
        return Ok(_mapper.Map<ReadDummyResponseModel>(dummy));
    }
    
    /// <summary>Create a new Dummy.</summary>
    /// <param name="model">Model describing the Dummy to create.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ReadDummyResponseModel), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    public async Task<IActionResult> CreateDummy([FromBody] WriteDummyRequestModel model)
    {
        var command = _mapper.Map<CreateDummyCommand>(model);
        var dummy = await _mediator.Send(command);
        var readModel = _mapper.Map<ReadDummyResponseModel>(dummy);
        return CreatedAtAction(nameof(GetDummyById), new { id = readModel.Id },  readModel);
    }

    /// <summary>Update an existing Dummy.</summary>
    /// <param name="id">The unique identifier of the Dummy.</param>
    /// <param name="model">Model describing the Dummy to update.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ReadDummyResponseModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    public async Task<IActionResult> UpdateDummy(string id, [FromBody] WriteDummyRequestModel model)
    {
        var command = new UpdateDummyCommand(id, model.Name);
        var dummy = await _mediator.Send(command);
        var readModel = _mapper.Map<ReadDummyResponseModel>(dummy);
        return Ok(readModel);
    }

    /// <summary>Delete an existing Dummy.</summary>
    /// <param name="id">The unique identifier of the Dummy.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 422)]
    public async Task<IActionResult> DeleteDummy(string id)
    {
        var command = new DeleteDummyByIdCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}