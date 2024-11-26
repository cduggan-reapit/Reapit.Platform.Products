using Reapit.Platform.Products.Core.Extensions;

namespace Reapit.Platform.Products.Api.Infrastructure.Logging;

/// <summary>Middleware to log received requests.</summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.</summary>
    /// <param name="next">The request delegate.</param>
    /// <param name="logger">The logging service.</param>
    public RequestLoggingMiddleware(
        RequestDelegate next, 
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invoke the middleware.</summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var logMessage = await GetRequestLoggerMessage(context.Request);
        _logger.Log(LogLevel.Information, logMessage.ToJson());
        
        await _next(context);
    }
    
    /// <summary>Get the request logger message model for an instance of HttpRequest.</summary>
    /// <param name="request">The request object.</param>
    private static async Task<RequestLoggingModel> GetRequestLoggerMessage(HttpRequest request)
        => new(
            Method: request.Method,
            Url: request.Path.Value, 
            Headers: request.Headers.ToDictionary(h => h.Key, h => h.Value.Select(v => v)),
            Query: request.Query.ToDictionary(q => q.Key, q => q.Value.Select(v => v)),
            Body: await ReadRequestBodyAsync(request));

    /// <summary>Read the body of an HttpRequest object.</summary>
    /// <param name="request">The request object.</param>
    private static async Task<string?> ReadRequestBodyAsync(HttpRequest request)
    {
        // Buffering adds weight and most calls won't have request body, so shortcut return to save the weight.
        if (request.ContentLength == 0)
            return null;

        // If there's body content, enable buffering so that it can be read more than once (i.e. by model binding + this middleware)
        if (!request.Body.CanSeek)
            request.EnableBuffering();

        // Read the data from the buffered stream
        request.Body.Position = 0;
        var reader = new StreamReader(request.Body);
        var content = await reader.ReadToEndAsync();

        // Return the pointer to be start of the stream and return the content.
        request.Body.Position = 0;
        return content;
    }
}