using Auth0.ManagementApi;
using Microsoft.Extensions.Options;
using Reapit.Platform.Products.Core.Configuration;
using Reapit.Platform.Products.Core.Services.IdentityProvider.Factories;

namespace Reapit.Platform.Products.Core.UnitTests.Services.IdentityProvider.Factories;

public class IdentityProviderClientFactoryTests
{
    private readonly ITokenCache _tokenFactory = Substitute.For<ITokenCache>();
    private readonly IdentityProviderOptions _options = GetAuth0Options();
    
    /*
     * GetClientAsync
     */

    [Fact]
    public async Task GetClientAsync_ReturnsInstance()
    {
        const string token = "arbitrary-token";
        _tokenFactory.GetAccessTokenAsync(Arg.Any<CancellationToken>()).Returns(token);

        var sut = CreateSut();
        var actual = await sut.GetClientAsync(default);
        actual.Should().BeOfType<ManagementApiClient>()
            .And.NotBeNull();
    }
    
    /*
     * Private methods
     */
     
    private IdentityProviderClientFactory CreateSut()
    {
        var optionsProvider = Substitute.For<IOptions<IdentityProviderOptions>>();
        optionsProvider.Value.Returns(_options);
         
        return new IdentityProviderClientFactory(_tokenFactory, optionsProvider);
    }

    private static IdentityProviderOptions GetAuth0Options(
        string domain = "test-domain.com", 
        string clientId = "test-client-id", 
        string clientSecret = "test-client-secret", 
        int tokenCacheSeconds = 86400) 
        => new()
        {
            Domain = domain,
            ClientId = clientId,
            ClientSecret = clientSecret,
            TokenCacheSeconds = tokenCacheSeconds
        };
}