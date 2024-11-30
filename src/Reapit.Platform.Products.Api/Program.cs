using System.Text.Json;
using System.Text.Json.Serialization;
using Reapit.Platform.ApiVersioning;
using Reapit.Platform.ApiVersioning.Options;
using Reapit.Platform.Cloud;
using Reapit.Platform.Common;
using Reapit.Platform.ErrorHandling;
using Reapit.Platform.Products.Api.Infrastructure.Configuration;
using Reapit.Platform.Products.Api.Infrastructure.Exceptions;
using Reapit.Platform.Products.Api.Infrastructure.Logging;
using Reapit.Platform.Products.Core;
using Reapit.Platform.Products.Data;
using Reapit.Platform.Swagger;
using Reapit.Platform.Swagger.Configuration;
using Serilog;

const string apiVersionHeader = "x-api-version";

var builder = WebApplication.CreateBuilder(args);

// Add logger
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
logger.Information("Logger configured");

// Inject configuration as necessary
builder.Services.AddReapitCloudServices();
builder.InjectRemoteConfiguration();

// Register reapit services
builder.Services.AddCommonServices()
    .AddErrorHandlingServices()
    .AddRangedApiVersioning(typeof(Reapit.Platform.Products.Api.Program).Assembly, new VersionedApiOptions { ApiVersionHeader = apiVersionHeader })
    .AddReapitSwagger(new ReapitSwaggerOptions { ApiVersionHeader = apiVersionHeader, DocumentTitle = $"Product Management API ({builder.Environment.EnvironmentName})" });

// Add services from other projects in this solution
builder.AddCoreServices()
    .AddDataServices();

// Add services for the Api project
builder.Services.AddAutoMapper(typeof(Reapit.Platform.Products.Api.Program).Assembly);

// Finally configure the controller routing and json serialization options
builder.Services.AddHttpClient();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // make sure null values are included in the response models 
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        
        // this is probably less "required" than "preferred" but still:
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

/*
 * Configure the application
 */

var app = builder.Build();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseReapitSwagger()
    .UseErrorHandlingServices()
    .RegisterCommonExceptions()
    .RegisterExceptions();

app.UseRangedApiVersioning();
app.UseRouting();
app.MapControllers();

app.Run();

namespace Reapit.Platform.Products.Api
{
    /// <summary>Class description allowing test service injection.</summary>
    public partial class Program { }
}