using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Primitives;
using Reapit.Platform.Products.Api.Infrastructure.Logging;

namespace Reapit.Platform.Products.Api.UnitTests.Infrastructure.Logging;

public class RequestLoggingMiddlewareTests
{
    private readonly RequestDelegate _next = _ => Task.CompletedTask;
    private readonly FakeLogger<RequestLoggingMiddleware> _logger = new();
    
    /*
     * InvokeAsync
     */

    [Fact]
    public async Task InvokeAsync_LogsMessage_WhenContentLengthIsZero()
    {
        var context = new DefaultHttpContext
        {
            Request =
            {
                Method = "OPTIONS",
                Path = "/test/url",
                QueryString = new QueryString("?queryParam=1&otherParam=2&otherParam=3"),
            }
        };
        context.Request.Headers.Append("example", new StringValues(["1", "2", "3"]));
        context.Request.Headers.Append("other", "1");
        context.Request.Headers.Append("Content-Length", "0");

        var expectedLogObject = new RequestLoggingModel(
            Method: "OPTIONS",
            Url: "/test/url",
            Headers: new Dictionary<string, IEnumerable<string?>>
            {
                { "Content-Length", ["0"] },
                { "example", ["1", "2", "3"] },
                { "other", ["1"] }
            },
            Query: new Dictionary<string, IEnumerable<string?>>
            {
                { "queryParam", ["1"] },
                { "otherParam", ["2","3"] }
            },
            Body: null);
        
        var sut = CreateSut();
        await sut.InvokeAsync(context);

        var actual = _logger.LatestRecord.Message;
        var actualObject = JsonSerializer.Deserialize<RequestLoggingModel>(actual);
        actualObject.Should().BeEquivalentTo(expectedLogObject);
    }
    
    [Fact]
    public async Task InvokeAsync_LogsMessage_WhenContentLengthIsNotZero_AndBodyEmpty()
    {
        var context = new DefaultHttpContext
        {
            Request =
            {
                Method = "OPTIONS",
                Path = "/test/url",
                ContentLength = 1
            }
        };
        
        var sut = CreateSut();
        await sut.InvokeAsync(context);

        var actual = _logger.LatestRecord.Message;
        var actualObject = JsonSerializer.Deserialize<RequestLoggingModel>(actual);
        
        // We've said there's content, but the body is empty so it should return an empty string
        actualObject!.Body.Should().BeEquivalentTo(string.Empty);
    }
    
    [Fact]
    public async Task InvokeAsync_LogsMessage_WhenContentLengthIsNotZero_AndBodyNotEmpty()
    {
        const string contentString = "test-content-string";
        using var body = new MemoryStream(Encoding.UTF8.GetBytes(contentString));
        var context = new DefaultHttpContext
        {
            Request =
            {
                Method = "OPTIONS",
                Path = "/test/url",
                ContentLength = 1,
                Body = body
            }
        };
        
        var sut = CreateSut();
        await sut.InvokeAsync(context);

        var actual = _logger.LatestRecord.Message;
        var actualObject = JsonSerializer.Deserialize<RequestLoggingModel>(actual);
        actualObject!.Body.Should().BeEquivalentTo(contentString);
    }

    /*
     * Private methods
     */
    
    private RequestLoggingMiddleware CreateSut() 
        => new(_next, _logger);
}