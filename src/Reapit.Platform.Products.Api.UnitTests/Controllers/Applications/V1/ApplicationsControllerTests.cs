using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Products.Api.Controllers.Applications.V1;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;
using Reapit.Platform.Products.Core.UseCases.Applications.DeleteApplication;
using Reapit.Platform.Products.Core.UseCases.Applications.GetApplicationById;
using Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;
using Reapit.Platform.Products.Core.UseCases.Applications.PatchApplication;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Applications.V1;

public class ApplicationsControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<ApplicationsProfile>())
        .CreateMapper();
    
    /*
     * GetApps
     */

    [Fact]
    public async Task GetApplications_ReturnsOk_WithResultPage()
    {
        var model = new GetApplicationsRequestModel(Cursor: 100L);
        var query = _mapper.Map<GetApplicationsQuery>(model);

        var entities = new[] { new App("name", "description", false) };
        var expected = _mapper.Map<ResultPage<ApplicationModel>>(entities);

        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entities);

        var sut = CreateSut();
        var response = await sut.GetApplications(model) as OkObjectResult;
        response.Should().NotBeNull().And.Match<OkObjectResult>(result => result.StatusCode == 200);

        var content = response!.Value as ResultPage<ApplicationModel>;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetAppById
     */

    [Fact]
    public async Task GetAppById_ReturnsOk_WithDetailsModel()
    {
        const string id = "id";
        var query = new GetApplicationByIdQuery(id);

        var entity = new App("name", "description", false);
        entity.Clients.Add(new Client(entity.Id, "external-id", ClientType.Machine, "machine-client", null, null, null, null));
        entity.Clients.Add(new Client(entity.Id, "external-id", ClientType.AuthCode, "auth-client", null, null, null, null));
        
        var expected = _mapper.Map<ApplicationDetailsModel>(entity);

        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var response = await sut.GetApplicationById(id) as OkObjectResult;
        response.Should().NotBeNull().And.Match<OkObjectResult>(result => result.StatusCode == 200);

        var content = response!.Value as ApplicationDetailsModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateApp
     */
    
    [Fact]
    public async Task CreateApplication_ReturnsCreated_WithSimpleModel()
    {
        var model = new CreateApplicationRequestModel(
            Name: "name", 
            Description: "description",
            IsFirstParty: false);

        var command = _mapper.Map<CreateApplicationCommand>(model);

        var entity = new App(model.Name, model.Description, model.IsFirstParty);

        var expected = _mapper.Map<ApplicationModel>(entity);

        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(entity);
        
        var sut = CreateSut();
        var response = await sut.CreateApplication(model) as CreatedAtActionResult;
        response.Should().NotBeNull().And.Match<CreatedAtActionResult>(result 
            => result.StatusCode == 201 
                && result.ActionName == nameof(ApplicationsController.GetApplicationById)
                && result.RouteValues!["id"] as string == entity.Id);

        var content = response!.Value as ApplicationModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * PatchApp
     */
    
    [Fact]
    public async Task PatchApplication_ReturnsNoContent()
    {
        const string id = "id";
        var model = new PatchApplicationRequestModel(
            Name: "name", 
            Description: "description");
        
        var command = new PatchApplicationCommand(
            Id: id, 
            Name: model.Name, 
            Description: model.Description);
        
        var sut = CreateSut();
        var response = await sut.PatchApplication(id, model) as NoContentResult;
        response.Should().NotBeNull().And.Match<NoContentResult>(result => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteApp
     */
    
    [Fact]
    public async Task DeleteApplication_ReturnsNoContent()
    {
        const string id = "id";
        var command = new DeleteApplicationCommand(Id: id);
        
        var sut = CreateSut();
        var response = await sut.DeleteApplication(id) as NoContentResult;
        response.Should().NotBeNull().And.Match<NoContentResult>(result => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private ApplicationsController CreateSut()
        => new(_mediator, _mapper);
}