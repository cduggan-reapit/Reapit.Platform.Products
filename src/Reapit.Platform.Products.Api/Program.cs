using Reapit.Platform.ApiVersioning;
using Reapit.Platform.ApiVersioning.Options;
using Reapit.Platform.Cloud;
using Reapit.Platform.Common;
using Reapit.Platform.ErrorHandling;
using Reapit.Platform.Swagger;
using Reapit.Platform.Swagger.Configuration;
using Reapit.Platform.Products.Api.Exceptions;
using Reapit.Platform.Products.Api.Infrastructure.Configuration;
using Reapit.Platform.Products.Api.Infrastructure.Logging;
using Reapit.Platform.Products.Core;
using Reapit.Platform.Products.Data;
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
    .AddRangedApiVersioning(typeof(Program).Assembly, new VersionedApiOptions { ApiVersionHeader = apiVersionHeader })
    .AddReapitSwagger(new ReapitSwaggerOptions { ApiVersionHeader = apiVersionHeader, DocumentTitle = $"Product Management API ({builder.Environment.EnvironmentName})" });

// Add services from other projects in this solution
builder.AddCoreServices()
    .AddDataServices();

// Add services for the Api project
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddControllers();

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
app.UseEndpoints(endpoint => endpoint.MapControllers());

app.Run();

/// <summary>Class description allowing test service injection.</summary>
public partial class Program { }