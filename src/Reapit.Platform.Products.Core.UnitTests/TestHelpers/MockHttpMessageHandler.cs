using System.Net;
using System.Text.Json;

namespace Reapit.Platform.Products.Core.UnitTests.TestHelpers;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string? _response;

    public ICollection<MockHttpMessageRequestLog> Requests { get; private set; } = new List<MockHttpMessageRequestLog>();
    public int RequestCount { get; private set; }
    
    public MockHttpMessageHandler(HttpStatusCode statusCode, object? response)
    {
        _statusCode = statusCode;

        switch (response)
        {
            case null:
                break;
            case string responseString:
                _response = responseString;
                break;
            default:
                _response = JsonSerializer.Serialize(response);
                break;
        }
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Log the request data
        Requests.Add(await MockHttpMessageRequestLog.FromRequestMessageAsync(request));

        var message = new HttpResponseMessage(_statusCode);
        if (_response is not null)
            message.Content = new StringContent(_response);

        RequestCount++;
        return message;
    }
}