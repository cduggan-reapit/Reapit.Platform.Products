using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.Products.Api.Controllers.Abstract;
using Reapit.Platform.Products.Api.Controllers.Shared.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1;

/// <summary>Endpoints for interacting with applications.</summary>
[IntroducedInVersion(1, 0)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class ApplicationsController : ReapitApiController
{
    
}