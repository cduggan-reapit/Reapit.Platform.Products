using System.Net;
using Microsoft.Extensions.Options;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;

namespace Reapit.Platform.Products.Core.UnitTests.Services.IdentityProvider.Factories;

public class TokenCacheTests
{
    private HttpClient? _httpClient;
    private readonly IdentityProviderOptions _options = GetAuth0Options();
    private readonly FakeLogger<TokenCache> _logger = new();
    
    /*
     * GetAccessTokenAsync
     */

    [Fact]
    public async Task GetAccessTokenAsync_GetsNewToken_WhenFirstCalled()
    {
        const string token = "this is the token that gets returned";
        var messageHandler = new MockHttpMessageHandler(HttpStatusCode.OK, new { access_token = token });
        _httpClient = new HttpClient(messageHandler);

        var sut = CreateSut();
        var actual = await sut.GetAccessTokenAsync(default);
        actual.Should().Be(token);
        
        // Get the request from the mock message handler (sneakily also fails if more than one request is made)
        var request = messageHandler.Requests.Single();
        
        // We instantiate the authentication client within the method so that we can set the BaseUri from config, but we
        // control the HttpMessageHandler that it uses to make its requests.  These checks confirm that:
        // - the request is sent where we expect it to go:
        request.Uri.Should().BeEquivalentTo($"https://{_options.Domain}/oauth/token");
        
        // - the request has form data
        request.FormData.Should().NotBeNull();
        
        // - We're using the client_credentials flow (and know FormData isn't null now!)
        request.FormData!["grant_type"].Should().BeEquivalentTo("client_credentials");
        
        // - The payload contains the right client_id:
        request.FormData["client_id"].Should().BeEquivalentTo(_options.ClientId);

        // - The payload contains the right client_secret:
        request.FormData["client_secret"].Should().BeEquivalentTo(_options.ClientSecret);
        
        // - The payload contains the right audience:
        request.FormData["audience"].Should().BeEquivalentTo($"https://{_options.Domain}/api/v2/");
    }

    [Fact]
    public async Task GetAccessTokenAsync_ReusesToken_WhenCalledBeforeTokenExpires()
    {
        var firstRequestTime = new DateTimeOffset(2024, 11, 8, 12, 3, 14, TimeSpan.Zero);
        const string firstToken = "first-token";
        
        var secondRequestTime = firstRequestTime.AddSeconds(_options.TokenCacheSeconds - 1);
        
        var sut = CreateSut();
        
        using (_ = new DateTimeOffsetProviderContext(firstRequestTime))
        {
            _httpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, new { access_token = firstToken }));
            var token = await sut.GetAccessTokenAsync(default);
            token.Should().Be(firstToken);
            _logger.LatestRecord.Message.Should().StartWith("Refreshing access token. Next refresh at: ");
        }
        
        using (_ = new DateTimeOffsetProviderContext(secondRequestTime))
        {
            _httpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, new { access_token = "something else" }));
            var token = await sut.GetAccessTokenAsync(default);
            token.Should().Be(firstToken);
            _logger.LatestRecord.Message.Should().StartWith("Re-using cached access token. Next refresh at: ");
        }
    }
    
    [Fact]
    public async Task GetAccessTokenAsync_GetsNewToken_WhenCalledAfterTokenExpires()
    {
        var firstRequestTime = new DateTimeOffset(2024, 11, 8, 12, 3, 14, TimeSpan.Zero);
        const string firstToken = "first-token";
        
        var secondRequestTime = firstRequestTime.AddSeconds(_options.TokenCacheSeconds + 1);
        const string secondToken = "second-token";
        
        var sut = CreateSut();
        
        using (_ = new DateTimeOffsetProviderContext(firstRequestTime))
        {
            _httpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, new { access_token = firstToken }));
            var token = await sut.GetAccessTokenAsync(default);
            token.Should().Be(firstToken);
            _logger.LatestRecord.Message.Should().StartWith("Refreshing access token. Next refresh at: ");
        }
        
        using (_ = new DateTimeOffsetProviderContext(secondRequestTime))
        {
            _httpClient = new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, new { access_token = secondToken }));
            var token = await sut.GetAccessTokenAsync(default);
            token.Should().Be(secondToken);
            _logger.LatestRecord.Message.Should().StartWith("Refreshing access token. Next refresh at: ");
        }
    }

     /*
      * Private methods
      */
     
     private TokenCache CreateSut()
     {
         var httpClientFactory = Substitute.For<IHttpClientFactory>();
         httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_ => _httpClient);

         var optionsProvider = Substitute.For<IOptions<IdentityProviderOptions>>();
         optionsProvider.Value.Returns(_options);
         
         return new TokenCache(httpClientFactory, optionsProvider, _logger);
     }

     private static IdentityProviderOptions GetAuth0Options(
         string domain = "test-domain.com", 
         string clientId = "test-client-id", 
         string clientSecret = "test-client-secret", 
         int tokenCacheSeconds = 3600) 
         => new()
         {
             Domain = domain,
             ClientId = clientId,
             ClientSecret = clientSecret,
             TokenCacheSeconds = tokenCacheSeconds
         };
}