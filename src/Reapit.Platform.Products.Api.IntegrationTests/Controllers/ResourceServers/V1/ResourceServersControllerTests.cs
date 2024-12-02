using System.Net;
using AutoMapper;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Data.Context;

namespace Reapit.Platform.Products.Api.IntegrationTests.Controllers.ResourceServers.V1;

public class ResourceServersControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<ResourceServersProfile>())
        .CreateMapper();

    private const string BaseUrl = "/api/resource-servers";
    
    /*
     * GET / => GetResourceServers
     */
    
    [Fact]
    public async Task GetResourceServers_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetResourceServers_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task GetResourceServers_ReturnsBadRequest_WhenQueryStringParametersInvalid()
    {
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?cursor=-1");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.QueryStringInvalid);
    }

    [Fact]
    public async Task GetResourceServers_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var expected = _mapper.Map<ResultPage<ResourceServerModel>>(SeedData.Take(3));
        
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?pageSize=3");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }
    
    /*
     * GET /{id} => GetResourceServerById
     */
    
    [Fact]
    public async Task GetResourceServerById_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetResourceServerById_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task GetResourceServerById_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/none");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task GetResourceServerById_ReturnsOk_WhenRequestSuccessful()
    {
        var entity = SeedData.ElementAt(37);
        var expected = _mapper.Map<ResourceServerDetailsModel>(entity);
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{entity.Id}");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }
    
    /*
     * POST / => CreateResourceServer
     */
    
    [Fact]
    public async Task CreateResourceServer_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var payload = GetCreateModel();
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, null, content: payload);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task CreateResourceServer_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var payload = GetCreateModel();
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, "0.9", content: payload);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task CreateResourceServer_ReturnsBadRequest_WhenValidationFailed()
    {
        await InitializeDatabaseAsync();
        var payload = GetCreateModel(name: new string('a', 500));
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: payload);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    [Fact]
    public async Task CreateResourceServer_ReturnsCrated_WhenEntityCreated()
    {
        await InitializeDatabaseAsync();
        var payload = GetCreateModel(
            audience: "test-audience",
            scopes: [
                new ResourceServerScopeModel("scope.one", null),
                new ResourceServerScopeModel("scope.two", "scope.two description")
            ]);
        
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: payload);
        response.Should().HaveStatusCode(HttpStatusCode.Created);
        
        // Create is a bit messy because we can't control the auto-generated stuff.  We check the properties we can:
        var responseModel = await response.Content.ReadFromJsonAsync<ResourceServerModel>();
        responseModel!.Name.Should().BeEquivalentTo(payload.Name);
        responseModel.Audience.Should().BeEquivalentTo(payload.Audience);

        // Get from details to check the stuff that's not in the simple model
        var detailResponse = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{responseModel.Id}");
        var detailModel = await detailResponse.Content.ReadFromJsonAsync<ResourceServerDetailsModel>();
        detailModel!.TokenLifetime.Should().Be(payload.TokenLifetime);
        
        // Scopes are a bit fiddly, so boil them down to tuples and compare those:
        var actualScopes = detailModel.Scopes.Select(a => (a.Value, a.Description)).ToList();
        var expectedScopes = payload.Scopes.Select(e => (e.Value, e.Description)).ToList();
        actualScopes.Should().BeEquivalentTo(expectedScopes);
    }
    
    /*
     * PATCH /{id} => PatchResourceServer
     */
    
    [Fact]
    public async Task PatchResourceServer_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var payload = GetUpdateModel();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", null, content: payload);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task PatchResourceServer_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var payload = GetUpdateModel();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", "0.9", content: payload);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task PatchResourceServer_ReturnsBadRequest_WhenValidationFailed()
    {
        await InitializeDatabaseAsync();
        var payload = GetUpdateModel(name: new string('a', 500));
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", content: payload);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    [Fact]
    public async Task PatchResourceServer_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var payload = GetUpdateModel();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/none", content: payload);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact]
    public async Task PatchResourceServer_ReturnsNoContent_WhenEntityUpdated()
    {
        await InitializeDatabaseAsync();
        var entity = SeedData.ElementAt(38);

        var payload = GetUpdateModel(name: "updated name");
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/{entity.Id}", content: payload);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    /*
     * DELETE /{id} => DeleteResourceServer
     */
    
    [Fact]
    public async Task DeleteResourceServer_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task DeleteResourceServer_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteResourceServer_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/none");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task DeleteResourceServer_ReturnsNoContent_WhenRequestSuccessful()
    {
        var entity = SeedData.ElementAt(39);
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/{entity.Id}");
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
        
        var checkResponse = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{entity.Id}");
        checkResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    /*
     * Private methods
     */
    
    private static CreateResourceServerRequestModel GetCreateModel(
        string audience = "audience",
        string name = "name",
        int tokenLifetime = 1234,
        ICollection<ResourceServerScopeModel>? scopes = null)
        => new(name, audience, tokenLifetime, scopes ?? []);

    private static UpdateResourceServerRequestModel GetUpdateModel(
        string? name = null,
        int? tokenLifetime = null,
        ICollection<ResourceServerScopeModel>? scopes = null)
        => new(name, tokenLifetime, scopes);
    
    private async Task InitializeDatabaseAsync()
    {
        await using var scope = ApiFactory.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<ProductDbContext>();

        _ = await dbContext.Database.EnsureDeletedAsync();
        _ = await dbContext.Database.EnsureCreatedAsync();
        
        dbContext.ResourceServers.AddRange(SeedData);

        await dbContext.SaveChangesAsync();
    }

    private static ICollection<Entities.ResourceServer> SeedData
        => Enumerable.Range(0, 100)
            .Select(seed =>
            {
                var seedGuid = new Guid($"{seed:D32}");
                var seedDate = BaseDate.AddDays(seed);
                using var guidContext = new GuidProviderContext(seedGuid);
                using var timeContext = new DateTimeOffsetProviderContext(seedDate);

                return new Entities.ResourceServer(
                    externalId: $"e:{seed:D3}",
                    name: $"Resource Server {seed:D3}",
                    audience: $"https://example.net/audience/{seed:D3}",
                    tokenLifetime: seed * 10
                )
                {
                    DateModified = seedDate.UtcDateTime.AddYears(1),
                    Scopes =
                    [
                        new Entities.Scope($"{seedGuid:N}", $"{seed:D3}.read", $"Example read scope for Resource Server {seed:D3}"),
                        new Entities.Scope($"{seedGuid:N}", $"{seed:D3}.write", $"Example write scope for Resource Server {seed:D3}"),
                        new Entities.Scope($"{seedGuid:N}", $"{seed:D3}.admin", $"Example admin scope for Resource Server {seed:D3}")
                    ]
                };
            })
            .ToList();
}