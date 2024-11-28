using FluentValidation.Results;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Services.Notifications.Models;
using Reapit.Platform.Products.Core.UnitTests.TestServices;
using Reapit.Platform.Products.Core.UseCases.Products.PatchProduct;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Products.PatchProduct;

public class PatchProductCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly MockNotificationsService _notifications = new();
    private readonly IValidator<PatchProductCommand> _validator = Substitute.For<IValidator<PatchProductCommand>>();
    private readonly FakeLogger<PatchProductCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        ConfigureValidation(false);

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ThrowNotFoundException_WhenProductNotFound()
    {
        const string id = "test-id";
        
        ConfigureValidation(true);

        _productRepository.GetProductByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(null));

        var request = GetRequest(id: id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsWithoutUpdate_WhenEntityUnchanged()
    {
        var product = new Product("name", "description");
        
        ConfigureValidation(true);

        _productRepository.GetProductByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var request = GetRequest(id: product.Id, name: product.Name, description: product.Description);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(product);

        await _productRepository.DidNotReceive().UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_ReturnsAfterUpdate_WhenChanged()
    {
        var product = new Product("name", "description");
        
        ConfigureValidation(true);

        _productRepository.GetProductByIdAsync(product.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        // Fix the time so that the generated notification timestamp is static.
        using var _ = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        
        var request = GetRequest(id: product.Id, name: "new name", description: null);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(product);

        await _productRepository.Received(1).UpdateAsync(Arg.Is<Product>(productParam => productParam.Name == request.Name), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        var expectedNotification = MessageEnvelope.ProductModified(actual);
        _notifications.LastMessage.Should().BeEquivalentTo(expectedNotification);
    }
    
    /*
     * Private methods
     */

    private PatchProductCommandHandler CreateSut()
    {
        _unitOfWork.Products.Returns(_productRepository);
        return new PatchProductCommandHandler(_unitOfWork, _notifications, _validator, _logger);
    }

    private static PatchProductCommand GetRequest(string id = "id", string? name = "name", string? description = "description")
        => new(id, name, description);

    private void ConfigureValidation(bool isSuccessful)
    {
        var result =  isSuccessful
            ? new ValidationResult()
            : new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]);

        _validator.ValidateAsync(Arg.Any<PatchProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(result);
    }
}