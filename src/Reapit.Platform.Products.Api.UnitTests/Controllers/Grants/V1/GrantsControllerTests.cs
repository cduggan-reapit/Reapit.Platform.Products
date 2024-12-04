using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Grants.V1;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;
using Reapit.Platform.Products.Core.UseCases.Grants.DeleteGrant;
using Reapit.Platform.Products.Core.UseCases.Grants.GetGrantById;
using Reapit.Platform.Products.Core.UseCases.Grants.GetGrants;
using Reapit.Platform.Products.Core.UseCases.Grants.PatchGrant;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Grants.V1;

public class GrantsControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<GrantsProfile>())
        .CreateMapper();
    
    /*
     * GetGrants
     */

    [Fact]
    public async Task GetGrants_ReturnsOk_WithResultPage()
    {
        var model = new GetGrantsRequestModel(Cursor: 100L);
        var query = _mapper.Map<GetGrantsQuery>(model);

        var entities = new[] { GetEntity() };
        var expected = _mapper.Map<ResultPage<GrantModel>>(entities);

        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entities);

        var sut = CreateSut();
        var response = await sut.GetGrants(model) as OkObjectResult;
        response.Should().NotBeNull().And.Match<OkObjectResult>(result => result.StatusCode == 200);

        var content = response!.Value as ResultPage<GrantModel>;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetGrantById
     */

    [Fact]
    public async Task GetGrantById_ReturnsOk_WithDetailsModel()
    {
        const string id = "id";
        var query = new GetGrantByIdQuery(id);

        var entity = GetEntity(null);
        var expected = _mapper.Map<GrantModel>(entity);

        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var response = await sut.GetGrantById(id) as OkObjectResult;
        response.Should().NotBeNull().And.Match<OkObjectResult>(result => result.StatusCode == 200);

        var content = response!.Value as GrantModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateGrant
     */
    
    [Fact]
    public async Task CreateGrant_ReturnsCreated_WithSimpleModel()
    {
        var model = new CreateGrantRequestModel(
            ClientId: "clientId",
            ResourceServerId: "resourceServerId",
            Scopes: ["scope.one", "scope.two"]);

        var command = _mapper.Map<CreateGrantCommand>(model);

        var entity = GetEntity();

        var expected = _mapper.Map<GrantModel>(entity);

        _mediator.Send(Arg.Any<CreateGrantCommand>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callInfo.Arg<CreateGrantCommand>().Should().BeEquivalentTo(command);
                return entity;
            });
        
        var sut = CreateSut();
        var response = await sut.CreateGrant(model) as CreatedAtActionResult;
        response.Should().NotBeNull().And.Match<CreatedAtActionResult>(result 
            => result.StatusCode == 201 
                && result.ActionName == nameof(GrantsController.GetGrantById)
                && result.RouteValues!["id"] as string == entity.Id);

        var content = response!.Value as GrantModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * PatchGrant
     */
    
    [Fact]
    public async Task PatchGrant_ReturnsNoContent()
    {
        const string id = "id";
        var model = new PatchGrantRequestModel(Scopes: ["scope.one", "scope.two", "scope.three"]);
        var command = new PatchGrantCommand(Id: id, Scopes: model.Scopes);
        
        var sut = CreateSut();
        var response = await sut.PatchGrant(id, model) as NoContentResult;
        response.Should().NotBeNull().And.Match<NoContentResult>(result => result.StatusCode == 204);
        
        // List the properties out so we can use SequenceEqual to compare scopes
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteGrant
     */
    
    [Fact]
    public async Task DeleteGrant_ReturnsNoContent()
    {
        const string id = "id";
        var command = new DeleteGrantCommand(Id: id);
        
        var sut = CreateSut();
        var response = await sut.DeleteGrant(id) as NoContentResult;
        response.Should().NotBeNull().And.Match<NoContentResult>(result => result.StatusCode == 204);
        
        // List the properties out so we can use SequenceEqual to compare scopes
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private GrantsController CreateSut()
        => new(_mediator, _mapper);
    
    private static Grant GetEntity(
        string clientName = "client name",
        string clientType = "machine",
        string resourceServerName = "resource server name",
        ICollection<string>? scopes = null,
        DateTimeOffset? baseDate = null)
    {
        baseDate ??= DateTimeOffset.UnixEpoch;
        using var _ = new DateTimeOffsetProviderContext(baseDate.Value);
        
        var client = GetClient(clientName, clientType);
        var resourceServer = GetResourceServer(resourceServerName);
        return new Grant(
            externalId: "never exposed",
            clientId: client.Id,
            resourceServerId: resourceServer.Id)
        {
            Client = client,
            ResourceServer = resourceServer,
            Scopes = (scopes ?? []).Select(scope => new Scope(resourceServer.Id, scope, null)).ToList(),
            DateModified = baseDate.Value.AddYears(1).UtcDateTime
        };
    }
    
    private static Client GetClient(string name, ClientType? type = null)
        => new("", "", type ?? ClientType.Machine, name, null, null, null, null);

    private static ResourceServer GetResourceServer(string name) 
        => new("", "", name, 3600);
}