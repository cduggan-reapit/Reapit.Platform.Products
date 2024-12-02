using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.DeleteResourceServer;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServerById;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServers;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.UpdateResourceServer;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.ResourceServers.V1;

public class ResourceServersControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<ResourceServersProfile>())
        .CreateMapper();
    
    /*
     * GetResourceServers
     */

    [Fact]
    public async Task GetResourceServers_ReturnsOk_WithResultPage()
    {
        var model = new GetResourceServersRequestModel(Cursor: 100L);
        var query = _mapper.Map<GetResourceServersQuery>(model);

        var entities = new[] { new ResourceServer("external-id", "audience", "name", 43200) };
        var expected = _mapper.Map<ResultPage<ResourceServerModel>>(entities);

        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entities);

        var sut = CreateSut();
        var response = await sut.GetResourceServers(model) as OkObjectResult;
        response.Should().NotBeNull().And.Match<OkObjectResult>(result => result.StatusCode == 200);

        var content = response!.Value as ResultPage<ResourceServerModel>;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetResourceServerById
     */

    [Fact]
    public async Task GetResourceServerById_ReturnsOk_WithDetailsModel()
    {
        const string id = "id";
        var query = new GetResourceServerByIdQuery(id);

        var entity = new ResourceServer("external-id", "audience", "name", 43200)
        {
            Scopes =
            [
                new Scope("", "scope.value", "scope.description")
            ]
        };
        var expected = _mapper.Map<ResourceServerDetailsModel>(entity);

        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var response = await sut.GetResourceServerById(id) as OkObjectResult;
        response.Should().NotBeNull().And.Match<OkObjectResult>(result => result.StatusCode == 200);

        var content = response!.Value as ResourceServerDetailsModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateResourceServer
     */
    
    [Fact]
    public async Task CreateResourceServer_ReturnsCreated_WithSimpleModel()
    {
        var model = new CreateResourceServerRequestModel(
            Name: "name", 
            Audience: "audience",
            TokenLifetime: 3600,
            Scopes: [new ResourceServerScopeModel("scope.value", "scope.description")]);

        var command = _mapper.Map<CreateResourceServerCommand>(model);

        var entity = new ResourceServer(
            "external-id",
            model.Audience,
            model.Name,
            model.TokenLifetime)
        {
            Scopes = model.Scopes.Select(scope => new Scope("", scope.Value, scope.Description)).ToList()
        };

        var expected = _mapper.Map<ResourceServerModel>(entity);

        _mediator.Send(Arg.Is<CreateResourceServerCommand>(actual 
                => actual.Name == command.Name
                && actual.Audience == command.Audience
                && actual.TokenLifetime == command.TokenLifetime
                && actual.Scopes.SequenceEqual(command.Scopes)), Arg.Any<CancellationToken>())
            .Returns(entity);
        
        var sut = CreateSut();
        var raw = await sut.CreateResourceServer(model);
        var response = raw as CreatedAtActionResult;
        response.Should().NotBeNull().And.Match<CreatedAtActionResult>(result 
            => result.StatusCode == 201 
                && result.ActionName == nameof(ResourceServersController.GetResourceServerById)
                && result.RouteValues!["id"] as string == entity.Id);

        var content = response!.Value as ResourceServerModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * PatchResourceServer
     */
    
    [Fact]
    public async Task PatchResourceServer_ReturnsNoContent()
    {
        const string id = "id";
        var model = new UpdateResourceServerRequestModel(
            Name: "name", 
            TokenLifetime: 3600,
            Scopes: [new ResourceServerScopeModel("scope.value", "scope.description")]);
        
        var command = new UpdateResourceServerCommand(
            Id: id, 
            Name: model.Name, 
            TokenLifetime: model.TokenLifetime, 
            Scopes: model.Scopes?.Select(scope => new RequestScopeModel(scope.Value, scope.Description)).ToList());
        
        var sut = CreateSut();
        var response = await sut.PatchResourceServer(id, model) as NoContentResult;
        response.Should().NotBeNull().And.Match<NoContentResult>(result => result.StatusCode == 204);
        
        // List the properties out so we can use SequenceEqual to compare scopes
        await _mediator.Received(1).Send(Arg.Is<UpdateResourceServerCommand>(
            actual => actual.Id == command.Id
            && actual.Name == command.Name
            && actual.TokenLifetime == command.TokenLifetime
            && actual.Scopes!.SequenceEqual(command.Scopes!)
            ), Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteResourceServer
     */
    
    [Fact]
    public async Task DeleteResourceServer_ReturnsNoContent()
    {
        const string id = "id";
        var command = new DeleteResourceServerCommand(Id: id);
        
        var sut = CreateSut();
        var response = await sut.DeleteResourceServer(id) as NoContentResult;
        response.Should().NotBeNull().And.Match<NoContentResult>(result => result.StatusCode == 204);
        
        // List the properties out so we can use SequenceEqual to compare scopes
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private ResourceServersController CreateSut()
        => new(_mediator, _mapper);
}