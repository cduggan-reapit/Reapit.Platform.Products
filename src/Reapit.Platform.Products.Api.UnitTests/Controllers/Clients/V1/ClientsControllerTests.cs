using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Clients.V1;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Core.UseCases.Clients.DeleteClient;
using Reapit.Platform.Products.Core.UseCases.Clients.GetClientById;
using Reapit.Platform.Products.Core.UseCases.Clients.GetClients;
using Reapit.Platform.Products.Core.UseCases.Clients.PatchClient;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Clients.V1;

public class ClientsControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<ClientsProfile>())
        .CreateMapper();
    
    /*
     * GetClients
     */

    [Fact]
    public async Task GetClients_ReturnsOk_WithResultPage()
    {
        var model = new GetClientsRequestModel(Cursor: 100L);
        var query = _mapper.Map<GetClientsQuery>(model);

        var entities = new[] { GetEntity(null) };
        var expected = _mapper.Map<ResultPage<ClientModel>>(entities);

        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entities);

        var sut = CreateSut();
        var response = await sut.GetClients(model) as OkObjectResult;
        response.Should().NotBeNull().And.Match<OkObjectResult>(result => result.StatusCode == 200);

        var content = response!.Value as ResultPage<ClientModel>;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetClientById
     */

    [Fact]
    public async Task GetClientById_ReturnsOk_WithDetailsModel()
    {
        const string id = "id";
        var query = new GetClientByIdQuery(id);

        var entity = GetEntity(null);
        var expected = _mapper.Map<ClientDetailsModel>(entity);

        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var response = await sut.GetClientById(id) as OkObjectResult;
        response.Should().NotBeNull().And.Match<OkObjectResult>(result => result.StatusCode == 200);

        var content = response!.Value as ClientDetailsModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateClient
     */
    
    [Fact]
    public async Task CreateClient_ReturnsCreated_WithSimpleModel()
    {
        var model = new CreateClientRequestModel(
            Name: "name", 
            Description: "description",
            Type: "machine", 
            AppId: "app-id",
            LoginUrl: "login-url",
            CallbackUrls: ["call-back"],
            SignOutUrls: ["sign-out"]);

        var command = _mapper.Map<CreateClientCommand>(model);

        var entity = GetEntity(null);

        var expected = _mapper.Map<ClientModel>(entity);

        _mediator.Send(Arg.Any<CreateClientCommand>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<CreateClientCommand>().Should().BeEquivalentTo(command);
                return entity;
            });
        
        var sut = CreateSut();
        var response = await sut.CreateClient(model) as CreatedAtActionResult;
        response.Should().NotBeNull().And.Match<CreatedAtActionResult>(result 
            => result.StatusCode == 201 
                && result.ActionName == nameof(ClientsController.GetClientById)
                && result.RouteValues!["id"] as string == entity.Id);

        var content = response!.Value as ClientModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * PatchClient
     */
    
    [Fact]
    public async Task PatchClient_ReturnsNoContent()
    {
        const string id = "id";
        var model = new PatchClientRequestModel(
            Name: "name", 
            Description: "description",
            LoginUrl: "log-in",
            CallbackUrls: ["call-back"],
            SignOutUrls: ["sign-out"]);
        
        var command = new PatchClientCommand(
            Id: id, 
            Name: model.Name, 
            Description: model.Description,
            LoginUrl: model.LoginUrl,
            CallbackUrls: model.CallbackUrls,
            SignOutUrls: model.SignOutUrls);
        
        var sut = CreateSut();
        var response = await sut.PatchClient(id, model) as NoContentResult;
        response.Should().NotBeNull().And.Match<NoContentResult>(result => result.StatusCode == 204);
        
        // List the properties out so we can use SequenceEqual to compare scopes
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteClient
     */
    
    [Fact]
    public async Task DeleteClient_ReturnsNoContent()
    {
        const string id = "id";
        var command = new DeleteClientCommand(Id: id);
        
        var sut = CreateSut();
        var response = await sut.DeleteClient(id) as NoContentResult;
        response.Should().NotBeNull().And.Match<NoContentResult>(result => result.StatusCode == 204);
        
        // List the properties out so we can use SequenceEqual to compare scopes
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private ClientsController CreateSut()
        => new(_mediator, _mapper);
    
    private static Client GetEntity(DateTimeOffset? baseDate)
    {
        using var dateContext = new DateTimeOffsetProviderContext(baseDate ?? DateTimeOffset.UnixEpoch);
        var client = new Client(
            appId: "app-id", 
            externalId: "external-id", 
            type: ClientType.AuthCode, 
            name: "auth code client", 
            description: "description of an auth code client", 
            loginUrl: "https://example.com/login",
            callbackUrls: [ "https://example.com/callback" ],
            signOutUrls: [ "https://example.com/signOut" ]) 
        {
            DateModified = (baseDate ?? DateTimeOffset.UnixEpoch).UtcDateTime.AddYears(1),
        };
        
        client.Grants.Add(GetGrant(client.Id, "resource one"));
        client.Grants.Add(GetGrant(client.Id, "resource two"));
        client.Grants.Add(GetGrant(client.Id, "resource three", false));
        return client;
    }

    private static Grant GetGrant(string clientId, string resourceServerName, bool includeMapping = true)
    {
        var resourceServer = GetResourceServer(resourceServerName);
        
        var guid = Guid.NewGuid();
        using var _ = new GuidProviderContext(guid);
        var grant = new Grant("external-id-1", clientId, resourceServer.Id)
        {
            Client = default!,
            ResourceServer = resourceServer
        };

        return grant;
    }

    private static ResourceServer GetResourceServer(string name)
    {
        using var _ = new GuidProviderContext(Guid.NewGuid());
        return new ResourceServer("", "", name, 3600);
    }
}