using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Products.Api.Controllers.Products.V1;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Core.UseCases.Products.CreateProduct;
using Reapit.Platform.Products.Core.UseCases.Products.DeleteProduct;
using Reapit.Platform.Products.Core.UseCases.Products.GetProductById;
using Reapit.Platform.Products.Core.UseCases.Products.GetProducts;
using Reapit.Platform.Products.Core.UseCases.Products.PatchProduct;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Products.V1;

public class ProductsControllerTests
{
    private readonly ISender _mediator = Substitute.For<ISender>();
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProductsProfile>())
        .CreateMapper();
    
    /*
     * GetProducts
     */

    [Fact]
    public async Task GetProducts_ReturnsOk_WithResultPage()
    {
        var requestModel = new GetProductsRequestModel(100);
        var query = _mapper.Map<GetProductsQuery>(requestModel);

        var entities = new[] { GetProduct() };
        var expected = _mapper.Map<ResultPage<ProductModel>>(entities);
        
        _mediator.Send(query, Arg.Any<CancellationToken>())
            .Returns(entities);

        var sut = CreateSut();
        var response = await sut.GetProducts(requestModel) as OkObjectResult;
        response.Should().NotBeNull().And.Match((OkObjectResult result) => result.StatusCode == 200);

        var content = response!.Value as ResultPage<ProductModel>;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetProductById
     */

    [Fact]
    public async Task GetProductById_ReturnsOk_WithProductModel()
    {
        const string id = "instance-id";
        var entity = GetProduct();
        var expected = _mapper.Map<ProductDetailsModel>(entity);
        
        _mediator.Send(Arg.Is<GetProductByIdQuery>(q => q.Id == id), Arg.Any<CancellationToken>())
            .Returns(entity);
        
        var sut = CreateSut();
        var response = await sut.GetProductById(id) as OkObjectResult;
        response.Should().NotBeNull().And.Match((OkObjectResult result) => result.StatusCode == 200);

        var content = response!.Value as ProductDetailsModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateProduct
     */

    [Fact]
    public async Task CreateProduct_ReturnsCreated_WithProductModel()
    {
        var request = new CreateProductRequestModel("name", "description");
        var command = new CreateProductCommand(request.Name, request.Description);

        var entity = GetProduct();
        var expected = _mapper.Map<ProductModel>(entity);
        
        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(entity);
        
        var sut = CreateSut();
        var response = await sut.CreateProduct(request) as CreatedAtActionResult;
        response.Should().NotBeNull().And.Match((CreatedAtActionResult result) => result.StatusCode == 201);

        var content = response!.Value as ProductModel;
        content.Should().BeEquivalentTo(expected);

        response.ActionName.Should().Be(nameof(ProductsController.GetProductById));
        response.RouteValues.Should().Contain(item => item.Key == "id" && entity.Id.Equals(item.Value as string));
    }
    
    /*
     * PatchProduct
     */
    
    [Fact]
    public async Task PatchProduct_ReturnsNoContent()
    {
        var entity = GetProduct();
        var request = new PatchProductRequestModel("new name", null);
        var command = new PatchProductCommand(entity.Id, request.Name, null);
        
        var sut = CreateSut();
        var response = await sut.PatchProduct(entity.Id, request) as NoContentResult;
        response.Should().NotBeNull().And.Match((NoContentResult result) => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * DeleteProduct
     */
    
    [Fact]
    public async Task DeleteProduct_ReturnsNoContent()
    {
        var entity = GetProduct();
        var command = new SoftDeleteProductCommand(entity.Id);
        
        var sut = CreateSut();
        var response = await sut.DeleteProduct(entity.Id) as NoContentResult;
        response.Should().NotBeNull().And.Match((NoContentResult result) => result.StatusCode == 204);
        
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private ProductsController CreateSut()
        => new(_mediator, _mapper);

    private static Product GetProduct(string name = "name", string description = "description")
        => new(name, description);
}