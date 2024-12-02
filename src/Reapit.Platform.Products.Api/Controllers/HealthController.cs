using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Swashbuckle.AspNetCore.Annotations;

namespace Reapit.Platform.Products.Api.Controllers;

/// <summary>Endpoints for checking the status of the service</summary>
public class HealthController : ReapitApiController
{
    /// <summary>Endpoint used to confirm service is live.</summary>
    [HttpGet]
    [ApiVersionNeutral]
    [SwaggerIgnore]
    [ProducesResponseType(204)]
    public IActionResult HealthCheck() 
        => NoContent();
}