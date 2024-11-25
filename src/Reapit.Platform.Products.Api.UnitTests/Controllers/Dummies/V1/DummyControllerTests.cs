using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;
using Reapit.Platform.Products.Core.UseCases.Dummies.CreateDummy;
using Reapit.Platform.Products.Core.UseCases.Dummies.GetDummies;
using Reapit.Platform.Products.Core.UseCases.Dummies.GetDummyById;
using Reapit.Platform.Products.Core.UseCases.Dummies.UpdateDummy;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Dummies.V1;

/// <summary>
/// Unit tests for the dummy controller class. This should focus on logic within the controller that does not depend
/// on integration (e.g. services throwing exceptions) or middleware (e.g. authentication/authorization). 
/// </summary>
public class DummyControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    private readonly IMapper _mapper = new MapperConfiguration(cfg
            => cfg.AddProfile(typeof(DummyProfile)))
        .CreateMapper();

    /*
     * GetDummies
     */
    
    [Fact]
    public async Task GetDummies_ReturnsOk_WhenNothingReturned()
    {
        var data = Array.Empty<Dummy>();
        _mediator.Send(Arg.Any<GetDummiesQuery>(), Arg.Any<CancellationToken>())
            .Returns(data);
        
        var sut = CreateSut();
        var result = await sut.GetDummies(new ReadDummiesRequestModel(null, null, null, null, null)) as OkObjectResult;
        result?.StatusCode.Should().Be(200);
        
        var actual = result?.Value as IEnumerable<ReadDummyResponseModel>;
        actual.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetDummies_ReturnsPopulatedCollection_WhenDummiesFound()
    {
        var fixtureDate = new DateTimeOffset(2020, 1, 26, 10, 45, 15, TimeSpan.Zero);
        using var dateProvider = new DateTimeOffsetProviderContext(fixtureDate);
        
        var data = Enumerable.Range(1, 5)
            .Select(i => new Dummy($"test-dummy-{i}") { Id = new Guid($"{i:D32}") });
    
        _mediator.Send(Arg.Any<GetDummiesQuery>(), Arg.Any<CancellationToken>())
            .Returns(data);

        // We don't need to test the mapping here, that's done in DummyProfileTests.cs
        var expected = _mapper.Map<IEnumerable<ReadDummyResponseModel>>(data); 
        
        var sut = CreateSut();
        var result = await sut.GetDummies(new ReadDummiesRequestModel(null, null, null, null, null)) as OkObjectResult;
        result?.StatusCode.Should().Be(200);
        
        var actual = result?.Value as IEnumerable<ReadDummyResponseModel>;
        actual.Should().BeEquivalentTo(expected);
    }

    /*
     * GetDummyById
     */
    
    [Fact]
    public async Task GetDummies_ReturnsDummyModel_ForMatchedObject()
    {
        var fixtureDate = new DateTimeOffset(2020, 1, 26, 10, 45, 15, TimeSpan.Zero);
        using var dateProvider = new DateTimeOffsetProviderContext(fixtureDate);
        
        var data = new Dummy("test-dummy-1") { Id = Guid.NewGuid() };
    
        _mediator.Send(Arg.Any<GetDummyByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(data);
    
        var expected = new ReadDummyResponseModel($"{data.Id:N}", "test-dummy-1", fixtureDate.UtcDateTime, fixtureDate.UtcDateTime);
        
        var sut = CreateSut();
        var result = await sut.GetDummyById(data.Id.ToString()) as OkObjectResult;
        result?.StatusCode.Should().Be(200);
        
        var actual = result?.Value as ReadDummyResponseModel;
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateDummy
     */

    [Fact]
    public async Task CreateDummy_ReturnsCreated_WhenRequestSuccessful()
    {
        var fixtureDate = new DateTimeOffset(2020, 1, 26, 10, 45, 15, TimeSpan.Zero);
        using var dateProvider = new DateTimeOffsetProviderContext(fixtureDate);
        
        var data = new Dummy("test-dummy-1") { Id = Guid.NewGuid() };
        var expected = new ReadDummyResponseModel($"{data.Id:N}", data.Name, data.DateCreated, data.DateModified);
        
        // Check the name when configuring the mock to confirm we've called Map<> first
        _mediator.Send(Arg.Is<CreateDummyCommand>(c => c.Name == data.Name), Arg.Any<CancellationToken>())
            .Returns(data);
        
        var requestModel = new WriteDummyRequestModel(data.Name);
        var sut = CreateSut();
        var result = await sut.CreateDummy(requestModel) as CreatedAtActionResult;
        result?.StatusCode.Should().Be(201);
    
        // Check the result points to the get endpoint
        result?.ActionName.Should().BeEquivalentTo(nameof(DummyController.GetDummyById));
        result?.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "id", expected.Id } });
        
        // Check the payload is what we expect
        var actual = result?.Value as ReadDummyResponseModel;
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * UpdateDummy
     */

    [Fact]
    public async Task UpdateDummy_ReturnsUpdatedDummy_WhenUpdateSuccessful()
    {
        const string id = "test-id";
        const string name = "test-name";

        var dummy = new Dummy(name) { Id = Guid.NewGuid() };
        var expected = _mapper.Map<ReadDummyResponseModel>(dummy);

        _mediator.Send(Arg.Is<UpdateDummyCommand>(command => command.Id == id && command.Name == name), Arg.Any<CancellationToken>())
            .Returns(dummy);

        var sut = CreateSut();
        var actual = await sut.UpdateDummy(id, new WriteDummyRequestModel(name)) as OkObjectResult;
        actual!.StatusCode.Should().Be(200);

        var actualObject = actual?.Value as ReadDummyResponseModel;
        actualObject.Should().BeEquivalentTo(expected);
    }
    
    /*
     * DeleteDummy
     */

    [Fact]
    public async Task DeleteDummy_ReturnsNoContent_WhenDeletionSuccessful()
    {
        const string id = "test-id";

        var sut = CreateSut();
        var actual = await sut.DeleteDummy(id) as NoContentResult;
        actual!.StatusCode.Should().Be(204);
    }
    
    /*
     * Private methods
     */

    private DummyController CreateSut() 
        => new(_mapper, _mediator);
}