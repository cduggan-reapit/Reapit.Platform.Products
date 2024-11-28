using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Products.Api.Controllers.ProductClients.V1;
using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Core.UseCases.ProductClients.CreateProductClient;
using Reapit.Platform.Products.Core.UseCases.ProductClients.DeleteProductClient;
using Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClientById;
using Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClients;
using Reapit.Platform.Products.Core.UseCases.ProductClients.PatchProductClient;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.ProductClients.V1;

public class ProductClientsControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProductClientsProfile>())
        .CreateMapper();
    
    /*
     * GetProductClients
     */

    [Fact]
    public async Task GetProductClients_ReturnsOk_WithResultPage()
    {
        var requestModel = new GetProductClientsRequestModel(100);
        var query = _mapper.Map<GetProductClientsQuery>(requestModel);

        var entities = new[] { GetProductClient() };
        var expected = _mapper.Map<ResultPage<ProductClientModel>>(entities);
        
        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entities);

        var sut = CreateSut();
        var response = await sut.GetProductClients(requestModel) as OkObjectResult;
        response.Should().NotBeNull().And.Match((OkObjectResult result) => result.StatusCode == 200);

        var content = response!.Value as ResultPage<ProductClientModel>;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetProductClientById
     */

    [Fact]
    public async Task GetProductClientById_ReturnsOk_WithProductClientModel()
    {
        const string id = "instance-id";
        var entity = GetProductClient();
        var expected = _mapper.Map<ProductClientDetailsModel>(entity);
        
        _mediator.Send(Arg.Is<GetProductClientByIdQuery>(q => q.Id == id), Arg.Any<CancellationToken>())
            .Returns(entity);
        
        var sut = CreateSut();
        var response = await sut.GetProductClientById(id) as OkObjectResult;
        response.Should().NotBeNull().And.Match((OkObjectResult result) => result.StatusCode == 200);

        var content = response!.Value as ProductClientDetailsModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateProductClient
     */

    [Fact]
    public async Task CreateProductClient_ReturnsCreated_WithProductClientModel()
    {
        var request = new CreateProductClientRequestModel("product-id", "name", "description", ClientType.ClientCredentials.Name, null, null, null);
        var command = new CreateProductClientCommand(request.ProductId, request.Name, request.Description, request.Type, request.Audience, request.CallbackUrls, request.SignOutUrls);

        var entity = GetProductClient();
        var expected = _mapper.Map<ProductClientModel>(entity);
        
        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(entity);
        
        var sut = CreateSut();
        var response = await sut.CreateProductClient(request) as CreatedAtActionResult;
        response.Should().NotBeNull().And.Match((CreatedAtActionResult result) => result.StatusCode == 201);

        var content = response!.Value as ProductClientModel;
        content.Should().BeEquivalentTo(expected);

        response.ActionName.Should().Be(nameof(ProductClientsController.GetProductClientById));
        response.RouteValues.Should().Contain(item => item.Key == "id" && entity.Id.Equals(item.Value as string));
    }
    
    /*
     * PatchProductClient
     */
    
    [Fact]
    public async Task PatchProductClient_ReturnsNoContent()
    {
        var entity = GetProductClient();
        var request = new PatchProductClientRequestModel("new name", "new description", ["callback"], ["sign-out"]);
        var command = new PatchProductClientCommand(entity.Id, request.Name, request.Description, request.CallbackUrls, request.SignOutUrls);
        
        var sut = CreateSut();
        var response = await sut.PatchProductClient(entity.Id, request) as NoContentResult;
        response.Should().NotBeNull().And.Match((NoContentResult result) => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteProductClient
     */
    
    [Fact]
    public async Task DeleteProductClient_ReturnsNoContent()
    {
        var entity = GetProductClient();
        var command = new SoftDeleteProductClientCommand(entity.Id);
        
        var sut = CreateSut();
        var response = await sut.DeleteProductClient(entity.Id) as NoContentResult;
        response.Should().NotBeNull().And.Match((NoContentResult result) => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private ProductClientsController CreateSut()
        => new(_mediator, _mapper);

    private static ProductClient GetProductClient(
        string productId = "productId",
        string productName = "productName",
        string name = "name", 
        string description = "description",
        ClientType? type = null,
        string? audience = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(
            productId: productId, 
            clientId: "inaccessible", 
            grantId: "inaccessible", 
            name: name, 
            description: description, 
            type: type ?? ClientType.ClientCredentials, 
            audience: audience,
            callbackUrls: callbackUrls,
            signOutUrls: signOutUrls)
        {
            Product = new Product(productName, null)
        };
}