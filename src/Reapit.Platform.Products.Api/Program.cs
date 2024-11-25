using Reapit.Platform.ApiVersioning;
using Reapit.Platform.ApiVersioning.Options;
using Reapit.Platform.Common;
using Reapit.Platform.ErrorHandling;
using Reapit.Platform.Swagger;
using Reapit.Platform.Swagger.Configuration;
using Reapit.Platform.Products.Api.Exceptions;
using Reapit.Platform.Products.Core;
using Reapit.Platform.Products.Data;

const string apiVersionHeader = "x-api-version";

var builder = WebApplication.CreateBuilder(args);

/*
 * Configure the service container
 */

// Add services from Reapit packages
builder.Services.AddCommonServices()
    .AddErrorHandlingServices()
    .AddRangedApiVersioning(typeof(Program).Assembly, new VersionedApiOptions { ApiVersionHeader = apiVersionHeader })
    .AddReapitSwagger(new ReapitSwaggerOptions { ApiVersionHeader = apiVersionHeader, DocumentTitle = "Reapit Demo API" });

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

app.UseReapitSwagger()
    .UseErrorHandlingServices()
    .RegisterCommonExceptions()
    .RegisterExceptions();

app.UseHttpsRedirection();
app.UseRangedApiVersioning();
app.UseRouting();
app.UseEndpoints(endpoint => endpoint.MapControllers());

app.Run();

/// <summary>Class description allowing test service injection.</summary>
public partial class Program { }