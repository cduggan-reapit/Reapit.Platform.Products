using System.Net;
using AutoMapper;
using Microsoft.IdentityModel.Logging;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Grants.V1;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.IntegrationTests.Controllers.Grants.V1;

public class GrantsControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private const string BaseUrl = "/api/grants";
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<GrantsProfile>())
        .CreateMapper();

    /*
     * GET / => GetGrants
     */
    
    [Fact]
    public async Task GetGrants_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetGrants_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task GetGrants_ReturnsBadRequest_WhenQueryStringParametersInvalid()
    {
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?cursor=-1");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.QueryStringInvalid);
    }

    [Fact]
    public async Task GetGrants_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var expected = _mapper.Map<ResultPage<GrantModel>>(SeedData.Take(3));
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?pageSize=3");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }
    
    /*
     * GET / => GetGrantById
     */
    
    [Fact]
    public async Task GetGrantById_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetGrantById_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task GetGrantById_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/missing");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact] 
    public async Task GetGrantById_ReturnsOk_WhenRequestSuccessful()
    {
        var entity = SeedData.ElementAt(12);
        var expected = _mapper.Map<GrantModel>(entity);
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{entity.Id}");
        await response.Should().HaveStatusCode(HttpStatusCode.OK)
            .And.HavePayloadAsync(expected);
    }
    
    /*
     * POST / => CreateGrant
     */
    
    [Fact]
    public async Task CreateGrant_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, null, content: GetCreateModel("", ""));
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task CreateGrant_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, "0.9", content: GetCreateModel("", ""));
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task CreateGrant_ReturnsUnprocessable_WhenValidationFailed()
    {
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: GetCreateModel("", ""));
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    [Fact]
    public async Task CreateGrant_ReturnsConflict_WhenClientAlreadyGrantedAccessToResourceServer()
    {
        await InitializeDatabaseAsync();
        
        const int clientNumber = 5;
        const int resourceServerNumber = 5;
        var model = new CreateGrantRequestModel($"{clientNumber:D32}", $"{resourceServerNumber:D32}", [$"{resourceServerNumber:D3}.read"]);
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: model);
        response.Should().HaveStatusCode(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task CreateGrant_ReturnsCreated_WhenEntityCreated()
    {
        await InitializeDatabaseAsync();
        
        const int clientNumber = 3;
        const int resourceServerNumber = 5;
        var model = new CreateGrantRequestModel($"{clientNumber:D32}", $"{resourceServerNumber:D32}", [$"{resourceServerNumber:D3}.read", $"{resourceServerNumber:D3}.admin"]);
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: model);
        await response.Should().HaveStatusCode(HttpStatusCode.Created)
            .And.MatchPayloadAsync<GrantModel>(actual => actual.Client.Id == model.ClientId
                                                         && actual.ResourceServer.Id == model.ResourceServerId
                                                         && actual.Scopes.SequenceEqual(model.Scopes));
    }
    
    /*
     * PATCH /{id} => PatchGrant
     */
        
    [Fact]
    public async Task PatchGrant_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", null, content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task PatchGrant_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", "0.9", content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task PatchGrant_ReturnsUnprocessable_WhenValidationFailed()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task PatchGrant_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/missing", content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task PatchGrant_ReturnsNoContent_WhenRequestSuccessful()
    {
        throw new NotImplementedException();
    }
    
    /*
     * DELETE /{id} => DeleteGrant
     */
    
    [Fact]
    public async Task DeleteGrant_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task DeleteGrant_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteGrant_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/missing");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task DeleteGrant_ReturnsNoContent_WhenRequestSuccessful()
    {
        var entity = SeedData.ElementAt(17);
        var url = $"{BaseUrl}/{entity.Id}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        var check = await SendRequestAsync(HttpMethod.Get, url);
        check.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
    
    /*
     * Private methods
     */
    
    private static CreateGrantRequestModel GetCreateModel(
        string clientId,
        string resourceServerId,
        ICollection<string>? scopes = null)
        => new(clientId, resourceServerId, scopes ?? []);

    private static PatchGrantRequestModel GetUpdateModel(ICollection<string>? scopes = null)
        => new(scopes ?? []);
    
    private async Task InitializeDatabaseAsync()
    {
        await using var scope = ApiFactory.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<ProductDbContext>();

        _ = await dbContext.Database.EnsureDeletedAsync();
        _ = await dbContext.Database.EnsureCreatedAsync();
        
        dbContext.Grants.AddRange(SeedData);
        
        await dbContext.SaveChangesAsync();
    }

    private static ICollection<Entities.Grant> SeedData
        => Enumerable.Range(0, 50)
            .Select(seed =>
            {
                var seedGuid = new Guid($"{seed:D32}");
                var seedDate = BaseDate.AddDays(seed);
                using var guidContext = new GuidProviderContext(seedGuid);
                using var timeContext = new DateTimeOffsetProviderContext(seedDate);

                var client = SeedClients.Single(client => client.Id == $"{seedGuid:N}");
                var resourceServer = SeedResourceServers.Single(resourceServer => resourceServer.Id == $"{seedGuid:N}");
                var scopes = resourceServer.Scopes.Take(2).ToList();
                return new Entities.Grant("", client.Id, resourceServer.Id)
                {
                    Scopes = scopes,
                    Client = client,
                    ResourceServer = resourceServer
                };
            })
            .ToList();
    
    private static ICollection<Entities.ResourceServer> SeedResourceServers
        => Enumerable.Range(0, 50)
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
    
    private static ICollection<Entities.Client> SeedClients
        => Enumerable.Range(0, 50)
            .Select(seed =>
            {
                var seedGuid = new Guid($"{seed:D32}");
                var seedDate = BaseDate.AddDays(seed);
                using var guidContext = new GuidProviderContext(seedGuid);
                using var timeContext = new DateTimeOffsetProviderContext(seedDate);
                return new Entities.Client(
                    appId: $"{seedGuid:N}", 
                    externalId: $"external-client-id-{seed:D3}",
                    type: ClientType.Machine, 
                    name: $"Client {seed:D3}",
                    description: null, 
                    loginUrl: null,
                    callbackUrls: null, 
                    signOutUrls: null)
                {
                    App = SeedApplications.Single(a => a.Id == $"{seedGuid:N}")
                };
            })
            .ToList();
    
    private static ICollection<Entities.App> SeedApplications
        => Enumerable.Range(0, 50)
            .Select(seed =>
            {
                var seedGuid = new Guid($"{seed:D32}");
                var seedDate = BaseDate.AddDays(seed);
                using var guidContext = new GuidProviderContext(seedGuid);
                using var timeContext = new DateTimeOffsetProviderContext(seedDate);
                return new Entities.App($"Application {seed:D3}", null, true);
            })
            .ToList();
}