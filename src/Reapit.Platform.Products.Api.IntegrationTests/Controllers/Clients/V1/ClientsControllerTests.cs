using System.Net;
using AutoMapper;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Clients.V1;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.IntegrationTests.Controllers.Clients.V1;

public class ClientsControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private const string BaseUrl = "/api/clients";
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<ClientsProfile>())
        .CreateMapper();

    /*
     * GET / => GetClients
     */
    
    [Fact]
    public async Task GetClients_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetClients_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task GetClients_ReturnsBadRequest_WhenQueryStringParametersInvalid()
    {
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?cursor=-1");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.QueryStringInvalid);
    }

    [Fact]
    public async Task GetClients_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var expected = _mapper.Map<ResultPage<ClientModel>>(SeedClients.Take(3));
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?pageSize=3");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }
    
    /*
     * GET / => GetClientById
     */
    
    [Fact]
    public async Task GetClientById_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetClientById_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task GetClientById_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/missing");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact] 
    public async Task GetClientById_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var entity = SeedClients.ElementAt(13);
        var expected = _mapper.Map<ClientDetailsModel>(entity);
        
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{entity.Id}");
        await response.Should().HaveStatusCode(HttpStatusCode.OK)
            .And.HavePayloadAsync(expected);
    }
    
    /*
     * POST / => CreateClient
     */
    
    [Fact]
    public async Task CreateClient_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, null, content: GetCreateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task CreateClient_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, "0.9", content: GetCreateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task CreateClient_ReturnsUnprocessable_WhenValidationFailed()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: GetCreateModel(name: new string('a', 101)));
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    [Fact]
    public async Task CreateClient_ReturnsCreated_WhenEntityCreated()
    {
        const int appNumber = 24;
        var request = GetCreateModel(appId: $"{appNumber:D32}", name: "hot mulligan", description: "end eric sparrow", type: ClientType.AuthCode, loginUrl: "https://login", callbackUrls: ["callback-url"], signOutUrls: ["sign-out-url"]);
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: request);
        response.Should().HaveStatusCode(HttpStatusCode.Created);

        // Check what we can in the simple payload
        var created = await response.Content.ReadFromJsonAsync<ClientModel>();
        created.Should().Match<ClientModel>(model => model.Name == request.Name && model.Type == request.Type && model.AppId == request.AppId);
        
        // ... and check the rest from the detailed payload
        var check = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{created!.Id}");
        var checkModel = await check.Content.ReadFromJsonAsync<ClientDetailsModel>();
        checkModel!.Description.Should().BeEquivalentTo(request.Description);
        checkModel.LoginUrl.Should().BeEquivalentTo(request.LoginUrl);
        checkModel.CallbackUrls.Should().BeEquivalentTo(request.CallbackUrls);
        checkModel.SignOutUrls.Should().BeEquivalentTo(request.SignOutUrls);
    }
    
    /*
     * PATCH /{id} => PatchClient
     */
        
    [Fact]
    public async Task PatchClient_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", null, content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task PatchClient_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", "0.9", content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task PatchClient_ReturnsUnprocessable_WhenValidationFailed()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", content: GetUpdateModel(name: new string('a', 101)));
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    [Fact]
    public async Task PatchClient_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/missing", content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task PatchClient_ReturnsNoContent_WhenRequestSuccessful()
    {
        var entity = SeedClients.ElementAt(42);
        var request = GetUpdateModel(name: "updated entity name");
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/{entity.Id}", content: request);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
        
        // Quickly check the update took:
        var checkResponse = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{entity.Id}");
        var checkModel = await checkResponse.Content.ReadFromJsonAsync<ClientDetailsModel>();
        checkModel!.Name.Should().BeEquivalentTo(request.Name);
        checkModel.Description.Should().BeEquivalentTo(entity.Description);
    }
    
    /*
     * DELETE /{id} => DeleteClient
     */
    
    [Fact]
    public async Task DeleteClient_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task DeleteClient_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteClient_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/missing");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task DeleteClient_ReturnsNoContent_WhenRequestSuccessful()
    {
        var entity = SeedClients.ElementAt(20); 
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
    
    private static CreateClientRequestModel GetCreateModel(
        string appId = "...",
        string type = "machine",
        string name = "audience",
        string description = "name",
        string? loginUrl = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(appId, type, name, description, loginUrl, callbackUrls, signOutUrls);

    private static PatchClientRequestModel GetUpdateModel(
        string? name = null,
        string? description = null,
        string? loginUrl = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(name, description, loginUrl, callbackUrls, signOutUrls);
    
    private async Task InitializeDatabaseAsync()
    {
        await using var scope = ApiFactory.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<ProductDbContext>();

        _ = await dbContext.Database.EnsureDeletedAsync();
        _ = await dbContext.Database.EnsureCreatedAsync();
        
        // Prerequisites: Apps!
        dbContext.Apps.AddRange(SeedApps);
        dbContext.Clients.AddRange(SeedClients);
        
        await dbContext.SaveChangesAsync();
    }

    private static ICollection<Entities.App> SeedApps
        => Enumerable.Range(0, 25)
            .Select(seed =>
            {
                var seedGuid = new Guid($"{seed:D32}");
                var seedDate = BaseDate.AddDays(seed);
                using var guidContext = new GuidProviderContext(seedGuid);
                using var timeContext = new DateTimeOffsetProviderContext(seedDate);
                var isFirstParty = seed % 10 == 0;
                return new Entities.App($"App {seed:D3}", $"Description of App {seed:D3}", isFirstParty);
            })
            .ToList();

    private static ICollection<Entities.Client> SeedClients
        => Enumerable.Range(0, 50)
            .Select(seed =>
            {
                var appNumber = (seed - seed % 2) / 2;
                var appId = $"{appNumber:D32}";
                var type = seed % 2 == 0 ? ClientType.Machine : ClientType.AuthCode;

                var seedGuid = new Guid($"{seed:D32}");
                var seedDate = BaseDate.AddDays(seed);
                using var guidContext = new GuidProviderContext(seedGuid);
                using var timeContext = new DateTimeOffsetProviderContext(seedDate);
                
                var client = new Entities.Client(
                    appId, 
                    $"external-id-{seed:D2}", 
                    type,
                    $"App {appNumber:D2} - {type.Name} client",
                    $"Description of App {appNumber:D2} - {type.Name} client",
                    loginUrl: type == ClientType.AuthCode ? $"https://{appNumber:D2}.example.net/login" : null,
                    callbackUrls: type == ClientType.AuthCode ? [$"https://{appNumber:D2}.example.net/call-back"] : null,
                    signOutUrls : type == ClientType.AuthCode ? [$"https://{appNumber:D2}.example.net/sign-out"] : null);

                // Clients for apps that are multiples of 10 do not have any grants
                if (appNumber % 10 == 0)
                    return client;
                
                // The rest all have one grant (we'll spam resource servers here, it doesn't matter)
                var resourceServer = new Entities.ResourceServer($"resource-server-{seed:D2}", "", $"Resource Server {seed:D2}", seed);
                client.Grants.Add(new Entities.Grant($"external-grant-id-{seed:D2}", client.Id, resourceServer.Id){ ResourceServer = resourceServer, Client = client });

                return client;
            })
            .ToList();
}